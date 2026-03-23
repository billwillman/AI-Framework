using System.Collections.Generic;
using UnityEngine;

namespace MeshClusterSystem
{
    /// <summary>
    /// LOD管理器 - 负责LOD级别的管理和切换
    /// </summary>
    public class LODManager : MonoBehaviour
    {
        [System.Serializable]
        public class LODSettings
        {
            [Range(0, 1)] public float screenRelativeTransitionHeight = 0.5f;
            [Range(0, 1)] public float simplificationRatio = 0.5f;
            public bool useBoundingBoxForLowestLOD = true;
            public bool enableCrossFade = true;
            public float crossFadeTransitionWidth = 0.25f;
        }
        
        [Header("LOD设置")]
        [SerializeField] private int maxLODLevels = 4;
        [SerializeField] private LODSettings[] lodSettings;
        [SerializeField] private float[] lodDistances = new float[] { 10f, 20f, 50f, 100f };
        [SerializeField] private bool useHysteresis = true;
        [SerializeField] private float hysteresisThreshold = 0.1f;
        
        [Header("性能优化")]
        [SerializeField] private bool enableAsyncLODGeneration = true;
        [SerializeField] private int maxAsyncOperations = 4;
        [SerializeField] private bool cacheLODMeshes = true;
        
        // LOD缓存
        private Dictionary<Mesh, Mesh[]> lodCache = new Dictionary<Mesh, Mesh[]>();
        private Dictionary<Mesh, Bounds> meshBoundsCache = new Dictionary<Mesh, Bounds>();
        
        // 异步操作队列
        private Queue<LODGenerationTask> generationQueue = new Queue<LODGenerationTask>();
        private List<LODGenerationTask> activeOperations = new List<LODGenerationTask>();
        
        private Camera mainCamera;
        private float lastLODUpdateTime;
        private const float LOD_UPDATE_INTERVAL = 0.2f;
        
        private class LODGenerationTask
        {
            public Mesh originalMesh;
            public int lodLevel;
            public float simplificationRatio;
            public System.Action<Mesh> onComplete;
            public bool isCompleted;
            public Mesh result;
        }
        
        private void Start()
        {
            mainCamera = Camera.main;
            InitializeLODSettings();
        }
        
        private void Update()
        {
            if (Time.time - lastLODUpdateTime < LOD_UPDATE_INTERVAL)
                return;
                
            lastLODUpdateTime = Time.time;
            
            // 处理异步LOD生成
            ProcessAsyncOperations();
            
            // 更新所有集群的LOD
            UpdateAllClustersLOD();
        }
        
        private void InitializeLODSettings()
        {
            if (lodSettings == null || lodSettings.Length != maxLODLevels)
            {
                lodSettings = new LODSettings[maxLODLevels];
                for (int i = 0; i < maxLODLevels; i++)
                {
                    lodSettings[i] = new LODSettings
                    {
                        screenRelativeTransitionHeight = 1.0f / (i + 1),
                        simplificationRatio = Mathf.Pow(0.5f, i),
                        useBoundingBoxForLowestLOD = (i == maxLODLevels - 1),
                        enableCrossFade = true,
                        crossFadeTransitionWidth = 0.25f
                    };
                }
            }
        }
        
