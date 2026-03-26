# Taco 系统整体概述

## 系统简介

Taco 是一个 Unity 游戏开发框架，包含两个核心系统：**Timeline** 和 **TreeDesigner**。这两个系统协同工作，为游戏开发提供强大的时序控制和行为管理能力。

## 系统关系图

```
┌─────────────────────────────────────────────────────────────┐
│                    Taco 框架生态系统                         │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────┐       数据流        ┌──────────────┐     │
│  │              │◄───────────────────►│              │     │
│  │  Timeline    │                     │ TreeDesigner │     │
│  │  系统        │     控制流          │  系统        │     │
│  │              │◄───────────────────►│              │     │
│  └──────────────┘                     └──────────────┘     │
│         │                           │                      │
│         │ 集成点                    │ 集成点               │
│         ▼                           ▼                      │
│  ┌──────────────┐             ┌──────────────┐             │
│  │  游戏对象    │             │    AI行为    │             │
│  │  动画/音频   │             │   状态机     │             │
│  │  粒子效果    │             │   决策树     │             │
│  │  相机控制    │             │   流程控制   │             │
│  └──────────────┘             └──────────────┘             │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Timeline 系统概述

### 核心功能
1. **时序控制**：精确控制动画、音频、特效的时间线
2. **多轨道编辑**：支持动画、音频、粒子、GameObject、Cinemachine等多种轨道
3. **可视化编辑**：完整的编辑器界面，支持拖放和关键帧编辑
4. **运行时播放**：在游戏运行时动态控制时间轴
5. **行为树集成**：通过 TreeTrack 在时间轴上执行行为树

### 适用场景
- 过场动画和剧情演出
- 技能序列和连招系统
- UI动画和转场效果
- 环境事件和交互序列
- 相机切换和镜头控制

## TreeDesigner 系统概述

### 核心功能
1. **可视化行为树**：节点式编辑器，直观设计AI行为
2. **丰富节点类型**：包含根节点、复合节点、装饰节点、动作节点等
3. **运行时执行**：高效的行为树执行引擎
4. **调试工具**：实时调试和状态监控
5. **模块化设计**：支持子树复用和自定义节点
6. **Timeline集成**：通过 TimelineNode 在行为树中触发时间轴事件

### 适用场景
- 游戏AI和敌人行为
- 角色状态机和决策逻辑
- 任务系统和流程控制
- 交互逻辑和条件判断
- 复杂的行为序列

## 系统集成方式

### 1. Timeline 调用 TreeDesigner
通过 **TreeTrack** 轨道在时间轴的特定时间段执行行为树：

```csharp
// 在时间轴上添加行为树轨道
var treeTrack = timeline.AddTrack<TreeTrack>();
treeTrack.TreeAsset = patrolTree;  // 行为树资源
treeTrack.StartTime = 0f;          // 开始时间
treeTrack.EndTime = 5f;            // 结束时间
```

**应用场景**：
- 过场动画中的角色AI行为
- 技能释放期间的决策逻辑
- 场景事件触发的行为变化

### 2. TreeDesigner 调用 Timeline
通过 **TimelineNode** 在行为树中触发时间轴事件：

```csharp
// 在行为树中添加时间轴节点
var timelineNode = new TimelineNode();
timelineNode.Timeline = cutsceneTimeline;  // 时间轴资源
timelineNode.EventTime = 2.5f;             // 触发时间
timelineNode.EventName = "ExplosionEvent"; // 事件名称
```

**应用场景**：
- AI决策触发特定的动画序列
- 状态转换时播放过渡效果
- 条件满足时触发剧情事件

### 3. 数据共享机制
两个系统通过以下方式共享数据：

#### 黑板数据 (Blackboard)
```csharp
// TreeDesigner 设置数据
treeRunner.SetValue("TargetPosition", enemyPosition);

// Timeline 读取数据
var targetPos = timeline.GetValue<Vector3>("TargetPosition");
```

#### 暴露属性 (Exposed Properties)
```csharp
// 在 TreeDesigner 中定义暴露属性
var healthProperty = new ExposedProperty();
healthProperty.PropertyName = "Health";
tree.ExposedProperties.Add(healthProperty);

