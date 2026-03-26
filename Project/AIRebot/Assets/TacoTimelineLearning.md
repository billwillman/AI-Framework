# Taco Timeline 系统学习文档

## 概述

Taco Timeline 是一个基于 Unity 的时间轴系统，用于创建和管理复杂的动画、音频、粒子效果等时间线序列。它提供了可视化编辑、运行时播放和控制功能，支持多种类型的轨道和事件。

## 系统架构

### 核心类

#### 1. Timeline (主类)
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.cs`
- **功能**: 时间轴的核心容器，管理所有轨道和事件
- **关键属性**:
  - `Tracks`: 所有轨道的列表
  - `Duration`: 时间轴总时长
  - `IsPlaying`: 播放状态
  - `CurrentTime`: 当前播放时间
- **关键方法**:
  - `Play()`: 开始播放时间轴
  - `Pause()`: 暂停播放
  - `Stop()`: 停止播放
  - `Seek(float time)`: 跳转到指定时间
  - `AddTrack<T>()`: 添加新轨道
  - `RemoveTrack(Track track)`: 移除轨道

#### 2. TimelinePlayer
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/TimelinePlayer.cs`
- **功能**: 时间轴的播放器组件，MonoBehaviour 包装
- **关键属性**:
  - `Timeline`: 引用的 Timeline 对象
  - `PlayOnAwake`: 是否在 Awake 时自动播放
  - `Loop`: 是否循环播放
- **关键方法**:
  - `Play()`: 播放时间轴
  - `Pause()`: 暂停播放
  - `Stop()`: 停止播放
  - `SetTime(float time)`: 设置当前时间

#### 3. Track (轨道基类)
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Track.cs`
- **功能**: 所有轨道的基类，定义轨道的基本行为
- **关键属性**:
  - `Name`: 轨道名称
  - `Color`: 轨道颜色（用于编辑器显示）
  - `Clips`: 轨道上的剪辑列表
  - `Muted`: 是否静音
  - `Locked`: 是否锁定
- **关键方法**:
  - `Evaluate(float time)`: 在指定时间评估轨道状态
  - `AddClip(Clip clip)`: 添加剪辑
  - `RemoveClip(Clip clip)`: 移除剪辑

### 轨道类型

#### 1. AnimationTrack
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.Animation.cs`
- **功能**: 动画轨道，控制 Animator 或 Animation 组件
- **关键属性**:
  - `Target`: 目标 GameObject
  - `AnimationClip`: 动画剪辑
  - `BlendMode`: 混合模式
- **支持功能**:
  - 动画混合
  - 动画事件
  - 权重控制

#### 2. AudioTrack
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.Audio.cs`
- **功能**: 音频轨道，控制 AudioSource 组件
- **关键属性**:
  - `AudioSource`: 目标 AudioSource
  - `AudioClip`: 音频剪辑
  - `Volume`: 音量控制
  - `Pitch`: 音高控制
- **支持功能**:
  - 音频淡入淡出
  - 空间音频设置
  - 循环播放

#### 3. GameObjectTrack
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.GameObject.cs`
- **功能**: GameObject 轨道，控制 GameObject 的激活状态和变换
- **关键属性**:
  - `Target`: 目标 GameObject
  - `Position`: 位置曲线
  - `Rotation`: 旋转曲线
  - `Scale`: 缩放曲线
- **支持功能**:
  - 位置/旋转/缩放动画
  - 激活/禁用控制
  - 父子关系变化

#### 4. ParticleSystemTrack
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.ParticleSystem.cs`
- **功能**: 粒子系统轨道，控制 ParticleSystem 组件
- **关键属性**:
  - `ParticleSystem`: 目标粒子系统
  - `PlayOnAwake`: 是否自动播放
  - `Loop`: 是否循环
- **支持功能**:
  - 粒子发射控制
  - 粒子参数动画
  - 粒子停止/重置

#### 5. CinemachineTrack
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Cinemachine/Timeline.Cinemachine.cs`
- **功能**: Cinemachine 虚拟相机轨道，控制相机切换
- **关键属性**:
  - `VirtualCamera`: Cinemachine 虚拟相机
  - `BlendDuration`: 混合时长
  - `Priority`: 相机优先级
- **支持功能**:
  - 相机切换
  - 相机混合
  - 相机参数动画

### Tree 子系统 (行为树集成)