        /// <summary>
        /// 为网格生成LOD级别
        /// </summary>
        public MeshClusterData.LODLevel[] GenerateLODLevelsForMesh(Mesh mesh)
        {
            if (mesh == null)
                return null;
                
            // 检查缓存
            if (cacheLODMeshes && lodCache.ContainsKey(mesh))
            {
                return CreateLODLevelsFromCache(mesh, lodCache[mesh]);
            }
            
            // 生成LOD网格
            Mesh[] lodMeshes = new Mesh[maxLODLevels];
            
            for (int i = 0; i < maxLODLevels; i++)
            {
                if (i == 0)
                {
                    // LOD 0 使用原始网格
                    lodMeshes[i] = mesh;
                }
                else
                {
                    // 异步生成LOD
                    if (enableAsyncLODGeneration)
                    {
                        QueueLODGeneration(mesh, i, lodSettings[i].simplificationRatio, 
                            (generatedMesh) => lodMeshes[i] = generatedMesh);
                    }
                    else
                    {
                        lodMeshes[i] = MeshCombiner.GenerateLODMesh(mesh, lodSettings[i].simplificationRatio);
                    }
                }
            }
            
            // 缓存结果
            if (cacheLODMeshes)
            {
                lodCache[mesh] = lodMeshes;
                meshBoundsCache[mesh] = mesh.bounds;
            }
            
            return CreateLODLevelsFromCache(mesh, lodMeshes);
        }
        
        /// <summary>
        /// 从缓存创建LOD级别
        /// </summary>
        private MeshClusterData.LODLevel[] CreateLODLevelsFromCache(Mesh originalMesh, Mesh[] lodMeshes)
        {
            MeshClusterData.LODLevel[] levels = new MeshClusterData.LODLevel[maxLODLevels];
            
            for (int i = 0; i < maxLODLevels; i++)
            {
                levels[i] = new MeshClusterData.LODLevel
                {
                    screenRelativeHeight = lodSettings[i].screenRelativeTransitionHeight,
                    mesh = lodMeshes[i],
                    simplificationRatio = lodSettings[i].simplificationRatio,
                    vertexCount = lodMeshes[i] != null ? lodMeshes[i].vertexCount : 0
                };
            }
            
            return levels;
        }
        
        /// <summary>
        /// 计算集群的当前LOD级别
        /// </summary>
        public int CalculateClusterLODLevel(MeshClusterData cluster, Camera camera)
        {
            if (cluster == null || camera == null || cluster.lodLevels == null)
                return 0;
                
            // 计算屏幕空间相对大小
            float screenSize = CalculateScreenSize(cluster.worldBounds, camera);
            
            // 应用迟滞效果（防止LOD频繁切换）
            float adjustedScreenSize = useHysteresis ? 
                ApplyHysteresis(screenSize, cluster) : screenSize;
            
            // 选择合适的LOD级别
            for (int i = 0; i < cluster.lodLevels.Length; i++)
            {
                if (adjustedScreenSize >= cluster.lodLevels[i].screenRelativeHeight)
                {
                    return i;
                }
            }
            
            return cluster.lodLevels.Length - 1;
        }
        
        /// <summary>
        /// 计算包围盒在屏幕上的相对大小
        /// </summary>
        private float CalculateScreenSize(Bounds bounds, Camera camera)
        {
            // 计算包围盒在屏幕空间的大小
            Vector3 center = camera.WorldToViewportPoint(bounds.center);
            
            if (center.z < camera.nearClipPlane)
                return 0;
                
            // 计算包围盒的半径（近似）
            float radius = bounds.extents.magnitude;
            Vector3 screenExtents = camera.WorldToViewportPoint(bounds.center + Vector3.right * radius);
            float screenRadius = Vector3.Distance(center, screenExtents);
            
            return screenRadius;
        }
        
        /// <summary>
        /// 应用迟滞效果
        /// </summary>
        private float ApplyHysteresis(float currentSize, MeshClusterData cluster)
        {
            // 简单的迟滞算法：只有当变化超过阈值时才切换LOD
            // 实际项目中可以使用更复杂的算法
            return currentSize;
        }
        
        /// <summary>
        /// 队列LOD生成任务
        /// </summary>
        private void QueueLODGeneration(Mesh mesh, int lodLevel, float simplificationRatio, 
            System.Action<Mesh> onComplete)
        {
            var task = new LODGenerationTask
            {
                originalMesh = mesh,
                lodLevel = lodLevel,
                simplificationRatio = simplificationRatio,
                onComplete = onComplete,
                isCompleted = false
            };
            
            generationQueue.Enqueue(task);
        }
        
