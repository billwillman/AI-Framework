# Taco TreeDesigner 系统学习文档

## 概述

Taco TreeDesigner 是一个可视化行为树编辑器系统，用于创建、编辑和执行复杂的行为树。它提供了完整的节点编辑器、运行时执行引擎和调试工具，特别适合游戏 AI、状态机和流程控制。

## 系统架构

### 核心类层次结构

```
BaseTree (抽象基类)
├── RunnableTree (可运行树)
│   └── OneRootTree (单根节点树)
└── SubTree (子树)

BaseNode (抽象基类)
├── RootNode (根节点)
│   └── EnterNode (入口节点)
├── CompositeNode (复合节点)
│   ├── SequenceNode (序列节点)
│   ├── SelectorNode (选择节点)
│   └── ParallelNode (并行节点)
├── DecoratorNode (装饰节点)
│   ├── LoopNode (循环节点)
│   ├── RepeatNode (重复节点)
│   ├── IfNode (条件节点)
│   ├── ForNode (循环节点)
│   └── WaitNode (等待节点)
├── ActionNode (动作节点)
│   ├── StateNode (状态节点)
│   ├── DebugNode (调试节点)
│   └── StopNode (停止节点)
├── ValueNode (值节点)
├── TriggerNode (触发器节点)
└── CustomNode (自定义节点)
    ├── SubTreeNode (子树节点)
    ├── ExposedPropertyNode (暴露属性节点)
    └── TreeValueNode (树值节点)
```

### 核心组件

#### 1. BaseTree (树基类)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Tree/BaseTree.cs`
- **功能**: 所有行为树的基类，定义树的基本结构
- **关键属性**:
  - `Nodes`: 树中所有节点的列表
  - `RootNode`: 根节点
  - `ExposedProperties`: 暴露的属性列表
  - `TreeName`: 树名称
- **关键方法**:
  - `AddNode(BaseNode node)`: 添加节点
  - `RemoveNode(BaseNode node)`: 移除节点
  - `ConnectNodes(BaseNode from, BaseNode to)`: 连接节点
  - `DisconnectNodes(BaseNode from, BaseNode to)`: 断开连接
  - `FindNode(string nodeId)`: 查找节点
  - `Clone()`: 克隆树

#### 2. RunnableTree (可运行树)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Tree/RunnableTree.cs`
- **功能**: 可执行的行为树，包含运行时状态
- **关键属性**:
  - `IsRunning`: 是否正在运行
  - `CurrentNode`: 当前执行的节点
  - `TreeState`: 树的状态（Success/Failure/Running）
  - `Blackboard`: 黑板数据（共享变量）
- **关键方法**:
  - `Start()`: 开始执行树
  - `Stop()`: 停止执行
  - `Update()`: 更新树状态
  - `Reset()`: 重置树状态
  - `GetValue<T>(string key)`: 获取黑板值
  - `SetValue<T>(string key, T value)`: 设置黑板值

#### 3. OneRootTree (单根节点树)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Tree/OneRootTree.cs`
- **功能**: 只有一个根节点的树，最常见的树类型
- **关键特性**:
  - 强制只有一个根节点
  - 自动创建入口节点
  - 简化树结构管理

#### 4. SubTree (子树)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Tree/SubTree.cs`
- **功能**: 可重用的子树资源
- **关键特性**:
  - 作为独立资源保存
  - 可在多个树中复用
  - 支持参数传递

### 节点系统

#### 1. BaseNode (节点基类)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/BaseNode.cs`
- **功能**: 所有节点的基类，定义节点的基本行为
- **关键属性**:
  - `NodeId`: 节点唯一标识
  - `NodeName`: 节点名称
  - `NodeType`: 节点类型
  - `Position`: 节点在编辑器中的位置
  - `ParentNodes`: 父节点列表
  - `ChildNodes`: 子节点列表
  - `InputPorts`: 输入端口列表
  - `OutputPorts`: 输出端口列表
