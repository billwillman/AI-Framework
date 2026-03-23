using System.Collections.Generic;
using UnityEngine;

namespace MeshClusterSystem
{
    /// <summary>
    /// Mesh Cluster管理器
    /// </summary>
    public class MeshClusterManager : MonoBehaviour
    {
        [Header("集群设置")]
        [SerializeField] private int maxClusters = 100;
        [SerializeField] private int maxInstancesPerCluster = 100;
        [SerializeField] private float clusterRadius = 50f;
        [SerializeField] private bool enableDynamicClustering = true;
        [SerializeField] private float updateInterval = 0.5f;
        
        [Header("LOD设置")]
        [SerializeField] private int lodLevels = 3;
        [SerializeField] private float[] lodDistances = new float[] { 20f, 50f, 100f };
        [SerializeField] private float lodTransitionSmoothness = 0.25f;
        
        [Header("性能优化")]
        [SerializeField] private bool enableFrustumCulling = true;
        [SerializeField] private bool enableOcclusionCulling = true;
        [SerializeField] private int maxDrawCallsPerFrame = 100;
        
        // 集群数据
        private Dictionary<string, MeshClusterData> clusters = new Dictionary<string, MeshClusterData>();
        private List<MeshClusterData> visibleClusters = new List<MeshClusterData>();
        private Queue<MeshClusterData> clusterPool = new Queue<MeshClusterData>();
        
        // 渲染组件
        private Camera mainCamera;
        private Plane[] cameraFrustumPlanes = new Plane[6];
        
        // 统计信息
        private int totalInstances;
        private int visibleInstances;
        private int drawCallsSaved;
        private float lastUpdateTime;
        