#### 1. TreeTrack
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Tree/Timeline.Tree.cs`
- **功能**: 行为树轨道，在时间轴上执行行为树
- **关键属性**:
  - `TreeAsset`: 行为树资源
  - `TreeRunner`: 行为树运行器
  - `StartTime`: 开始时间
  - `EndTime`: 结束时间
- **支持功能**:
  - 行为树执行
  - 树状态同步
  - 树参数传递

#### 2. TimelineNode
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Tree/Timeline.Node.cs`
- **功能**: 行为树节点，用于在时间轴上触发事件
- **关键属性**:
  - `Timeline`: 关联的时间轴
  - `EventTime`: 事件触发时间
  - `EventName`: 事件名称
- **支持功能**:
  - 时间轴事件触发
  - 行为树与时间轴同步

#### 3. TimelineRunningTree
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Tree/TimelineRunningTree.cs`
- **功能**: 运行时行为树，在时间轴播放期间执行
- **关键属性**:
  - `IsRunning`: 是否正在运行
  - `CurrentState`: 当前状态
- **支持功能**:
  - 实时行为树执行
  - 状态反馈
  - 错误处理

### 时间控制

#### TimeControl
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.TimeControl.cs`
- **功能**: 时间控制组件，管理时间缩放和播放速度
- **关键属性**:
  - `TimeScale`: 时间缩放因子
  - `PlaybackSpeed`: 播放速度
  - `IsPaused`: 是否暂停
- **关键方法**:
  - `SetTimeScale(float scale)`: 设置时间缩放
  - `SetPlaybackSpeed(float speed)`: 设置播放速度
  - `Pause()`: 暂停
  - `Resume()`: 恢复

### 实用工具

#### TimelineUtility
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/TimelineUtility.cs`
- **功能**: 时间轴实用工具类
- **关键方法**:
  - `CreateTimeline()`: 创建新时间轴
  - `LoadTimeline(string path)`: 加载时间轴
  - `SaveTimeline(Timeline timeline, string path)`: 保存时间轴
  - `BakeTimeline(Timeline timeline)`: 烘焙时间轴
  - `ConvertToAnimationClip(Timeline timeline)`: 转换为动画剪辑

### 编辑器集成

#### 1. TimelineEditorWindow
- **位置**: `TimelineSkill/Taco/Timeline/Editor/Scripts/TimelineEditorWindow.cs`
- **功能**: 时间轴编辑器主窗口
- **关键特性**:
  - 可视化轨道编辑
  - 时间轴缩放和滚动
  - 剪辑拖放
  - 关键帧编辑
  - 实时预览

#### 2. TimelineTrackView
- **位置**: `TimelineSkill/Taco/Timeline/Editor/Scripts/TimelineTrackView.cs`
- **功能**: 轨道视图组件
- **关键特性**:
  - 轨道列表显示
  - 轨道属性编辑
  - 剪辑时间线显示
  - 轨道颜色编码

### 属性系统

#### TimelineAttributes
- **位置**: `TimelineSkill/Taco/Timeline/Scripts/Timeline.Attributes.cs`
- **功能**: 自定义属性，用于编辑器扩展
- **关键属性**:
  - `[TrackColor]`: 设置轨道颜色
  - `[ClipIcon]`: 设置剪辑图标
  - `[HideInInspector]`: 在检视器中隐藏
  - `[ReadOnly]`: 只读属性

## 使用示例

### 基本用法

```csharp
// 创建时间轴
var timeline = new Timeline();
timeline.Duration = 10f;

// 添加动画轨道
var animTrack = timeline.AddTrack<AnimationTrack>();
animTrack.Name = "Character Animation";
animTrack.AnimationClip = Resources.Load<AnimationClip>("Run");

// 添加音频轨道
var audioTrack = timeline.AddTrack<AudioTrack>();
audioTrack.Name = "Background Music";
audioTrack.AudioClip = Resources.Load<AudioClip>("Music");

// 创建播放器
var player = gameObject.AddComponent<TimelinePlayer>();
player.Timeline = timeline;
player.PlayOnAwake = true;
player.Loop = true;

// 播放时间轴
player.Play();
```

### 高级用法：行为树集成

```csharp
// 创建带行为树的时间轴
var timeline = new Timeline();

// 添加行为树轨道
var treeTrack = timeline.AddTrack<TreeTrack>();
treeTrack.Name = "AI Behavior";
treeTrack.TreeAsset = Resources.Load<BaseTree>("AI/PatrolTree");
treeTrack.StartTime = 0f;
treeTrack.EndTime = 5f;

// 添加时间轴节点到行为树
var timelineNode = new TimelineNode();
timelineNode.Timeline = timeline;
timelineNode.EventTime = 2.5f;
timelineNode.EventName = "SpecialEvent";

