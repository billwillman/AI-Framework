using System.Collections.Generic;
using UnityEngine;

namespace MeshClusterSystem
{
    /// <summary>
    /// Mesh Cluster调试器 - 提供性能监控和调试功能
    /// </summary>
    public class MeshClusterDebugger : MonoBehaviour
    {
        [Header("调试显示")]
        [SerializeField] private bool showStatistics = true;
        [SerializeField] private bool showClusterBounds = true;
        [SerializeField] private bool showLODVisualization = true;
        [SerializeField] private bool showPerformanceMetrics = true;
        
        [Header("统计设置")]
        [SerializeField] private float updateInterval = 1.0f;
        [SerializeField] private int maxHistorySize = 100;
        [SerializeField] private bool logToConsole = false;
        
        [Header("颜色设置")]
        [SerializeField] private Color clusterBoundsColor = new Color(0, 1, 0, 0.3f);
        [SerializeField] private Color visibleClusterColor = new Color(1, 0, 0, 0.5f);
        [SerializeField] private Color[] lodColors = new Color[]
        {
            Color.green,
            Color.yellow,
            new Color(1f, 0.5f, 0f), // 橙色
            Color.red
        };
        
        // 性能数据
        private PerformanceMetrics currentMetrics;
        private Queue<PerformanceMetrics> metricsHistory = new Queue<PerformanceMetrics>();
        
        // 引用
        private MeshClusterManager clusterManager;
        private LODManager lodManager;
        private MeshClusterRenderer clusterRenderer;
        
        // GUI样式
        private GUIStyle labelStyle;
        private GUIStyle boxStyle;
        private GUIStyle headerStyle;
        
        private float lastUpdateTime;
        
        /// <summary>
        /// 性能指标结构
        /// </summary>
        [System.Serializable]
        public struct PerformanceMetrics
        {
            public float timestamp;
            public int totalInstances;
            public int visibleInstances;
            public int clusterCount;
            public int visibleClusterCount;
            public int drawCallsSaved;
            public float frameTime;
            public float memoryUsageMB;
            public int gpuInstancingBatches;
            public int indirectDrawCalls;
            
            // LOD统计
            public int[] lodLevelCounts;
            public float averageLODLevel;
            
            // 性能评分
            public float performanceScore;
        }
        
        private void Start()
        {
            clusterManager = MeshClusterManager.Instance;
            lodManager = FindObjectOfType<LODManager>();
            clusterRenderer = FindObjectOfType<MeshClusterRenderer>();
            
            InitializeGUIStyles();
        }
        
        private void Update()
        {
            if (Time.time - lastUpdateTime < updateInterval)
                return;
                
            lastUpdateTime = Time.time;
            
            // 收集性能数据
            CollectPerformanceMetrics();
            
            // 记录历史
            RecordMetricsHistory();
            
            // 输出到控制台
            if (logToConsole)
            {
                LogMetricsToConsole();
            }
        }
        
        private void OnGUI()
        {
            if (!showStatistics)
                return;
                
            InitializeGUIStyles();
            
            // 绘制调试信息
            DrawDebugGUI();
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
                
            // 绘制集群边界
            if (showClusterBounds && clusterManager != null)
            {
                DrawClusterBounds();
            }
            
            // 绘制LOD可视化
            if (showLODVisualization && lodManager != null)
            {
                DrawLODVisualization();
            }
        }
        
