# Mesh Cluster 系统

## 概述

Mesh Cluster（网格集群）是一个高性能的网格渲染优化系统，用于将大量网格实例合并为集群，减少draw call，优化渲染性能。

## 核心特性

### 1. 网格集群化
- 将空间相邻的网格合并为集群
- 支持静态和动态集群
- 自动集群大小调整

### 2. LOD管理
- 多级LOD支持（0-3级）
- 动态LOD切换
- 异步LOD网格生成
- LOD缓存系统

### 3. 渲染优化
- GPU实例化支持
- 视锥体裁剪
- 遮挡剔除
- 间接渲染支持

### 4. 性能监控
- 实时性能统计
- 内存使用监控
- 调试可视化
- 性能报告导出

## 系统架构

```
MeshClusterSystem/
├── MeshClusterManager.cs      # 集群管理器（主控制器）
├── MeshClusterData.cs         # 数据结构定义
├── MeshCombiner.cs            # 网格合并器
├── LODManager.cs              # LOD管理器
├── MeshClusterRenderer.cs     # 渲染器
├── MeshClusterDebugger.cs     # 调试器
└── MeshClusterExample.cs      # 使用示例
```

## 快速开始

### 1. 基本使用

```csharp
// 获取或创建MeshClusterManager
MeshClusterManager manager = MeshClusterManager.Instance;

// 添加网格实例
string clusterId = manager.AddMeshInstance(
    mesh,          // Mesh对象
    material,      // Material对象
    position,      // 位置
    rotation,      // 旋转
    scale,         // 缩放
    isStatic: true // 是否为静态
);

// 更新实例变换
manager.UpdateMeshInstance(clusterId, instanceIndex, newPosition, newRotation, newScale);

// 移除实例
manager.RemoveMeshInstance(clusterId, instanceIndex);
```

### 2. 使用示例场景

```csharp
// 创建示例场景
MeshClusterExample example = FindObjectOfType<MeshClusterExample>();
if (example == null)
{
    GameObject exampleGO = new GameObject("MeshClusterExample");
    example = exampleGO.AddComponent<MeshClusterExample>();
}

// 配置示例
example.SetInstanceCount(100);
example.SetSpawnArea(new Vector3(100, 10, 100));
example.SetAutoSpawn(true);
```

### 3. 性能监控

```csharp
// 获取调试器
MeshClusterDebugger debugger = MeshClusterDebugger.GetOrCreateDebugger();

// 打印统计信息
debugger.PrintAllStatistics();

// 导出性能报告
string report = debugger.ExportPerformanceReport();
Debug.Log(report);
```

## 配置参数

### MeshClusterManager 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| maxClusters | 最大集群数量 | 100 |
| maxInstancesPerCluster | 每个集群最大实例数 | 100 |
| clusterRadius | 集群半径 | 50 |
| enableDynamicClustering | 启用动态集群 | true |
| updateInterval | 更新间隔 | 0.5 |

### LODManager 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| maxLODLevels | 最大LOD级别 | 4 |
| lodDistances | LOD距离阈值 | [10, 20, 50, 100] |
| enableAsyncLODGeneration | 异步LOD生成 | true |
| cacheLODMeshes | LOD网格缓存 | true |

### MeshClusterRenderer 配置

| 参数 | 说明 | 默认值 |
|------|------|--------|
| renderPassType | 渲染通道类型 | Forward |
| shadowCastingMode | 阴影投射模式 | On |
| enableGPUDrivenRendering | GPU驱动渲染 | false |
| enableIndirectRendering | 间接渲染 | false |

## 性能优化建议

### 1. 静态网格
- 标记不移动的网格为静态
- 使用静态批处理
- 预生成LOD网格

### 2. 动态网格
- 限制动态集群大小
- 使用GPU实例化
- 避免频繁更新

### 3. 内存优化
- 启用LOD缓存
- 合理设置集群大小
- 定期清理未使用的资源

### 4. 渲染优化
- 使用视锥体裁剪
- 启用遮挡剔除
- 使用合适的LOD级别

## 调试功能

### 1. 可视化调试
- 集群边界显示
- LOD级别可视化
- 性能统计显示

### 2. 控制台输出
```csharp
// 打印所有统计
MeshClusterDebugger.GetOrCreateDebugger().PrintAllStatistics();

// 打印管理器统计
MeshClusterManager.Instance.PrintStatistics();

// 打印渲染器统计
FindObjectOfType<MeshClusterRenderer>().PrintRenderingStatistics();
```

### 3. 性能分析
- 实时帧时间监控
- 内存使用统计
- Draw call优化统计
- 性能评分系统

## 高级功能

### 1. 自定义渲染
```csharp
// 获取渲染器
MeshClusterRenderer renderer = FindObjectOfType<MeshClusterRenderer>();

// 设置渲染目标
renderer.SetRenderTarget(customRenderTexture);

// 添加自定义渲染命令
renderer.AddCustomCommand((cmd) => {
    cmd.SetGlobalColor("_CustomColor", Color.red);
});
```

### 2. 异步操作
```csharp
// LOD管理器支持异步网格生成
LODManager lodManager = FindObjectOfType<LODManager>();

// 生成LOD级别
MeshClusterData.LODLevel[] lodLevels = lodManager.GenerateLODLevelsForMesh(mesh);
```

### 3. 扩展系统
系统设计为可扩展的，可以：
- 添加新的集群算法
- 集成第三方网格简化库
- 支持自定义渲染管线
- 添加新的调试工具

## 注意事项

### 1. 内存管理
- 大网格会占用较多内存
- LOD缓存会增加内存使用
- 定期清理未使用的资源

### 2. 性能考虑
- 动态集群更新有性能开销
- 过多的集群会增加CPU负担
- 复杂的LOD计算可能影响性能

### 3. 兼容性
- 需要Unity 2019.4或更高版本
- 某些功能需要特定渲染管线
- GPU实例化需要硬件支持

## 故障排除

### 1. 网格不显示
- 检查网格和材质是否有效
- 确认实例是否添加到集群
- 检查相机裁剪设置

### 2. 性能问题
- 减少动态实例数量
- 增大集群半径
- 降低LOD级别数量

### 3. 内存泄漏
- 定期清理LOD缓存
- 移除不再使用的实例
- 监控内存使用情况

## 版本历史

### v1.0.0 (2024-01-01)
- 初始版本
- 基础集群系统
- LOD管理
- 基本渲染优化

### v1.1.0 (计划)
- GPU驱动渲染
- 更高级的LOD算法
- 物理系统集成
- 编辑器工具

## 技术支持

如有问题，请：
1. 检查控制台错误信息
2. 启用调试模式
3. 查看性能统计
4. 联系开发团队

## 许可证

MIT License - 详见LICENSE文件