// 在 Timeline 中访问
var health = timeline.GetExposedProperty<float>("Health");
```

## 典型工作流程

### 场景1：过场动画中的AI行为
```
┌─────────────────────────────────────────────────────┐
│                  工作流程                            │
├─────────────────────────────────────────────────────┤
│ 1. 使用 Timeline 设计过场动画序列                   │
│ 2. 在关键时间点添加 TreeTrack                      │
│ 3. 使用 TreeDesigner 设计角色AI行为树               │
│ 4. 将行为树关联到 TreeTrack                        │
│ 5. 运行时：时间轴播放时触发行为树执行               │
│ 6. 行为树执行结果反馈给时间轴（如触发事件）         │
└─────────────────────────────────────────────────────┘
```

### 场景2：技能系统中的决策逻辑
```
┌─────────────────────────────────────────────────────┐
│                  工作流程                            │
├─────────────────────────────────────────────────────┤
│ 1. 使用 TreeDesigner 设计技能决策树                 │
│ 2. 根据条件选择不同的技能序列                       │
│ 3. 通过 TimelineNode 触发对应的技能时间轴           │
│ 4. Timeline 播放技能动画、特效、音效               │
│ 5. 时间轴事件触发行为树状态更新                    │
│ 6. 形成闭环的决策-执行-反馈循环                    │
└─────────────────────────────────────────────────────┘
```

## 技术架构特点

### 1. 松耦合设计
- 两个系统独立运行，通过接口通信
- 可以单独使用或组合使用
- 易于扩展和替换组件

### 2. 数据驱动
- 所有配置数据可序列化
- 支持运行时动态修改
- 便于版本控制和团队协作

### 3. 高性能
- 优化的执行引擎
- 最小化GC分配
- 支持大量并发实例

### 4. 易用性
- 完整的可视化编辑器
- 直观的API设计
- 丰富的示例和文档

### 5. 可扩展性
- 插件式架构
- 自定义节点/轨道支持
- 编辑器工具扩展

## 文件结构总览

```
AIRebot/Assets/TimelineSkill/Taco/
├── Timeline/                          # Timeline 系统
│   ├── Scripts/
│   │   ├── Timeline.cs                # 主时间轴类
│   │   ├── TimelinePlayer.cs          # 时间轴播放器
│   │   ├── Track.cs                   # 轨道基类
│   │   ├── Timeline.Animation.cs      # 动画轨道
│   │   ├── Timeline.Audio.cs          # 音频轨道
│   │   ├── Timeline.GameObject.cs     # GameObject轨道
│   │   ├── Timeline.ParticleSystem.cs # 粒子系统轨道
│   │   ├── Timeline.TimeControl.cs    # 时间控制
│   │   ├── TimelineUtility.cs         # 实用工具
│   │   ├── Tree/                      # 行为树集成
│   │   │   ├── Timeline.Tree.cs       # 行为树轨道
│   │   │   ├── Timeline.Node.cs       # 时间轴节点
│   │   │   └── TimelineRunningTree.cs # 运行时行为树
│   │   └── Cinemachine/               # 相机控制
│   │       └── Timeline.Cinemachine.cs
│   └── Editor/                        # 编辑器相关
│       └── Scripts/
│           ├── TimelineEditorWindow.cs
│           └── TimelineTrackView.cs
└── TreeDesigner/                      # TreeDesigner 系统
    └── Scripts/
        ├── Tree/                      # 树相关
        │   ├── BaseTree.cs            # 树基类
        │   ├── RunnableTree.cs        # 可运行树
        │   ├── OneRootTree.cs         # 单根节点树
        │   └── SubTree.cs             # 子树
        ├── Node/                      # 节点系统
        │   ├── BaseNode.cs            # 节点基类
        │   ├── RunnableNode.cs        # 可运行节点
        │   ├── Root/                  # 根节点类型
        │   ├── Composite/             # 复合节点类型
        │   ├── Decorator/             # 装饰节点类型
        │   ├── Action/                # 动作节点类型
        │   ├── Value/                 # 值节点类型
        │   ├── Trigger/               # 触发器节点类型
        │   └── Custom/                # 自定义节点类型
        ├── PropertyPort/              # 端口系统
        │   └── PropertyPort.cs
        ├── Edge/                      # 边系统
        │   └── BaseEdge.cs
        ├── ExposedProperty/           # 暴露属性
        │   └── ExposedProperty.cs
        ├── TreeRunner.cs              # 树运行器
        ├── Debugger.cs                # 调试器
        └── Enum.cs                    # 枚举定义
```

## 学习路径建议

### 初级阶段
1. **Timeline 基础**：
   - 学习创建简单的时间轴
   - 掌握动画和音频轨道
   - 理解时间轴播放控制

2. **TreeDesigner 基础**：
   - 学习创建简单的行为树
   - 掌握基本节点类型
   - 理解行为树执行流程

### 中级阶段
1. **Timeline 进阶**：
   - 学习使用多种轨道类型
   - 掌握时间轴事件系统
   - 理解轨道混合和叠加

2. **TreeDesigner 进阶**：
   - 学习复杂节点组合
   - 掌握黑板数据系统
   - 理解子树复用机制

### 高级阶段
1. **系统集成**：
   - 学习 Timeline 与 TreeDesigner 的交互
   - 掌握数据共享机制
   - 理解同步控制模式

2. **扩展开发**：
   - 学习创建自定义轨道
   - 掌握自定义节点开发
   - 理解编辑器扩展

## 最佳实践总结

### 设计原则
1. **关注点分离**：
   - Timeline 负责时序控制
   - TreeDesigner 负责决策逻辑
   - 数据通过接口传递

2. **模块化设计**：
   - 将复杂行为拆分为子树
   - 将长时序拆分为子时间轴
   - 通过组合实现复杂功能

3. **性能优化**：
   - 避免在每帧创建新实例
   - 合理使用对象池
   - 优化条件检查频率

### 开发流程
1. **原型阶段**：
   - 快速搭建核心功能
   - 验证系统可行性
   - 收集反馈和需求

2. **实现阶段**：
   - 逐步完善功能细节
   - 添加错误处理和调试
   - 进行性能优化

3. **优化阶段**：
   - 代码重构和清理
   - 添加文档和示例
   - 进行系统测试

## 资源链接

### 文档
- `TacoTimelineLearning.md` - Timeline 系统详细文档
- `TacoTreeDesignerLearning.md` - TreeDesigner 系统详细文档
- 本文件 - 系统整体概述和集成指南

### 示例项目
- 查看 `TimelineSkill/Taco/Examples/` 目录（如果存在）
- 参考已有的使用场景
- 创建自己的测试场景

### 技术支持
- 查看源码注释和文档
- 参考 Unity 官方文档
- 在社区中寻求帮助

## 总结

Taco 框架通过 Timeline 和 TreeDesigner 两个系统的完美结合，为 Unity 游戏开发提供了强大的时序控制和行为管理能力。无论是简单的动画序列还是复杂的 AI 行为系统，Taco 都能提供高效、灵活、易用的解决方案。

通过本框架，开发者可以：
- 快速创建复杂的游戏逻辑
- 提高开发效率和代码质量
- 实现更好的游戏体验和表现力
- 降低维护成本和扩展难度

希望这份文档能帮助你更好地理解和使用 Taco 框架，创造出更加精彩的游戏作品！