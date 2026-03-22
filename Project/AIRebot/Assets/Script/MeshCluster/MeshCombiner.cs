using System.Collections.Generic;
using UnityEngine;

namespace MeshClusterSystem
{
    /// <summary>
    /// 网格合并器 - 负责将多个网格合并为单个集群网格
    /// </summary>
    public static class MeshCombiner
    {
        /// <summary>
        /// 合并网格到集群
        /// </summary>
        public static bool CombineMeshesForCluster(MeshClusterData cluster)
        {
            if (cluster == null || cluster.instances.Count == 0)
                return false;
                
            // 按材质分组
            Dictionary<Material, List<MeshClusterData.MeshInstanceData>> materialGroups = 
                new Dictionary<Material, List<MeshClusterData.MeshInstanceData>>();
            
            foreach (var instance in cluster.instances)
            {
                if (instance.material == null)
                    continue;
                    
                if (!materialGroups.ContainsKey(instance.material))
                {
                    materialGroups[instance.material] = new List<MeshClusterData.MeshInstanceData>();
                }
                materialGroups[instance.material].Add(instance);
            }
            
            // 为每个材质组创建合并的网格
            List<Mesh> subMeshes = new List<Mesh>();
            List<Material> materials = new List<Material>();
            List<Matrix4x4[]> instanceMatricesPerMaterial = new List<Matrix4x4[]>();
            
            foreach (var kvp in materialGroups)
            {
                Material material = kvp.Key;
                List<MeshClusterData.MeshInstanceData> instances = kvp.Value;
                
                if (instances.Count == 0)
                    continue;
                    
                // 检查是否所有网格都相同（用于GPU实例化）
                bool canUseGPUInstancing = CanUseGPUInstancing(instances);
                
                if (canUseGPUInstancing && instances.Count > 1)
                {
                    // 使用GPU实例化
                    SetupGPUInstancing(cluster, instances, material);
                }
                else
                {
                    // 合并网格
                    Mesh combinedMesh = CombineInstanceMeshes(instances);
                    if (combinedMesh != null)
                    {
                        subMeshes.Add(combinedMesh);
                        materials.Add(material);
                    }
                }
            }
            
            // 如果有合并的网格，创建最终网格
            if (subMeshes.Count > 0)
            {
                cluster.combinedMesh = CombineSubMeshes(subMeshes, materials);
                cluster.materials = materials.ToArray();
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查是否可以使用GPU实例化
        /// </summary>
        private static bool CanUseGPUInstancing(List<MeshClusterData.MeshInstanceData> instances)
        {
            if (instances.Count < 2)
                return false;
                
            Mesh firstMesh = instances[0].mesh;
            Material firstMaterial = instances[0].material;
            
            foreach (var instance in instances)
            {
                if (instance.mesh != firstMesh || instance.material != firstMaterial)
                    return false;
                    
                // 检查网格属性是否相同
                if (instance.mesh.subMeshCount != firstMesh.subMeshCount)
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 设置GPU实例化
        /// </summary>
        private static void SetupGPUInstancing(MeshClusterData cluster, 
            List<MeshClusterData.MeshInstanceData> instances, Material material)
        {
            // 准备实例矩阵
            Matrix4x4[] matrices = new Matrix4x4[instances.Count];
            Vector4[] colors = new Vector4[instances.Count];
            
            for (int i = 0; i < instances.Count; i++)
            {
                matrices[i] = instances[i].localToWorld;
                colors[i] = Color.white; // 可以添加每个实例的颜色
            }
            
            // 存储到集群
            cluster.instanceMatrices = matrices;
            cluster.instanceColors = colors;
            cluster.combinedMesh = instances[0].mesh;
            cluster.materials = new Material[] { material };
            
            // 启用GPU实例化
            if (material.enableInstancing)
            {
                material.enableInstancing = true;
            }
        }
        
        /// <summary>
        /// 合并实例网格
        /// </summary>
        private static Mesh CombineInstanceMeshes(List<MeshClusterData.MeshInstanceData> instances)
        {
            if (instances.Count == 0)
                return null;
                
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            
            foreach (var instance in instances)
            {
                if (instance.mesh == null)
                    continue;
                    
                for (int subMeshIndex = 0; subMeshIndex < instance.mesh.subMeshCount; subMeshIndex++)
                {
                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = instance.mesh,
                        subMeshIndex = subMeshIndex,
                        transform = instance.localToWorld
                    };
                    combineInstances.Add(combineInstance);
                }
            }
            
            if (combineInstances.Count == 0)
                return null;
                
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
            combinedMesh.RecalculateBounds();
            combinedMesh.RecalculateNormals();
            combinedMesh.RecalculateTangents();
            
            return combinedMesh;
        }
        
        /// <summary>
        /// 合并子网格
        /// </summary>
        private static Mesh CombineSubMeshes(List<Mesh> subMeshes, List<Material> materials)
        {
            if (subMeshes.Count == 0)
                return null;
                
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            
            for (int i = 0; i < subMeshes.Count; i++)
            {
                CombineInstance combineInstance = new CombineInstance
                {
                    mesh = subMeshes[i],
                    subMeshIndex = 0,
                    transform = Matrix4x4.identity
                };
                combineInstances.Add(combineInstance);
            }
            
            Mesh finalMesh = new Mesh();
            finalMesh.CombineMeshes(combineInstances.ToArray(), false, false);
            finalMesh.RecalculateBounds();
            
            return finalMesh;
        }
        
        /// <summary>
        /// 为LOD生成简化网格
        /// </summary>
        public static Mesh GenerateLODMesh(Mesh originalMesh, float simplificationRatio)
        {
            if (originalMesh == null || simplificationRatio >= 1.0f)
                return originalMesh;
                
            // 这里可以使用Unity的Mesh Simplification或第三方库
            // 简化示例（实际项目中需要更复杂的算法）
            Mesh simplifiedMesh = Object.Instantiate(originalMesh);
            
            // 简单的顶点简化（实际项目应使用专业算法）
            Vector3[] vertices = simplifiedMesh.vertices;
            int[] triangles = simplifiedMesh.triangles;
            
            if (simplificationRatio < 0.3f)
            {
                // 重度简化
                simplifiedMesh = CreateBoundingBoxMesh(originalMesh.bounds);
            }
            else if (simplificationRatio < 0.6f)
            {
                // 中度简化 - 减少三角形数量
                int targetTriangleCount = Mathf.Max(100, (int)(triangles.Length * simplificationRatio / 3));
                // 实际项目中应使用网格简化算法
            }
            
            simplifiedMesh.RecalculateBounds();
            simplifiedMesh.RecalculateNormals();
            
            return simplifiedMesh;
        }
        
        /// <summary>
        /// 创建包围盒网格（最低LOD）
        /// </summary>
        private static Mesh CreateBoundingBoxMesh(Bounds bounds)
        {
            Mesh boxMesh = new Mesh();
            
            Vector3 size = bounds.size;
            Vector3 center = bounds.center;
            
            Vector3[] vertices = new Vector3[8];
            vertices[0] = center + new Vector3(-size.x / 2, -size.y / 2, -size.z / 2);
            vertices[1] = center + new Vector3(size.x / 2, -size.y / 2, -size.z / 2);
            vertices[2] = center + new Vector3(size.x / 2, size.y / 2, -size.z / 2);
            vertices[3] = center + new Vector3(-size.x / 2, size.y / 2, -size.z / 2);
            vertices[4] = center + new Vector3(-size.x / 2, -size.y / 2, size.z / 2);
            vertices[5] = center + new Vector3(size.x / 2, -size.y / 2, size.z / 2);
            vertices[6] = center + new Vector3(size.x / 2, size.y / 2, size.z / 2);
            vertices[7] = center + new Vector3(-size.x / 2, size.y / 2, size.z / 2);
            
            int[] triangles = new int[]
            {
                0, 2, 1, 0, 3, 2, // 前面
                1, 2, 6, 1, 6, 5, // 右面
                5, 6, 7, 5, 7, 4, // 后面
                4, 7, 3, 4, 3, 0, // 左面
                3, 7, 6, 3, 6, 2, // 上面
                4, 0, 1, 4, 1, 5  // 下面
            };
            
            boxMesh.vertices = vertices;
            boxMesh.triangles = triangles;
            boxMesh.RecalculateNormals();
            boxMesh.RecalculateBounds();
            
            return boxMesh;
        }
        
        /// <summary>
        /// 优化网格数据
        /// </summary>
        public static void OptimizeMesh(Mesh mesh)
        {
            if (mesh == null)
                return;
                
            // 合并重复顶点
            mesh.Optimize();
            
            // 重新索引
            mesh.RecalculateBounds();
            
            // 设置网格为可读写（如果需要）
            if (!mesh.isReadable)
            {
                // 注意：这需要网格在导入设置中启用"Read/Write"
            }
        }
        
        /// <summary>
        /// 计算网格的内存占用
        /// </summary>
        public static long CalculateMeshMemorySize(Mesh mesh)
        {
            if (mesh == null)
                return 0;
                
            long size = 0;
            
            // 顶点数据
            if (mesh.vertices != null)
                size += mesh.vertices.Length * 3 * sizeof(float);
                
            // 三角形数据
            if (mesh.triangles != null)
                size += mesh.triangles.Length * sizeof(int);
                
            // 法线
            if (mesh.normals != null)
                size += mesh.normals.Length * 3 * sizeof(float);
                
            // UV
            if (mesh.uv != null)
                size += mesh.uv.Length * 2 * sizeof(float);
                
            // 切线
            if (mesh.tangents != null)
                size += mesh.tangents.Length * 4 * sizeof(float);
                
            // 颜色
            if (mesh.colors != null)
                size += mesh.colors.Length * 4 * sizeof(float);
                
            return size;
        }
    }
}