        /// <summary>
        /// 处理异步操作
        /// </summary>
        private void ProcessAsyncOperations()
        {
            // 移除已完成的操作
            activeOperations.RemoveAll(op => op.isCompleted);
            
            // 启动新的操作
            while (activeOperations.Count < maxAsyncOperations && generationQueue.Count > 0)
            {
                var task = generationQueue.Dequeue();
                activeOperations.Add(task);
                
                // 在实际项目中，这里应该使用Job System或异步任务
                // 这里简化处理
                task.result = MeshCombiner.GenerateLODMesh(task.originalMesh, task.simplificationRatio);
                task.isCompleted = true;
                
                if (task.onComplete != null)
                {
                    task.onComplete(task.result);
                }
            }
        }
        
        /// <summary>
        /// 更新所有集群的LOD
        /// </summary>
        private void UpdateAllClustersLOD()
        {
            var manager = MeshClusterManager.Instance;
            if (manager == null)
                return;
                
            // 获取所有可见集群并更新LOD
            // 实际实现中需要通过MeshClusterManager获取集群
        }
        
        /// <summary>
        /// 获取网格的LOD网格
        /// </summary>
        public Mesh GetLODMesh(Mesh originalMesh, int lodLevel)
        {
            if (originalMesh == null || lodLevel < 0 || lodLevel >= maxLODLevels)
                return originalMesh;
                
            if (cacheLODMeshes && lodCache.ContainsKey(originalMesh))
            {
                var lodMeshes = lodCache[originalMesh];
                if (lodLevel < lodMeshes.Length && lodMeshes[lodLevel] != null)
                {
                    return lodMeshes[lodLevel];
                }
            }
            
            return originalMesh;
        }
        
        /// <summary>
        /// 清理LOD缓存
        /// </summary>
        public void ClearLODCache()
        {
            lodCache.Clear();
            meshBoundsCache.Clear();
            
            // 清理生成队列
            generationQueue.Clear();
            activeOperations.Clear();
        }
        
        /// <summary>
        /// 获取LOD统计信息
        /// </summary>
        public LODStatistics GetStatistics()
        {
            return new LODStatistics
            {
                cacheSize = lodCache.Count,
                queuedTasks = generationQueue.Count,
                activeOperations = activeOperations.Count,
                totalCachedMeshes = CalculateTotalCachedMeshes()
            };
        }
        
        private int CalculateTotalCachedMeshes()
        {
            int total = 0;
            foreach (var kvp in lodCache)
            {
                total += kvp.Value.Length;
            }
            return total;
        }
        
        /// <summary>
        /// LOD统计信息
        /// </summary>
        public struct LODStatistics
        {
            public int cacheSize;
            public int queuedTasks;
            public int activeOperations;
            public int totalCachedMeshes;
        }
        
        #region 调试方法
        public void DrawLODVisualization()
        {
            if (mainCamera == null)
                return;
                
            // 绘制LOD距离可视化
            for (int i = 0; i < lodDistances.Length; i++)
            {
                float distance = lodDistances[i];
                Gizmos.color = GetLODColor(i);
                Gizmos.DrawWireSphere(mainCamera.transform.position, distance);
            }
        }
        
        private Color GetLODColor(int lodLevel)
        {
            switch (lodLevel)
            {
                case 0: return Color.green;
                case 1: return Color.yellow;
                case 2: return new Color(1f, 0.5f, 0f); // 橙色
                case 3: return Color.red;
                default: return Color.white;
            }
        }
        
        public void PrintLODStatistics()
        {
            var stats = GetStatistics();
            Debug.Log($"LOD Manager Statistics:");
            Debug.Log($"  Cache Size: {stats.cacheSize}");
            Debug.Log($"  Queued Tasks: {stats.queuedTasks}");
            Debug.Log($"  Active Operations: {stats.activeOperations}");
            Debug.Log($"  Total Cached Meshes: {stats.totalCachedMeshes}");
        }
        #endregion
    }
}