- **关键方法**:
  - `OnStart()`: 节点开始执行时调用
  - `OnUpdate()`: 节点更新时调用
  - `OnStop()`: 节点停止时调用
  - `OnReset()`: 节点重置时调用
  - `Execute()`: 执行节点逻辑
  - `GetPort(string portName)`: 获取指定端口

#### 2. RunnableNode (可运行节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/RunnableNode.cs`
- **功能**: 可执行节点的基类，包含运行时状态
- **关键属性**:
  - `NodeState`: 节点状态（Success/Failure/Running）
  - `ExecutionTime`: 执行时间
  - `IsActive`: 是否激活
- **关键方法**:
  - `StartExecution()`: 开始执行
  - `UpdateExecution()`: 更新执行
  - `StopExecution()`: 停止执行
  - `FinishExecution(NodeState state)`: 完成执行

### 节点类型详解

#### 根节点类型

##### 1. RootNode (根节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Root/RootNode.cs`
- **功能**: 树的起点，每个树必须有一个根节点
- **特性**:
  - 没有父节点
  - 可以有多个子节点
  - 自动开始执行

##### 2. EnterNode (入口节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Root/EnterNode.cs`
- **功能**: 特殊的根节点，定义树的入口点
- **特性**:
  - 标记为树的开始
  - 可配置初始参数
  - 支持条件检查

#### 复合节点类型

##### 1. CompositeNode (复合节点基类)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Composite/CompositeNode.cs`
- **功能**: 管理多个子节点的执行顺序
- **特性**:
  - 可以包含多个子节点
  - 控制子节点的执行流程
  - 提供组合逻辑

##### 2. SequenceNode (序列节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Composite/SequenceNode.cs`
1. **功能**: 按顺序执行所有子节点，全部成功才算成功
- **执行逻辑**:
  - 依次执行每个子节点
  - 如果某个子节点失败，立即返回失败
  - 所有子节点成功才返回成功
- **应用场景**: 需要按顺序执行多个动作的任务

##### 3. SelectorNode (选择节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Composite/SelectorNode.cs`
- **功能**: 选择第一个成功的子节点执行
- **执行逻辑**:
  - 依次尝试每个子节点
  - 选择第一个成功的子节点
  - 如果所有子节点都失败，返回失败
- **应用场景**: 条件选择、优先级行为

##### 4. ParallelNode (并行节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Composite/ParallelNode.cs`
- **功能**: 并行执行所有子节点
- **执行逻辑**:
  - 同时启动所有子节点
  - 根据配置等待子节点完成
  - 支持成功/失败条件
- **应用场景**: 同时执行多个独立动作

#### 装饰节点类型

##### 1. DecoratorNode (装饰节点基类)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Decorator/DecoratorNode.cs`
- **功能**: 修饰子节点的行为，只能有一个子节点
- **特性**:
  - 改变子节点的执行逻辑
  - 添加条件检查
  - 控制执行次数

##### 2. LoopNode (循环节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Decorator/LoopNode.cs`
- **功能**: 循环执行子节点指定次数或直到条件满足
- **配置参数**:
  - `LoopCount`: 循环次数（0表示无限循环）
  - `BreakCondition`: 中断条件
  - `DelayBetweenLoops`: 循环间隔
- **应用场景**: 巡逻、重复动作

##### 3. RepeatNode (重复节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Decorator/RepeatNode.cs`
- **功能**: 重复执行子节点直到成功或失败
- **与 LoopNode 的区别**:
  - 关注执行结果而非次数
  - 支持成功/失败条件
  - 可配置超时时间

##### 4. IfNode (条件节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Decorator/IfNode.cs`
- **功能**: 条件判断，根据条件执行不同分支
- **配置参数**:
  - `Condition`: 条件表达式
  - `TrueNode`: 条件为真时执行的节点
  - `FalseNode`: 条件为假时执行的节点
- **应用场景**: 决策、分支逻辑