// 播放
var player = gameObject.AddComponent<TimelinePlayer>();
player.Timeline = timeline;
player.Play();
```

### 编辑器脚本示例

```csharp
// 自定义轨道编辑器
[CustomEditor(typeof(AnimationTrack))]
public class AnimationTrackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var track = (AnimationTrack)target;
        
        EditorGUILayout.LabelField("Animation Track", EditorStyles.boldLabel);
        
        track.Target = EditorGUILayout.ObjectField("Target", track.Target, typeof(GameObject), true) as GameObject;
        track.AnimationClip = EditorGUILayout.ObjectField("Animation Clip", track.AnimationClip, typeof(AnimationClip), false) as AnimationClip;
        
        if (GUILayout.Button("Preview Animation"))
        {
            // 预览动画
        }
    }
}
```

## 系统特点

### 1. 模块化设计
- 每个轨道类型独立封装
- 易于扩展新的轨道类型
- 插件式架构

### 2. 高性能
- 基于事件的时间评估
- 优化的轨道更新
- 最小化GC分配

### 3. 易用性
- 直观的API设计
- 完整的编辑器支持
- 丰富的文档和示例

### 4. 集成性
- 与 Unity 动画系统深度集成
- 支持 Cinemachine 相机系统
- 与 Taco TreeDesigner 行为树系统无缝对接

### 5. 可扩展性
- 自定义轨道支持
- 自定义事件系统
- 脚本化扩展点

## 最佳实践

### 1. 性能优化
- 避免在每帧创建新的 Timeline 实例
- 使用对象池管理轨道和剪辑
- 在不需要时禁用轨道评估

### 2. 内存管理
- 及时释放不再使用的 Timeline 资源
- 使用 AssetBundle 加载 Timeline 资源
- 避免在 Timeline 中存储大型数据

### 3. 编辑器工作流
- 使用预制件保存常用的 Timeline 配置
- 利用 TimelineUtility 进行批量操作
- 自定义编辑器工具提高效率

### 4. 调试技巧
- 使用 TimelinePlayer 的调试视图
- 启用轨道日志输出
- 使用时间轴事件断点

## 常见问题

### Q1: 如何创建自定义轨道？
A: 继承 `Track` 基类并实现 `Evaluate` 方法，使用 `[TrackColor]` 属性设置轨道颜色。

### Q2: 时间轴可以循环播放吗？
A: 可以，通过 `TimelinePlayer.Loop` 属性或 `Timeline.SetLoop(true)` 方法设置。

### Q3: 如何同步多个时间轴？
A: 使用 `TimelineUtility.SyncTimelines()` 方法或创建主从时间轴关系。

### Q4: 时间轴支持网络同步吗？
A: 系统本身不包含网络同步，但可以通过自定义事件和网络消息实现。

### Q5: 如何导出时间轴为动画剪辑？
A: 使用 `TimelineUtility.ConvertToAnimationClip()` 方法。

## 目录结构

```
TimelineSkill/Taco/Timeline/
├── Scripts/
│   ├── Timeline.cs                    # 主时间轴类
│   ├── TimelinePlayer.cs              # 时间轴播放器
│   ├── Track.cs                       # 轨道基类
│   ├── Timeline.Animation.cs          # 动画轨道
│   ├── Timeline.Audio.cs              # 音频轨道
│   ├── Timeline.GameObject.cs         # GameObject轨道
│   ├── Timeline.ParticleSystem.cs     # 粒子系统轨道
│   ├── Timeline.TimeControl.cs        # 时间控制
│   ├── TimelineUtility.cs             # 实用工具
│   ├── Timeline.Attributes.cs         # 自定义属性
│   ├── Tree/
│   │   ├── Timeline.Tree.cs           # 行为树轨道
│   │   ├── Timeline.Node.cs           # 时间轴节点
│   │   └── TimelineRunningTree.cs     # 运行时行为树
│   └── Cinemachine/
│       └── Timeline.Cinemachine.cs    # Cinemachine轨道
└── Editor/
    └── Scripts/
        ├── TimelineEditorWindow.cs    # 编辑器窗口
        └── TimelineTrackView.cs       # 轨道视图
```

## 总结

Taco Timeline 系统是一个功能强大、易于使用的时间轴解决方案，特别适合游戏开发中的过场动画、技能序列、UI动画等场景。其模块化设计和良好的扩展性使得它能够适应各种复杂的需求，与 TreeDesigner 系统的深度集成为 AI 行为和时间控制的结合提供了完美的解决方案。