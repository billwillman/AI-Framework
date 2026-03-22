using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshClusterSystem
{
    /// <summary>
    /// Mesh Cluster渲染器 - 负责与Unity渲染管线的集成
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class MeshClusterRenderer : MonoBehaviour
    {
        [Header("渲染设置")]
        [SerializeField] private RenderPassType renderPassType = RenderPassType.Forward;
        [SerializeField] private LayerMask cullingMask = ~0;
        [SerializeField] private bool enableDepthPrepass = true;
        [SerializeField] private bool enableMotionVectors = false;
        
        [Header("阴影设置")]
        [SerializeField] private ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
        [SerializeField] private bool receiveShadows = true;
        [SerializeField] private float shadowDistance = 100f;
        
        [Header("高级设置")]
        [SerializeField] private bool enableGPUDrivenRendering = false;
        [SerializeField] private bool enableIndirectRendering = false;
        [SerializeField] private int maxIndirectDrawCalls = 1000;
        
        // 渲染数据
        private CommandBuffer commandBuffer;
        private MaterialPropertyBlock propertyBlock;
        private List<Matrix4x4> batchMatrices = new List<Matrix4x4>();
        private List<Vector4> batchColors = new List<Vector4>();
        
        // 引用
        private MeshClusterManager clusterManager;
        private LODManager lodManager;
        private Camera targetCamera;
        
        // 统计
        private int renderedClusters;
        private int renderedInstances;
        private int gpuInstancingBatches;
        private int indirectDrawCalls;
        
        public enum RenderPassType
        {
            Forward,
            Deferred,
            UniversalRP,
            HDRP
        }
        
        private void Start()
        {
            targetCamera = GetComponent<Camera>();
            clusterManager = MeshClusterManager.Instance;
            lodManager = FindObjectOfType<LODManager>();
            
            InitializeCommandBuffer();
            InitializeMaterialPropertyBlock();
            
            // 注册到渲染管线
            RegisterToRenderPipeline();
        }
        
        private void OnDestroy()
        {
            CleanupCommandBuffer();
            UnregisterFromRenderPipeline();
        }
        
        private void InitializeCommandBuffer()
        {
            commandBuffer = new CommandBuffer
            {
                name = "MeshClusterRenderer"
            };
        }
        
        private void InitializeMaterialPropertyBlock()
        {
            propertyBlock = new MaterialPropertyBlock();
        }
        
        private void RegisterToRenderPipeline()
        {
            // 在URP/HDRP中注册渲染特性
            // 这里简化处理，实际项目中需要根据渲染管线类型进行注册
        }
        
        private void UnregisterFromRenderPipeline()
        {
            if (commandBuffer != null)
            {
                commandBuffer.Release();
                commandBuffer = null;
            }
        }
        
        private void OnPreRender()
        {
            if (clusterManager == null || targetCamera == null)
                return;
                
            // 准备渲染
            PrepareRendering();
        }
        
        private void OnPostRender()
        {
            // 清理临时数据
            CleanupFrameData();
        }
        
        /// <summary>
        /// 准备渲染
        /// </summary>
        private void PrepareRendering()
        {
            commandBuffer.Clear();
            renderedClusters = 0;
            renderedInstances = 0;
            gpuInstancingBatches = 0;
            indirectDrawCalls = 0;
            
            // 设置全局渲染状态
            commandBuffer.SetGlobalFloat("_ClusterRendering", 1.0f);
            
            // 执行视锥体裁剪
            PerformFrustumCulling();
            
            // 构建渲染批次
            BuildRenderBatches();
            
            // 执行渲染
            ExecuteRendering();
        }
        
        /// <summary>
        /// 执行视锥体裁剪
        /// </summary>
        private void PerformFrustumCulling()
        {
            // 视锥体裁剪已经在MeshClusterManager中处理
            // 这里可以添加额外的裁剪逻辑
        }
        
        /// <summary>
        /// 构建渲染批次
        /// </summary>
        private void BuildRenderBatches()
        {
            if (clusterManager == null)
                return;
                
            // 获取可见集群
            // 实际实现中需要通过MeshClusterManager获取可见集群列表
        }
        
        /// <summary>
        /// 执行渲染
        /// </summary>
        private void ExecuteRendering()
        {
            if (enableGPUDrivenRendering)
            {
                RenderGPUDriven();
            }
            else if (enableIndirectRendering)
            {
                RenderIndirect();
            }
            else
            {
                RenderTraditional();
            }
            
            // 执行命令缓冲区
            Graphics.ExecuteCommandBuffer(commandBuffer);
        }
        
        /// <summary>
        /// 传统渲染方式
        /// </summary>
        private void RenderTraditional()
        {
            // 遍历所有集群并渲染
            // 实际实现中需要获取集群数据
        }
        
        /// <summary>
        /// GPU驱动渲染
        /// </summary>
        private void RenderGPUDriven()
        {
            // GPU驱动渲染实现
            // 需要Compute Shader支持
        }
        
        /// <summary>
        /// 间接渲染
        /// </summary>
        private void RenderIndirect()
        {
            // 间接绘制调用
            // 使用Graphics.DrawMeshInstancedIndirect
        }
        
        /// <summary>
        /// 渲染单个集群
        /// </summary>
        private void RenderCluster(MeshClusterData cluster, int lodLevel)
        {
            if (cluster == null || cluster.combinedMesh == null || cluster.materials == null)
                return;
                
            // 获取LOD网格
            Mesh renderMesh = cluster.combinedMesh;
            if (lodManager != null && cluster.lodLevels != null && 
                lodLevel < cluster.lodLevels.Length)
            {
                renderMesh = cluster.lodLevels[lodLevel].mesh ?? cluster.combinedMesh;
            }
            
            // 设置材质属性
            propertyBlock.Clear();
            
            // 添加实例数据
            if (cluster.instanceColors != null && cluster.instanceColors.Length > 0)
            {
                propertyBlock.SetVectorArray("_InstanceColors", cluster.instanceColors);
            }
            
            // 渲染所有子网格
            for (int subMeshIndex = 0; subMeshIndex < renderMesh.subMeshCount; subMeshIndex++)
            {
                if (subMeshIndex >= cluster.materials.Length)
                    break;
                    
                Material material = cluster.materials[subMeshIndex];
                
                if (material == null)
                    continue;
                    
                // 使用GPU实例化
                if (cluster.instanceMatrices != null && cluster.instanceMatrices.Length > 0 && 
                    material.enableInstancing)
                {
                    RenderWithGPUInstancing(renderMesh, subMeshIndex, material, 
                        cluster.instanceMatrices, propertyBlock);
                    gpuInstancingBatches++;
                }
                else
                {
                    // 单个绘制调用
                    RenderSingle(renderMesh, subMeshIndex, material, propertyBlock);
                }
            }
            
            renderedClusters++;
            renderedInstances += cluster.instances.Count;
        }
        
        /// <summary>
        /// 使用GPU实例化渲染
        /// </summary>
        private void RenderWithGPUInstancing(Mesh mesh, int subMeshIndex, Material material, 
            Matrix4x4[] matrices, MaterialPropertyBlock propertyBlock)
        {
            if (matrices.Length > 0)
            {
                Graphics.DrawMeshInstanced(
                    mesh,
                    subMeshIndex,
                    material,
                    matrices,
                    matrices.Length,
                    propertyBlock,
                    shadowCastingMode,
                    receiveShadows,
                    0, // layer
                    targetCamera,
                    LightProbeUsage.BlendProbes,
                    null
                );
            }
        }
        
        /// <summary>
        /// 单个绘制调用渲染
        /// </summary>
        private void RenderSingle(Mesh mesh, int subMeshIndex, Material material, 
            MaterialPropertyBlock propertyBlock)
        {
            Graphics.DrawMesh(
                mesh,
                Matrix4x4.identity,
                material,
                0, // layer
                targetCamera,
                subMeshIndex,
                propertyBlock,
                shadowCastingMode,
                receiveShadows,
                null,
                false, // useLightProbes
                null
            );
        }
        
        /// <summary>
        /// 清理帧数据
        /// </summary>
        private void CleanupFrameData()
        {
            batchMatrices.Clear();
            batchColors.Clear();
        }
        
        /// <summary>
        /// 设置渲染目标
        /// </summary>
        public void SetRenderTarget(RenderTexture renderTexture)
        {
            if (commandBuffer != null && renderTexture != null)
            {
                commandBuffer.SetRenderTarget(renderTexture);
            }
        }
        
        /// <summary>
        /// 清除渲染目标
        /// </summary>
        public void ClearRenderTarget(bool clearColor = true, bool clearDepth = true)
        {
            if (commandBuffer != null)
            {
                commandBuffer.ClearRenderTarget(clearColor, clearDepth, Color.clear);
            }
        }
        
        /// <summary>
        /// 添加自定义渲染命令
        /// </summary>
        public void AddCustomCommand(System.Action<CommandBuffer> commandAction)
        {
            if (commandAction != null && commandBuffer != null)
            {
                commandAction(commandBuffer);
            }
        }
        
        #region 公共属性
        public int RenderedClusters => renderedClusters;
        public int RenderedInstances => renderedInstances;
        public int GPUInstancingBatches => gpuInstancingBatches;
        public int IndirectDrawCalls => indirectDrawCalls;
        public Camera TargetCamera => targetCamera;
        #endregion
        
        #region 调试方法
        public void DrawRenderBounds()
        {
            if (clusterManager == null)
                return;
                
            // 绘制渲染边界
            Gizmos.color = Color.cyan;
            
            // 实际实现中需要获取集群数据并绘制边界
        }
        
        public void PrintRenderingStatistics()
        {
            Debug.Log($"Mesh Cluster Rendering Statistics:");
            Debug.Log($"  Rendered Clusters: {renderedClusters}");
            Debug.Log($"  Rendered Instances: {renderedInstances}");
            Debug.Log($"  GPU Instancing Batches: {gpuInstancingBatches}");
            Debug.Log($"  Indirect Draw Calls: {indirectDrawCalls}");
            Debug.Log($"  Draw Calls Saved: {renderedInstances - renderedClusters}");
        }
        
        private void OnDrawGizmosSelected()
        {
            if (targetCamera != null)
            {
                // 绘制相机视锥体
                Gizmos.color = Color.yellow;
                Gizmos.matrix = targetCamera.transform.localToWorldMatrix;
                Gizmos.DrawFrustum(Vector3.zero, targetCamera.fieldOfView, 
                    targetCamera.farClipPlane, targetCamera.nearClipPlane, 
                    targetCamera.aspect);
            }
        }
        #endregion
    }
}