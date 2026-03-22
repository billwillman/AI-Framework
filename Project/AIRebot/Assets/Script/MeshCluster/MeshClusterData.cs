using System.Collections.Generic;
using UnityEngine;

namespace MeshClusterSystem
{
    /// <summary>
    /// Mesh Cluster的数据结构
    /// </summary>
    [System.Serializable]
    public class MeshClusterData
    {
        public string clusterId;
        public Bounds worldBounds;
        public List<MeshInstanceData> instances = new List<MeshInstanceData>();
        public Mesh combinedMesh;
        public Material[] materials;
        public LODLevel[] lodLevels;
        public bool isStatic = true;
        public float lastUpdateTime;
        
        // 渲染状态
        public Matrix4x4[] instanceMatrices;
        public Vector4[] instanceColors;
        
        /// <summary>
        /// Mesh实例数据
        /// </summary>
        [System.Serializable]
        public class MeshInstanceData
        {
            public Mesh mesh;
            public Material material;
            public Matrix4x4 localToWorld;
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public int lodLevel = 0;
            public bool isVisible = true;
        }
        
        /// <summary>
        /// LOD级别定义
        /// </summary>
        [System.Serializable]
        public class LODLevel
        {
            public float screenRelativeHeight = 0.5f;
            public Mesh mesh;
            public float simplificationRatio = 1.0f;
            public int vertexCount;
        }
    }
}