        #region 单例模式
        private static MeshClusterManager instance;
        public static MeshClusterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MeshClusterManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("MeshClusterManager");
                        instance = go.AddComponent<MeshClusterManager>();
                    }
                }
                return instance;
            }
        }
        #endregion
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            Initialize();
        }
        
        private void Initialize()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            
            // 预创建集群池
            for (int i = 0; i < maxClusters; i++)
            {
                MeshClusterData cluster = new MeshClusterData
                {
                    clusterId = $"Cluster_{i}",
                    worldBounds = new Bounds(Vector3.zero, Vector3.one * clusterRadius)
                };
                clusterPool.Enqueue(cluster);
            }
        }
        
        private void Update()
        {
            if (Time.time - lastUpdateTime < updateInterval)
                return;
            
            lastUpdateTime = Time.time;
            
            // 更新可见性
            UpdateVisibility();
            
            // 动态集群更新
            if (enableDynamicClustering)
            {
                UpdateDynamicClusters();
            }
            
            // 渲染集群
            RenderClusters();
            
            // 更新统计
            UpdateStatistics();
        }
        
        /// <summary>
        /// 添加Mesh到集群系统
        /// </summary>
        public string AddMeshInstance(Mesh mesh, Material material, Vector3 position, 
            Quaternion rotation, Vector3 scale, bool isStatic = true)
        {
            if (mesh == null || material == null)
                return null;
                
            // 寻找合适的集群
            MeshClusterData targetCluster = FindSuitableCluster(position);
            if (targetCluster == null)
            {
                if (clusterPool.Count > 0)
                {
                    targetCluster = clusterPool.Dequeue();
                    clusters[targetCluster.clusterId] = targetCluster;
                }
                else
                {
                    // 合并最小的集群
                    targetCluster = MergeSmallestCluster();
                }
            }
            
            // 添加实例
            var instanceData = new MeshClusterData.MeshInstanceData
            {
                mesh = mesh,
                material = material,
                position = position,
                rotation = rotation,
                scale = scale,
                localToWorld = Matrix4x4.TRS(position, rotation, scale)
            };
            
            targetCluster.instances.Add(instanceData);
            targetCluster.worldBounds.Encapsulate(position);
            
            // 标记需要重新合并
            if (isStatic)
            {
                ScheduleClusterRebuild(targetCluster);
            }
            
            totalInstances++;
            return targetCluster.clusterId;
        }
        
        /// <summary>
        /// 移除Mesh实例
        /// </summary>
        public bool RemoveMeshInstance(string clusterId, int instanceIndex)
        {
            if (!clusters.ContainsKey(clusterId))
                return false;
                
            var cluster = clusters[clusterId];
            if (instanceIndex < 0 || instanceIndex >= cluster.instances.Count)
                return false;
                
            cluster.instances.RemoveAt(instanceIndex);
            totalInstances--;
            
            // 如果集群为空，回收
            if (cluster.instances.Count == 0)
            {
                clusters.Remove(clusterId);
                clusterPool.Enqueue(cluster);
            }
            else
            {
                ScheduleClusterRebuild(cluster);
            }
            
            return true;
        }
        
        /// <summary>
        /// 更新Mesh实例变换
        /// </summary>
        public void UpdateMeshInstance(string clusterId, int instanceIndex, 
            Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (!clusters.ContainsKey(clusterId))
                return;
                
            var cluster = clusters[clusterId];
            if (instanceIndex < 0 || instanceIndex >= cluster.instances.Count)
                return;
                
            var instance = cluster.instances[instanceIndex];
            instance.position = position;
            instance.rotation = rotation;
            instance.scale = scale;
            instance.localToWorld = Matrix4x4.TRS(position, rotation, scale);
            
            if (!cluster.isStatic)
            {
                ScheduleClusterRebuild(cluster);
            }
        }
        
        #region 私有方法
        private MeshClusterData FindSuitableCluster(Vector3 position)
        {
            MeshClusterData bestCluster = null;
            float bestDistance = float.MaxValue;
            
            foreach (var cluster in clusters.Values)
            {
                if (cluster.instances.Count >= maxInstancesPerCluster)
                    continue;
                    
                float distance = Vector3.Distance(cluster.worldBounds.center, position);
                if (distance < clusterRadius && distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCluster = cluster;
                }
            }
            
            return bestCluster;
        }
        
        private MeshClusterData MergeSmallestCluster()
        {
            MeshClusterData smallest = null;
            int minCount = int.MaxValue;
            
            foreach (var cluster in clusters.Values)
            {
                if (cluster.instances.Count < minCount)
                {
                    minCount = cluster.instances.Count;
                    smallest = cluster;
                }
            }
            
            return smallest;
        }
        
        private void ScheduleClusterRebuild(MeshClusterData cluster)
        {
            // 标记为需要重建
            cluster.lastUpdateTime = Time.time;
        }
        
        private void UpdateVisibility()
        {
            if (!enableFrustumCulling || mainCamera == null)
            {
                visibleClusters.Clear();
                visibleClusters.AddRange(clusters.Values);
                return;
            }
            
            // 计算相机视锥体
            GeometryUtility.CalculateFrustumPlanes(mainCamera, cameraFrustumPlanes);
            
            visibleClusters.Clear();
            visibleInstances = 0;
            
            foreach (var cluster in clusters.Values)
            {
                if (GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, cluster.worldBounds))
                {
                    visibleClusters.Add(cluster);
                    visibleInstances += cluster.instances.Count;
                }
            }
        }
        
        private void UpdateDynamicClusters()
        {
            // 动态集群更新逻辑
            // 可以根据性能需求调整集群大小和分布
        }
        
        private void RenderClusters()
        {
            if (mainCamera == null)
                return;
                
            int drawCalls = 0;
            Vector3 cameraPos = mainCamera.transform.position;
            
            foreach (var cluster in visibleClusters)
            {
                if (drawCalls >= maxDrawCallsPerFrame)
                    break;
                    
                // LOD选择
                float distance = Vector3.Distance(cameraPos, cluster.worldBounds.center);
                int lodLevel = CalculateLODLevel(distance, cluster);
                
                // 渲染集群
                if (cluster.combinedMesh != null)
                {
                    RenderCluster(cluster, lodLevel);
                    drawCalls++;
                }
            }
        }
        
        private int CalculateLODLevel(float distance, MeshClusterData cluster)
        {
            for (int i = 0; i < lodDistances.Length; i++)
            {
                if (distance < lodDistances[i])
                    return i;
            }
            return lodDistances.Length - 1;
        }
        
        private void RenderCluster(MeshClusterData cluster, int lodLevel)
        {
            // 使用GPU实例化渲染
            if (cluster.instanceMatrices != null && cluster.instanceMatrices.Length > 0)
            {
                for (int i = 0; i < cluster.materials.Length; i++)
                {
                    Graphics.DrawMeshInstanced(
                        cluster.combinedMesh, 
                        i, 
                        cluster.materials[i], 
                        cluster.instanceMatrices,
                        cluster.instanceMatrices.Length,
                        null,
                        UnityEngine.Rendering.ShadowCastingMode.On,
                        true
                    );
                }
            }
        }
        
        private void UpdateStatistics()
        {
            // 更新性能统计
            drawCallsSaved = totalInstances - visibleClusters.Count;
        }
        #endregion
        
        #region 公共属性
        public int TotalInstances => totalInstances;
        public int VisibleInstances => visibleInstances;
        public int DrawCallsSaved => drawCallsSaved;
        public int ClusterCount => clusters.Count;
        public int VisibleClusterCount => visibleClusters.Count;
        #endregion
        
        #region 调试方法
        public void DrawClusterBounds()
        {
            foreach (var cluster in clusters.Values)
            {
                Gizmos.color = visibleClusters.Contains(cluster) ? Color.green : Color.red;
                Gizmos.DrawWireCube(cluster.worldBounds.center, cluster.worldBounds.size);
            }
        }
        
        public void PrintStatistics()
        {
            Debug.Log($"Mesh Cluster Statistics:");
            Debug.Log($"  Total Instances: {totalInstances}");
            Debug.Log($"  Visible Instances: {visibleInstances}");
            Debug.Log($"  Clusters: {clusters.Count}");
            Debug.Log($"  Visible Clusters: {visibleClusters.Count}");
            Debug.Log($"  Draw Calls Saved: {drawCallsSaved}");
        }
        #endregion
    }
}