##### 5. ForNode (循环节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Decorator/ForNode.cs`
- **功能**: For 循环，支持索引变量
- **配置参数**:
  - `StartIndex`: 起始索引
  - `EndIndex`: 结束索引
  - `Step`: 步长
  - `IndexVariable`: 索引变量名
- **应用场景**: 遍历数组、重复操作

##### 6. WaitNode (等待节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Decorator/Time/WaitNode.cs`
- **功能**: 等待指定时间
- **配置参数**:
  - `WaitTime`: 等待时间（秒）
  - `RandomRange`: 随机时间范围
  - `CanBeInterrupted`: 是否可被中断
- **应用场景**: 延迟、冷却时间

#### 动作节点类型

##### 1. ActionNode (动作节点基类)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Action/ActionNode.cs`
- **功能**: 执行具体动作的节点基类
- **特性**:
  - 没有子节点
  - 执行具体游戏逻辑
  - 返回成功/失败状态

##### 2. StateNode (状态节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Action/StateNode.cs`
- **功能**: 管理状态机状态
- **配置参数**:
  - `StateName`: 状态名称
  - `StateParameters`: 状态参数
  - `TransitionConditions`: 转换条件
- **应用场景**: 状态机、行为状态

##### 3. DebugNode (调试节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Action/DebugNode.cs`
- **功能**: 调试输出，不影响游戏逻辑
- **配置参数**:
  - `Message`: 调试消息
  - `LogType`: 日志类型（Info/Warning/Error）
  - `Condition`: 输出条件
- **应用场景**: 调试、日志记录

##### 4. StopNode (停止节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Action/StopNode.cs`
- **功能**: 停止树或节点的执行
- **配置参数**:
  - `StopTarget`: 停止目标（当前节点/整个树）
  - `StopReason`: 停止原因
  - `ForceStop`: 是否强制停止
- **应用场景**: 异常处理、紧急停止

#### 值节点类型

##### ValueNode (值节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Value/ValueNode.cs`
- **功能**: 提供常量值或计算值
- **支持的数据类型**:
  - `int`, `float`, `bool`, `string`
  - `Vector2`, `Vector3`, `Quaternion`
  - `GameObject`, `Component`
  - 自定义对象
- **应用场景**: 参数传递、计算值

#### 自定义节点类型

##### 1. SubTreeNode (子树节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Custom/SubTreeNode.cs`
- **功能**: 引用和执行子树
- **配置参数**:
  - `SubTreeAsset`: 子树资源
  - `InputMapping`: 输入参数映射
  - `OutputMapping`: 输出参数映射
- **应用场景**: 模块化设计、代码复用

##### 2. ExposedPropertyNode (暴露属性节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Custom/ExposedPropertyNode.cs`
- **功能**: 访问和修改树的暴露属性
- **配置参数**:
  - `PropertyName`: 属性名称
  - `Operation`: 操作类型（Get/Set）
  - `Value`: 设置的值
- **应用场景**: 动态参数控制

##### 3. TreeValueNode (树值节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Custom/TreeValueNode.cs`
- **功能**: 访问树的黑板值
- **配置参数**:
  - `Key`: 黑板键名
  - `DefaultValue`: 默认值
  - `ValueType`: 值类型
- **应用场景**: 数据共享、状态传递

#### 触发器节点类型

##### TriggerNode (触发器节点)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Node/Trigger/TriggerNode.cs`
- **功能**: 响应外部事件触发器
- **配置参数**:
  - `TriggerEvent`: 触发事件名称
  - `Condition`: 触发条件
  - `AutoReset`: 是否自动重置
- **应用场景**: 事件驱动、响应式行为

### 端口系统

#### PropertyPort (属性端口)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/PropertyPort/PropertyPort.cs`
- **功能**: 定义节点的输入输出端口
- **关键属性**:
  - `PortName`: 端口名称
  - `PortType`: 端口类型（Input/Output）
  - `DataType`: 数据类型
  - `IsMultiple`: 是否允许多个连接
  - `DefaultValue`: 默认值