        private void InitializeGUIStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    normal = { textColor = Color.white }
                };
                
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = MakeTex(2, 2, new Color(0, 0, 0, 0.7f)) }
                };
                
                headerStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.yellow }
                };
            }
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        /// <summary>
        /// 收集性能指标
        /// </summary>
        private void CollectPerformanceMetrics()
        {
            currentMetrics = new PerformanceMetrics
            {
                timestamp = Time.time
            };
            
            if (clusterManager != null)
            {
                currentMetrics.totalInstances = clusterManager.TotalInstances;
                currentMetrics.visibleInstances = clusterManager.VisibleInstances;
                currentMetrics.clusterCount = clusterManager.ClusterCount;
                currentMetrics.visibleClusterCount = clusterManager.VisibleClusterCount;
                currentMetrics.drawCallsSaved = clusterManager.DrawCallsSaved;
            }
            
            if (clusterRenderer != null)
            {
                currentMetrics.gpuInstancingBatches = clusterRenderer.GPUInstancingBatches;
                currentMetrics.indirectDrawCalls = clusterRenderer.IndirectDrawCalls;
            }
            
            // 计算帧时间
            currentMetrics.frameTime = Time.deltaTime * 1000f; // 转换为毫秒
            
            // 计算内存使用
            currentMetrics.memoryUsageMB = CalculateMemoryUsage();
            
            // 计算LOD统计
            CalculateLODStatistics();
            
            // 计算性能评分
            currentMetrics.performanceScore = CalculatePerformanceScore();
        }
        
        /// <summary>
        /// 计算内存使用
        /// </summary>
        private float CalculateMemoryUsage()
        {
            // 简化计算，实际项目中可以使用Profiler
            float memoryMB = 0;
            
            // 估算网格内存
            if (clusterManager != null)
            {
                // 这里简化处理，实际需要遍历所有网格
                memoryMB += clusterManager.TotalInstances * 0.1f; // 假设每个实例0.1MB
            }
            
            return memoryMB;
        }
        
        /// <summary>
        /// 计算LOD统计
        /// </summary>
        private void CalculateLODStatistics()
        {
            // 这里简化处理，实际需要统计每个实例的LOD级别
            currentMetrics.lodLevelCounts = new int[4];
            currentMetrics.averageLODLevel = 1.5f; // 示例值
        }
        
        /// <summary>
        /// 计算性能评分
        /// </summary>
        private float CalculatePerformanceScore()
        {
            float score = 100f;
            
            // 基于帧时间扣分
            if (currentMetrics.frameTime > 33f) // > 30FPS
                score -= 20f;
            else if (currentMetrics.frameTime > 16f) // > 60FPS
                score -= 10f;
                
            // 基于内存使用扣分
            if (currentMetrics.memoryUsageMB > 500f)
                score -= 20f;
            else if (currentMetrics.memoryUsageMB > 200f)
                score -= 10f;
                
            // 基于draw call优化加分
            if (currentMetrics.drawCallsSaved > 100)
                score += 10f;
            else if (currentMetrics.drawCallsSaved > 50)
                score += 5f;
                
            return Mathf.Clamp(score, 0, 100);
        }
        
        /// <summary>
        /// 记录指标历史
        /// </summary>
        private void RecordMetricsHistory()
        {
            metricsHistory.Enqueue(currentMetrics);
            if (metricsHistory.Count > maxHistorySize)
            {
                metricsHistory.Dequeue();
            }
        }
        
        /// <summary>
        /// 输出指标到控制台
        /// </summary>
        private void LogMetricsToConsole()
        {
            Debug.Log($"Mesh Cluster Performance:");
            Debug.Log($"  Instances: {currentMetrics.totalInstances} (Visible: {currentMetrics.visibleInstances})");
            Debug.Log($"  Clusters: {currentMetrics.clusterCount} (Visible: {currentMetrics.visibleClusterCount})");
            Debug.Log($"  Draw Calls Saved: {currentMetrics.drawCallsSaved}");
            Debug.Log($"  GPU Instancing Batches: {currentMetrics.gpuInstancingBatches}");
            Debug.Log($"  Frame Time: {currentMetrics.frameTime:F2}ms");
            Debug.Log($"  Memory Usage: {currentMetrics.memoryUsageMB:F2}MB");
            Debug.Log($"  Performance Score: {currentMetrics.performanceScore:F1}/100");
        }
        
        /// <summary>
        /// 绘制调试GUI
        /// </summary>
        private void DrawDebugGUI()
        {
            // 开始GUI区域
            GUILayout.BeginArea(new Rect(10, 10, 300, 400), boxStyle);
            
            // 标题
            GUILayout.Label("Mesh Cluster Debugger", headerStyle);
            GUILayout.Space(10);
            
            // 基本信息
            GUILayout.Label($"Instances: {currentMetrics.totalInstances}", labelStyle);
            GUILayout.Label($"Visible: {currentMetrics.visibleInstances}", labelStyle);
            GUILayout.Label($"Clusters: {currentMetrics.clusterCount}", labelStyle);
            GUILayout.Label($"Visible Clusters: {currentMetrics.visibleClusterCount}", labelStyle);
            GUILayout.Space(5);
            
            // 性能指标
            GUILayout.Label($"Draw Calls Saved: {currentMetrics.drawCallsSaved}", labelStyle);
            GUILayout.Label($"GPU Instancing: {currentMetrics.gpuInstancingBatches}", labelStyle);
            GUILayout.Label($"Indirect Draw: {currentMetrics.indirectDrawCalls}", labelStyle);
            GUILayout.Space(5);
            
            // 帧时间和内存
            GUILayout.Label($"Frame Time: {currentMetrics.frameTime:F2}ms", labelStyle);
            GUILayout.Label($"Memory: {currentMetrics.memoryUsageMB:F2}MB", labelStyle);
            GUILayout.Space(5);
            
            // 性能评分
            Color scoreColor = GetScoreColor(currentMetrics.performanceScore);
            GUI.color = scoreColor;
            GUILayout.Label($"Performance: {currentMetrics.performanceScore:F1}/100", labelStyle);
            GUI.color = Color.white;
            GUILayout.Space(5);
            
            // 控制按钮
            if (GUILayout.Button("Print Statistics"))
            {
                PrintAllStatistics();
            }
            
            if (GUILayout.Button("Toggle Bounds"))
            {
                showClusterBounds = !showClusterBounds;
            }
            
            if (GUILayout.Button("Toggle LOD Visualization"))
            {
                showLODVisualization = !showLODVisualization;
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// 绘制集群边界
        /// </summary>
        private void DrawClusterBounds()
        {
            // 实际实现中需要获取集群数据并绘制边界
            // 这里简化处理
            if (clusterManager != null)
            {
                // 调用集群管理器的绘制方法
                // clusterManager.DrawClusterBounds();
            }
        }
        
        /// <summary>
        /// 绘制LOD可视化
        /// </summary>
        private void DrawLODVisualization()
        {
            if (lodManager != null)
            {
                // 调用LOD管理器的绘制方法
                // lodManager.DrawLODVisualization();
            }
        }
        
        /// <summary>
        /// 根据评分获取颜色
        /// </summary>
        private Color GetScoreColor(float score)
        {
            if (score >= 80) return Color.green;
            if (score >= 60) return Color.yellow;
            if (score >= 40) return new Color(1f, 0.5f, 0f); // 橙色
            return Color.red;
        }
        
        /// <summary>
        /// 打印所有统计信息
        /// </summary>
        public void PrintAllStatistics()
        {
            if (clusterManager != null)
            {
                clusterManager.PrintStatistics();
            }
            
            if (lodManager != null)
            {
                lodManager.PrintLODStatistics();
            }
            
            if (clusterRenderer != null)
            {
                clusterRenderer.PrintRenderingStatistics();
            }
            
            LogMetricsToConsole();
        }
        
        /// <summary>
        /// 获取性能历史数据
        /// </summary>
        public PerformanceMetrics[] GetPerformanceHistory()
        {
            return metricsHistory.ToArray();
        }
        
        /// <summary>
        /// 获取当前性能指标
        /// </summary>
        public PerformanceMetrics GetCurrentMetrics()
        {
            return currentMetrics;
        }
        
        /// <summary>
        /// 导出性能报告
        /// </summary>
        public string ExportPerformanceReport()
        {
            System.Text.StringBuilder report = new System.Text.StringBuilder();
            
            report.AppendLine("Mesh Cluster Performance Report");
            report.AppendLine("===============================");
            report.AppendLine($"Timestamp: {System.DateTime.Now}");
            report.AppendLine();
            
            report.AppendLine("Current Metrics:");
            report.AppendLine($"  Total Instances: {currentMetrics.totalInstances}");
            report.AppendLine($"  Visible Instances: {currentMetrics.visibleInstances}");
            report.AppendLine($"  Cluster Count: {currentMetrics.clusterCount}");
            report.AppendLine($"  Draw Calls Saved: {currentMetrics.drawCallsSaved}");
            report.AppendLine($"  Frame Time: {currentMetrics.frameTime:F2}ms");
            report.AppendLine($"  Memory Usage: {currentMetrics.memoryUsageMB:F2}MB");
            report.AppendLine($"  Performance Score: {currentMetrics.performanceScore:F1}/100");
            
            return report.ToString();
        }
        
        #region 静态方法
        /// <summary>
        /// 创建调试器实例
        /// </summary>
        public static MeshClusterDebugger CreateDebugger()
        {
            GameObject debuggerGO = new GameObject("MeshClusterDebugger");
            return debuggerGO.AddComponent<MeshClusterDebugger>();
        }
        
        /// <summary>
        /// 获取或创建调试器
        /// </summary>
        public static MeshClusterDebugger GetOrCreateDebugger()
        {
            MeshClusterDebugger debugger = FindObjectOfType<MeshClusterDebugger>();
            if (debugger == null)
            {
                debugger = CreateDebugger();
            }
            return debugger;
        }
        #endregion
    }
}