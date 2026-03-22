using UnityEngine;

namespace MeshClusterSystem
{
    /// <summary>
    /// Mesh Cluster使用示例
    /// </summary>
    public class MeshClusterExample : MonoBehaviour
    {
        [Header("示例设置")]
        [SerializeField] private int instanceCount = 100;
        [SerializeField] private float spawnRadius = 50f;
        [SerializeField] private Vector3 spawnArea = new Vector3(100, 10, 100);
        [SerializeField] private bool autoSpawn = true;
        [SerializeField] private float spawnInterval = 0.1f;
        
        [Header("网格设置")]
        [SerializeField] private Mesh[] meshes;
        [SerializeField] private Material[] materials;
        [SerializeField] private bool randomRotation = true;
        [SerializeField] private bool randomScale = true;
        [SerializeField] private Vector2 scaleRange = new Vector2(0.5f, 2f);
        
        [Header("测试功能")]
        [SerializeField] private bool testDynamicUpdates = false;
        [SerializeField] private float updateSpeed = 1f;
        [SerializeField] private float moveRadius = 10f;
        
        private MeshClusterManager clusterManager;
        private string[] instanceIds;
        private Vector3[] originalPositions;
        private float spawnTimer;
        private int spawnedCount;
        
        private void Start()
        {
            clusterManager = MeshClusterManager.Instance;
            
            if (clusterManager == null)
            {
                Debug.LogError("MeshClusterManager not found! Creating one...");
                GameObject managerGO = new GameObject("MeshClusterManager");
                clusterManager = managerGO.AddComponent<MeshClusterManager>();
            }
            
            if (autoSpawn)
            {
                instanceIds = new string[instanceCount];
                originalPositions = new Vector3[instanceCount];
                SpawnAllInstances();
            }
        }
        
        private void Update()
        {
            if (autoSpawn && spawnedCount < instanceCount)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnInterval)
                {
                    spawnTimer = 0;
                    SpawnInstance();
                }
            }
            
            if (testDynamicUpdates && instanceIds != null)
            {
                UpdateDynamicInstances();
            }
        }
        
        /// <summary>
        /// 生成所有实例
        /// </summary>
        private void SpawnAllInstances()
        {
            for (int i = 0; i < instanceCount; i++)
            {
                SpawnInstance();
            }
        }
        
        /// <summary>
        /// 生成单个实例
        /// </summary>
        private void SpawnInstance()
        {
            if (spawnedCount >= instanceCount)
                return;
                
            // 随机位置
            Vector3 position = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );
            
            // 随机旋转
            Quaternion rotation = randomRotation ? 
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)) : 
                Quaternion.identity;
                
            // 随机缩放
            Vector3 scale = randomScale ? 
                Vector3.one * Random.Range(scaleRange.x, scaleRange.y) : 
                Vector3.one;
                
            // 随机选择网格和材质
            Mesh mesh = meshes.Length > 0 ? meshes[Random.Range(0, meshes.Length)] : null;
            Material material = materials.Length > 0 ? materials[Random.Range(0, materials.Length)] : null;
            
            if (mesh == null)
            {
                // 创建默认立方体
                mesh = CreateCubeMesh();
            }
            
            if (material == null)
            {
                // 创建默认材质
                material = CreateDefaultMaterial();
            }
            
            // 添加到集群系统
            string instanceId = clusterManager.AddMeshInstance(mesh, material, position, rotation, scale);
            
            if (!string.IsNullOrEmpty(instanceId))
            {
                instanceIds[spawnedCount] = instanceId;
                originalPositions[spawnedCount] = position;
                spawnedCount++;
                
                Debug.Log($"Spawned instance {spawnedCount}/{instanceCount} at {position}");
            }
        }
        
        /// <summary>
        /// 更新动态实例
        /// </summary>
        private void UpdateDynamicInstances()
        {
            for (int i = 0; i < spawnedCount; i++)
            {
                if (string.IsNullOrEmpty(instanceIds[i]))
                    continue;
                    
                // 计算新位置（简单圆周运动）
                float time = Time.time * updateSpeed + i * 0.1f;
                Vector3 offset = new Vector3(
                    Mathf.Sin(time) * moveRadius,
                    0,
                    Mathf.Cos(time) * moveRadius
                );
                
                Vector3 newPosition = originalPositions[i] + offset;
                Quaternion newRotation = Quaternion.Euler(0, time * 50, 0);
                
                // 更新实例
                // 注意：这里需要知道实例在集群中的索引
                // 实际实现中需要更复杂的管理
            }
        }
        
        /// <summary>
        /// 创建默认立方体网格
        /// </summary>
        private Mesh CreateCubeMesh()
        {
            Mesh mesh = new Mesh();
            
            Vector3[] vertices = new Vector3[8]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f)
            };
            
            int[] triangles = new int[36]
            {
                0, 2, 1, 0, 3, 2, // 前面
                1, 2, 6, 1, 6, 5, // 右面
                5, 6, 7, 5, 7, 4, // 后面
                4, 7, 3, 4, 3, 0, // 左面
                3, 7, 6, 3, 6, 2, // 上面
                4, 0, 1, 4, 1, 5  // 下面
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// 创建默认材质
        /// </summary>
        private Material CreateDefaultMaterial()
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(
                Random.Range(0.2f, 1f),
                Random.Range(0.2f, 1f),
                Random.Range(0.2f, 1f)
            );
            material.enableInstancing = true;
            
            return material;
        }
        
        /// <summary>
        /// 清理所有实例
        /// </summary>
        public void ClearAllInstances()
        {
            if (clusterManager == null || instanceIds == null)
                return;
                
            // 实际实现中需要遍历所有实例并移除
            // 这里简化处理
            Debug.Log("Clearing all instances...");
            
            spawnedCount = 0;
            instanceIds = new string[instanceCount];
            originalPositions = new Vector3[instanceCount];
        }
        
        /// <summary>
        /// 重新生成实例
        /// </summary>
        public void RegenerateInstances()
        {
            ClearAllInstances();
            SpawnAllInstances();
        }
        
        /// <summary>
        /// 打印性能统计
        /// </summary>
        public void PrintPerformanceStats()
        {
            if (clusterManager != null)
            {
                clusterManager.PrintStatistics();
            }
            
            // 创建调试器并打印报告
            var debugger = MeshClusterDebugger.GetOrCreateDebugger();
            debugger.PrintAllStatistics();
        }
        
        #region 编辑器方法
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // 绘制生成区域
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireCube(transform.position, spawnArea);
            
            // 绘制生成半径
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
        #endif
        #endregion
        
        #region 公共方法
        /// <summary>
        /// 设置实例数量
        /// </summary>
        public void SetInstanceCount(int count)
        {
            instanceCount = Mathf.Max(1, count);
            
            if (autoSpawn)
            {
                ClearAllInstances();
                instanceIds = new string[instanceCount];
                originalPositions = new Vector3[instanceCount];
            }
        }
        
        /// <summary>
        /// 设置生成区域
        /// </summary>
        public void SetSpawnArea(Vector3 area)
        {
            spawnArea = area;
        }
        
        /// <summary>
        /// 启用/禁用自动生成
        /// </summary>
        public void SetAutoSpawn(bool enabled)
        {
            autoSpawn = enabled;
        }
        #endregion
    }
}