- **关键方法**:
  - `Connect(PropertyPort otherPort)`: 连接端口
  - `Disconnect(PropertyPort otherPort)`: 断开连接
  - `GetValue<T>()`: 获取端口值
  - `SetValue<T>(T value)`: 设置端口值

### 边系统

#### BaseEdge (边基类)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Edge/BaseEdge.cs`
- **功能**: 连接两个节点的边
- **关键属性**:
  - `FromNode`: 源节点
  - `ToNode`: 目标节点
  - `FromPort`: 源端口
  - `ToPort`: 目标端口
  - `EdgeType`: 边类型（数据流/控制流）
- **关键方法**:
  - `CanConnect()`: 检查是否可以连接
  - `ValidateConnection()`: 验证连接有效性
  - `TransferData()`: 传输数据

### 暴露属性系统

#### ExposedProperty (暴露属性)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/ExposedProperty/ExposedProperty.cs`
- **功能**: 可在树外部访问和修改的属性
- **关键属性**:
  - `PropertyName`: 属性名称
  - `PropertyType`: 属性类型
  - `DefaultValue`: 默认值
  - `IsReadOnly`: 是否只读
  - `Description`: 属性描述
- **支持的操作**:
  - 在编辑器中配置
  - 在运行时动态修改
  - 序列化保存

### 运行时系统

#### TreeRunner (树运行器)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/TreeRunner.cs`
- **功能**: MonoBehaviour 组件，在场景中运行行为树
- **关键属性**:
  - `TreeAsset`: 行为树资源
  - `AutoStart`: 是否自动开始
  - `UpdateMode`: 更新模式（Update/FixedUpdate/LateUpdate）
  - `DebugMode`: 调试模式
- **关键方法**:
  - `StartTree()`: 开始执行树
  - `StopTree()`: 停止执行
  - `PauseTree()`: 暂停执行
  - `ResumeTree()`: 恢复执行
  - `RestartTree()`: 重新开始
- **事件**:
  - `OnTreeStarted`: 树开始事件
  - `OnTreeStopped`: 树停止事件
  - `OnNodeExecuted`: 节点执行事件
  - `OnTreeStateChanged`: 树状态改变事件

#### Debugger (调试器)
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Debugger.cs`
- **功能**: 行为树调试工具
- **关键特性**:
  - 实时显示树执行状态
  - 节点高亮显示
  - 执行路径追踪
  - 性能分析
  - 断点调试

### 枚举定义

#### Enum.cs
- **位置**: `TimelineSkill/Taco/TreeDesigner/Scripts/Enum.cs`
- **包含的枚举**:
  - `NodeType`: 节点类型（Root/Composite/Decorator/Action等）
  - `NodeState`: 节点状态（Success/Failure/Running）
  - `TreeState`: 树状态
  - `PortType`: 端口类型（Input/Output）
  - `EdgeType`: 边类型（Data/Control）
  - `UpdateMode`: 更新模式
  - `LogType`: 日志类型

## 使用示例

### 基本用法：创建简单行为树

```csharp
// 创建行为树
var tree = ScriptableObject.CreateInstance<OneRootTree>();
tree.TreeName = "SimpleAI";

// 创建根节点
var rootNode = new EnterNode();
rootNode.NodeName = "Start";
tree.AddNode(rootNode);
tree.RootNode = rootNode;

// 创建序列节点
var sequenceNode = new SequenceNode();
sequenceNode.NodeName = "Main Sequence";
tree.AddNode(sequenceNode);
tree.ConnectNodes(rootNode, sequenceNode);

// 创建等待节点
var waitNode = new WaitNode();
waitNode.NodeName = "Wait";
waitNode.WaitTime = 2f;
tree.AddNode(waitNode);
tree.ConnectNodes(sequenceNode, waitNode);

// 创建调试节点
var debugNode = new DebugNode();
debugNode.NodeName = "Log";
debugNode.Message = "Action completed!";
tree.AddNode(debugNode);
tree.ConnectNodes(sequenceNode, debugNode);

// 在场景中使用
var runner = gameObject.AddComponent<TreeRunner>();
runner.TreeAsset = tree;
runner.AutoStart = true;
```

### 高级用法：带条件的行为树

```csharp
// 创建带条件的行为树
var tree = ScriptableObject.CreateInstance<OneRootTree>();
tree.TreeName = "ConditionalAI";

// 添加暴露属性
var healthProperty = new ExposedProperty();
healthProperty.PropertyName = "Health";
healthProperty.PropertyType = typeof(float);
healthProperty.DefaultValue = 100f;
tree.ExposedProperties.Add(healthProperty);

// 创建根节点
var root = new EnterNode();
tree.AddNode(root);
tree.RootNode = root;

// 创建选择节点（决策）
var selector = new SelectorNode();
tree.AddNode(selector);
tree.ConnectNodes(root, selector);

// 条件1：低血量时逃跑
var ifLowHealth = new IfNode();
ifLowHealth.Condition = "Health < 30";
tree.AddNode(ifLowHealth);

var escapeSequence = new SequenceNode();
tree.AddNode(escapeSequence);

var debugEscape = new DebugNode();
debugEscape.Message = "Health low, escaping!";
tree.AddNode(debugEscape);

tree.ConnectNodes(ifLowHealth, escapeSequence);
tree.ConnectNodes(escapeSequence, debugEscape);

// 条件2：正常状态时攻击
var attackSequence = new SequenceNode();
tree.AddNode(attackSequence);

var debugAttack = new DebugNode();
debugAttack.Message = "Attacking enemy!";
tree.AddNode(debugAttack);

tree.ConnectNodes(selector, ifLowHealth);
tree.ConnectNodes(selector, attackSequence);
tree.ConnectNodes(attackSequence, debugAttack);
```

### 编辑器扩展示例

```csharp
// 自定义节点编辑器
[CustomEditor(typeof(ActionNode))]
public class ActionNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var node = (ActionNode)target;
        
        EditorGUILayout.LabelField("Action Node", EditorStyles.boldLabel);
        
        node.NodeName = EditorGUILayout.TextField("Node Name", node.NodeName);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Execution Settings", EditorStyles.boldLabel);
        
        // 自定义属性编辑
        // ...
        
        if (GUILayout.Button("Test Node"))
        {
            // 测试节点逻辑
        }
    }
}
```

## 系统特点

### 1. 可视化编辑
- 完整的节点编辑器
- 拖放式节点连接
- 实时预览
- 多级撤销/重做

### 2. 模块化设计
- 可复用的子树
- 自定义节点类型
- 插件式架构

### 3. 高性能
- 优化的节点执行
- 最小化GC分配
- 支持大量并发树

### 4. 易用性
- 直观的API设计
- 完整的文档和示例
- 丰富的调试工具

### 5. 可扩展性
- 自定义节点支持
- 自定义数据类型
- 编辑器扩展点

### 6. 集成性
- 与 Unity 深度集成
- 支持 Timeline 系统
- 可与其他 AI 系统结合

## 最佳实践

### 1. 树设计原则
- 保持树结构简洁
- 合理使用复合节点
- 避免过深的嵌套
- 使用子树实现模块化

### 2. 性能优化
- 避免在每帧创建新节点
- 使用对象池管理节点实例
- 优化条件检查频率
- 禁用不需要的调试功能

### 3. 内存管理
- 及时释放不再使用的树
- 使用 AssetBundle 加载树资源
- 避免在节点中存储大型数据

### 4. 调试技巧
- 使用 DebugNode 输出关键信息
- 启用树调试视图
- 设置执行断点
- 使用性能分析器

### 5. 编辑器工作流
- 使用预制节点库
- 创建常用子树模板
- 利用搜索和过滤功能
- 自定义编辑器工具

## 常见问题

### Q1: 如何创建自定义节点？
A: 继承 `BaseNode` 或 `ActionNode` 基类，实现必要的方法，使用 `[NodeType]` 属性注册节点类型。

### Q2: 行为树可以并行执行吗？
A: 可以，使用 `ParallelNode` 或创建多个 `TreeRunner` 实例。

### Q3: 如何保存和加载行为树？
A: 行为树是 ScriptableObject，可以使用 Unity 的资源系统保存和加载。

### Q4: 行为树支持网络同步吗？
A: 系统本身不包含网络同步，但可以通过黑板数据和自定义节点实现。

### Q5: 如何调试行为树执行？
A: 使用 `Debugger` 组件或 `DebugNode` 节点，启用树调试视图。

## 目录结构

```
TimelineSkill/Taco/TreeDesigner/
├── Scripts/
│   ├── Tree/
│   │   ├── BaseTree.cs              # 树基类
│   │   ├── RunnableTree.cs          # 可运行树
│   │   ├── OneRootTree.cs           # 单根节点树
│   │   └── SubTree.cs               # 子树
│   ├── Node/
│   │   ├── BaseNode.cs              # 节点基类
│   │   ├── RunnableNode.cs          # 可运行节点
│   │   ├── Root/
│   │   │   ├── RootNode.cs          # 根节点
│   │   │   └── EnterNode.cs         # 入口节点
│   │   ├── Composite/
│   │   │   ├── CompositeNode.cs     # 复合节点基类
│   │   │   ├── SequenceNode.cs      # 序列节点
│   │   │   ├── SelectorNode.cs      # 选择节点
│   │   │   └── ParallelNode.cs      # 并行节点
│   │   ├── Decorator/
│   │   │   ├── DecoratorNode.cs     # 装饰节点基类
│   │   │   ├── LoopNode.cs          # 循环节点
│   │   │   ├── RepeatNode.cs        # 重复节点
│   │   │   ├── IfNode.cs            # 条件节点
│   │   │   ├── ForNode.cs           # 循环节点
│   │   │   └── Time/
│   │   │       └── WaitNode.cs      # 等待节点
│   │   ├── Action/
│   │   │   ├── ActionNode.cs        # 动作节点基类
│   │   │   ├── StateNode.cs         # 状态节点
│   │   │   ├── DebugNode.cs         # 调试节点
│   │   │   └── StopNode.cs          # 停止节点
│   │   ├── Value/
│   │   │   └── ValueNode.cs         # 值节点
│   │   ├── Trigger/
│   │   │   └── TriggerNode.cs       # 触发器节点
│   │   └── Custom/
│   │       ├── SubTreeNode.cs       # 子树节点
│   │       ├── ExposedPropertyNode.cs # 暴露属性节点
│   │       └── TreeValueNode.cs     # 树值节点
│   ├── PropertyPort/
│   │   └── PropertyPort.cs          # 属性端口
│   ├── Edge/
│   │   └── BaseEdge.cs              # 边基类
│   ├── ExposedProperty/
│   │   └── ExposedProperty.cs       # 暴露属性
│   ├── TreeRunner.cs                # 树运行器
│   ├── Debugger.cs                  # 调试器
│   └── Enum.cs                      # 枚举定义
└── Editor/
    └── (编辑器相关脚本)
```

## 与 Timeline 系统的集成

TreeDesigner 与 Timeline 系统深度集成，主要通过以下方式：

1. **TreeTrack**: 在时间轴上执行行为树
2. **TimelineNode**: 在行为树中触发时间轴事件
3. **共享数据**: 通过黑板和暴露属性共享数据
4. **同步控制**: 时间轴控制行为树的开始/停止

这种集成使得可以创建复杂的时序行为，如：
- 过场动画中的角色AI行为
- 技能序列中的决策逻辑
- 场景事件触发的行为变化

## 总结

Taco TreeDesigner 是一个功能强大、灵活易用的行为树系统，特别适合游戏开发中的 AI 行为、状态机、流程控制等场景。其可视化编辑器和完整的运行时支持使得行为树的设计和执行变得简单高效，与 Timeline 系统的深度集成为时序控制提供了完美的解决方案。