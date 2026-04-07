# Taco GameplayTag 系统使用说明文档

## 目录

- [1. 系统概述](#1-系统概述)
- [2. 核心类结构](#2-核心类结构)
- [3. GameplayTagData — 标签数据资产](#3-gameplaytagdata--标签数据资产)
- [4. GameplayTagInfo — 标签信息](#4-gameplaytaginfo--标签信息)
- [5. GameplayTagContainer — 标签容器](#5-gameplaytagcontainer--标签容器)
  - [5.1 编辑器操作（Editor）](#51-编辑器操作editor)
  - [5.2 运行时操作（Runtime）](#52-运行时操作runtime)
  - [5.3 查询方法](#53-查询方法)
- [6. GameplayTagUtility — 工具类](#6-gameplaytagutility--工具类)
  - [6.1 标签匹配](#61-标签匹配)
  - [6.2 父子关系查询](#62-父子关系查询)
  - [6.3 容器级别匹配](#63-容器级别匹配)
- [7. 初始化流程](#7-初始化流程)
- [8. 在 Ability 中的实际使用](#8-在-ability-中的实际使用)
- [9. 编辑器工具](#9-编辑器工具)
  - [9.1 标签编辑窗口（GameplayTagEditWindow）](#91-标签编辑窗口gameplaytageditwwindow)
  - [9.2 标签选择窗口（GameplayTagSelectWindow）](#92-标签选择窗口gameplaytagselectwindow)
  - [9.3 容器 PropertyDrawer](#93-容器-propertydrawer)
- [10. 当前项目已定义的标签](#10-当前项目已定义的标签)
- [11. 最佳实践](#11-最佳实践)

---

## 1. 系统概述

Taco 的 GameplayTag 系统是一套 **层级化标签管理方案**，用于 Timeline 技能和行为树系统中。标签采用 **"."分隔的层级路径** 表示，例如 `Mugen.Stand.Idle`。

核心设计特点：

- **GUID 引用**：容器内部通过 GUID 引用标签，重命名标签时不会丢失引用关系
- **编辑器友好**：提供完整的可视化编辑窗口、选择器弹窗、PropertyDrawer
- **引用追踪**：每个标签记录自身被哪些对象引用（`Reference` 字段），方便删除前检查影响
- **运行时/编辑器分离**：编辑器下通过 GUID 操作，运行时通过字符串名称操作

**命名空间**: `Taco.Gameplay`

---

## 2. 核心类结构

```
GameplayTagData (ScriptableObject)    ← 全局标签数据资产（存储所有标签定义）
├── GameplayTagInfo                   ← 单个标签的信息（Name, Guid, Expanded, Multi, Reference）
│
GameplayTagContainer                  ← 标签容器（挂在具体对象上，选中了哪些标签）
│   ├── TagGuids: List<string>        ← 编辑器序列化（持久存储）
│   └── Tags: List<string>            ← 运行时缓存（标签名称列表）
│
GameplayTagUtility (static)           ← 工具类（匹配、父子关系查询）
```

**文件清单：**

| 文件 | 说明 |
|------|------|
| `Taco/Gameplay/Scripts/GameplayTagData.cs` | 标签数据资产 + GameplayTagInfo 定义 |
| `Taco/Gameplay/Scripts/GameplayTagContainer.cs` | 标签容器（运行时部分） |
| `Taco/Gameplay/Scripts/GameplayTagContainerExtension.cs` | 标签容器（编辑器扩展，partial class） |
| `Taco/Gameplay/Scripts/GameplayTagUtility.cs` | 标签工具类（匹配、查询） |
| `Taco/Gameplay/Scripts/GameplayTagUtilityExtension.cs` | 工具类编辑器初始化 |
| `Taco/Gameplay/Editor/Scripts/GameplayTagEditWindow.cs` | 标签编辑窗口 |
| `Taco/Gameplay/Editor/Scripts/GameplayTagSelectWindow.cs` | 标签选择窗口 |
| `Taco/Gameplay/Editor/Scripts/GameplayTagContainerDrawer.cs` | 容器 PropertyDrawer |
| `Taco/Gameplay/Editor/Scripts/GameplayTagContainerView.cs` | 容器可视化 View |
| `Taco/Gameplay/Editor/Scripts/GameplayTagEditorUtility.cs` | 编辑器工具类 |
| `Taco/Gameplay/Editor/Scripts/GameplayTagInfoView.cs` | 标签信息视图 |

---

## 3. GameplayTagData — 标签数据资产

`GameplayTagData` 是继承自 `ScriptableObject` 的全局标签数据库，存储项目中所有已定义的标签。

**资产路径**: `Assets/Resources/GameplayTagData.asset`

```csharp
public class GameplayTagData : ScriptableObject
{
    List<GameplayTagInfo> m_GameplayTagInfos;   // 所有标签信息列表
    Dictionary<string, GameplayTagInfo> m_NameTagInfoMap;  // 名称 → 信息 映射
    Dictionary<string, GameplayTagInfo> m_GuidTagInfoMap;  // GUID → 信息 映射
}
```

**常用方法：**

| 方法 | 说明 |
|------|------|
| `Init()` | 初始化映射字典和编辑器树结构 |
| `Contains(string tag)` | 检查标签是否存在 |
| `NameToInfo(string name)` | 名称 → GameplayTagInfo |
| `GuidToInfo(string guid)` | GUID → GameplayTagInfo |
| `NameToGuid(string name)` | 名称 → GUID |
| `GuidToName(string guid)` | GUID → 名称 |
| `this[string tag]` | 索引器，等同于 NameToInfo |

**编辑器专用方法（`#if UNITY_EDITOR`）：**

| 方法 | 说明 |
|------|------|
| `AddTag(string tag)` | 添加标签（自动创建缺失的父标签） |
| `RemoveTag(string tagName)` | 删除标签及其所有子标签 |
| `RemoveTagWithoutChildren(string tag)` | 仅删除当前标签，子标签上移到父节点 |
| `ChangeTag(oldTag, newTag, newShortTag)` | 重命名标签 |
| `MoveTag(movingTag, targetParentTag)` | 移动标签到另一个父节点下 |
| `MoveToRoot(movingTag)` | 移动到根层级 |
| `GetChildTagInfos(parentTag, includeSelf)` | 获取所有子标签 |
| `SetExpandedState(name, state)` | 设置展开/折叠状态 |
| `SetMultiState(name, state)` | 设置单选/多选模式 |

---

## 4. GameplayTagInfo — 标签信息

每个已定义的标签对应一个 `GameplayTagInfo` 实例：

```csharp
[Serializable]
public class GameplayTagInfo
{
    public string Name;           // 完整标签名，如 "Mugen.Stand.Idle"
    public string Guid;           // 唯一标识符
    public bool Expanded;         // 编辑器中是否展开
    public bool Multi;            // 是否为多选模式（允许同时选择同级多个子标签）
    public List<string> Reference; // 引用此标签的对象列表（GlobalObjectId 格式）
}
```

**Multi 字段说明：**
- `Multi = false`（Single 模式）：选择子标签时自动取消父标签的选择
- `Multi = true`（Multi 模式）：允许同级多个子标签同时被选中

---

## 5. GameplayTagContainer — 标签容器

`GameplayTagContainer` 是挂在具体业务对象上的标签选择器，表示"这个对象选中了哪些标签"。

```csharp
[Serializable]
public partial class GameplayTagContainer
{
    public List<string> TagGuids;       // 序列化存储的 GUID 列表
    public string ReferencePath;        // 引用路径标识（用于追踪引用）
    
    List<string> m_Tags;               // 运行时标签名称缓存
    public List<string> Tags { get; }  // 访问时自动调用 Init()
    
    public Action OnValueChanged;      // 值变化回调
}
```

### 5.1 编辑器操作（Editor）

编辑器下操作会同步维护 `TagGuids`、`Tags`、`Reference` 三方数据：

```csharp
// 添加标签（编辑器下）
container.AddTag(GameplayTagInfo tagToAdd);

// 移除标签（编辑器下）
container.RemoveTag(GameplayTagInfo tagToRemove);

// 移除标签及其所有子标签（编辑器下）
container.RemoveTagWithChild(GameplayTagInfo tagToRemove);

// 清空所有标签（编辑器下）
container.ClearTags();
```

### 5.2 运行时操作（Runtime）

运行时操作只维护 `Tags` 列表（字符串名称），不涉及 GUID 和 Reference：

```csharp
// 添加标签（运行时）
container.AddTagRuntime("Mugen.Jump");

// 移除标签（运行时）
container.RemoveTagRuntime("Mugen.Jump");

// 移除标签及其所有子标签（运行时）
container.RemoveTagWithChildRuntime("Mugen.Stand");

// 清空（运行时）
container.ClearTagRuntime();
```

**AddTagRuntime 的特殊行为：** 添加子标签时，会自动移除已存在的父标签。例如容器中有 `Mugen.Stand`，添加 `Mugen.Stand.Idle` 会移除 `Mugen.Stand`。

### 5.3 查询方法

```csharp
// 容器中是否包含输入标签，或包含输入标签的父标签
// 例: 容器有 "Mugen.Stand.Idle"，查询 "Mugen.Stand" 返回 true
bool IsParentOf(string childTag);

// 容器中是否包含输入标签，或包含输入标签的子标签
// 例: 容器有 "Mugen.Stand"，查询 "Mugen.Stand.Idle" 返回 true
bool IsChildOf(string parentTag);
```

---

## 6. GameplayTagUtility — 工具类

### 6.1 标签匹配

```csharp
// 检查 tag 是否以 targetTag 开头（精确层级匹配，不是简单 StartsWith）
// "Mugen.Stand".StartTagIs("Mugen")         → true
// "Mugen.Stand".StartTagIs("Mugen.Stand")   → true
// "Mugen.StandUp".StartTagIs("Mugen.Stand") → false（层级不匹配）
bool StartTagIs(this string tag, string targetTag);

// 检查 tag 是否以 targetTag 结尾（精确层级匹配）
bool EndTagIs(this string tag, string targetTag);
```

### 6.2 父子关系查询

```csharp
// 获取父标签
// "Mugen.Stand.Idle" → "Mugen.Stand"
// "Mugen" → ""
string GetParentTag(string childTag);

// 获取所有父标签（不含自身）
// "Mugen.Stand.Idle" → ["Mugen", "Mugen.Stand"]
string[] GetParentTags(string childTag);

// 从标签列表中获取指定父标签的所有子标签（不含自身）
string[] GetChildrenTags(string parentTag, List<string> tags);

// 获取两个标签之间的中间层级标签
string[] GetMiddleTags(string parentTag, string childTag);
```

### 6.3 容器级别匹配

```csharp
// a 的每一个 tag 都是 b 的父 tag
bool AllParentOf(this GameplayTagContainer a, GameplayTagContainer b);

// a 中至少有一个 tag 是 b 的子 tag
bool PartChildOf(this GameplayTagContainer a, GameplayTagContainer b);
```

---

## 7. 初始化流程

### 运行时初始化

```csharp
// GameplayTagUtility.cs - 通过 [RuntimeInitializeOnLoadMethod] 自动执行
[RuntimeInitializeOnLoadMethod]
public static void RuntimeInit()
{
    GameplayTagData = Resources.Load<GameplayTagData>("GameplayTagData");
    GameplayTagData?.Init();
}
```

### 编辑器初始化

```csharp
// GameplayTagUtilityExtension.cs - 通过 [InitializeOnLoadMethod] 自动执行
[InitializeOnLoadMethod]
public static void EditorInit()
{
    GameplayTagData = Resources.Load<GameplayTagData>("GameplayTagData");
    GameplayTagData?.Init();
}
```

### GameplayTagContainer 初始化

容器在访问 `Tags` 属性时自动初始化，将 GUID 列表转换为名称列表：

```csharp
public void Init()
{
    m_Tags = new List<string>();
    for (int i = TagGuids.Count - 1; i >= 0; i--)
    {
        string tag = m_GameplayTagData.GuidToName(TagGuids[i]);
        if (!string.IsNullOrEmpty(tag))
            m_Tags.Add(tag);
    }
    OnValueChanged?.Invoke();
}
```

---

## 8. 在 Ability 中的实际使用

### 8.1 AnimancerAbility 的 Tag 字段定义

`AnimancerAbility`（UnityTimeline 体系的技能）定义了 5 个 `GameplayTagContainer` 字段，控制技能的标识、激活条件、互斥关系：

```csharp
public partial class AnimancerAbility : OneRootTree
{
    public GameplayTagContainer AbilityTags;           // 技能自身标签
    public GameplayTagContainer CancelAbilitiesWithTag; // 激活时取消带有这些标签的技能
    public GameplayTagContainer BlockAbilitiesWithTag;  // 阻止带有这些标签的技能激活
    public GameplayTagContainer ActiveTags;             // 激活期间授予的标签
    public GameplayTagContainer RequiredTags;           // 激活所需标签
}
```

### 8.2 各 Tag 字段详解

#### AbilityTags — 技能自身标签

**作用**：标识这个技能"是什么"，相当于技能的身份证。

**被谁使用**：
- 被其他技能的 `CancelAbilitiesWithTag` 匹配，判断是否应该被取消
- 被其他技能的 `BlockAbilitiesWithTag` 匹配，判断是否被阻止激活
- 被 Agent 全局的 `BlockAbilitiesWithTag` 匹配，判断是否被全局阻止

**匹配逻辑**（支持层级匹配）：
```
技能 A 的 AbilityTags = [Mugen.Stand.Idle]
技能 B 的 CancelAbilitiesWithTag = [Mugen.Stand]

→ B 激活时会取消 A（因为 Mugen.Stand.Idle 是 Mugen.Stand 的子标签）
```

#### CancelAbilitiesWithTag — 激活时取消其他技能

**作用**：当本技能成功激活时，会取消所有 `AbilityTags` 匹配这些标签的正在运行的技能。

**执行时机**：`TryStartAbility` 中，在通过所有检查、即将调用 `StartAbility()` 之前执行。

**代码逻辑**：
```csharp
// AnimancerAbilityAgent.TryStartAbility 中的取消逻辑
foreach (var ability in Abilities)
{
    if (ability.Active)
    {
        // 检查正在运行的技能的 AbilityTags 是否部分匹配本技能的 CancelAbilitiesWithTag
        if (ability.AbilityTags.PartChildOf(abilityToStart.CancelAbilitiesWithTag))
        {
            ability.CancelAbility(abilityToStart);
            TryStopAbility(ability);
        }
    }
}
```

**典型场景**：
- 跳跃技能设置 `CancelAbilitiesWithTag = [Mugen.Stand]` → 跳跃时取消所有站立状态技能

#### BlockAbilitiesWithTag — 阻止其他技能激活

**作用**：当本技能正在运行时，阻止所有 `AbilityTags` 匹配这些标签的技能激活。被阻止的技能会尝试进入缓冲队列。

**执行时机**：`TryStartAbility` 中，在检查新技能能否激活时遍历所有已激活技能。

**代码逻辑**：
```csharp
// AnimancerAbilityAgent.TryStartAbility 中的阻止检查
foreach (var ability in Abilities)
{
    if (ability.Active && abilityToStart.AbilityTags.PartChildOf(ability.BlockAbilitiesWithTag))
    {
        Starting = false;
        AddToBuffer(abilityToStart);  // 加入缓冲队列，等阻止条件消失后重试
        return false;
    }
}
```

**典型场景**：
- 攻击技能设置 `BlockAbilitiesWithTag = [Mugen.Stand]` → 攻击期间不能切换到站立状态

#### ActiveTags — 激活期间授予的标签

**作用**：技能激活时将这些标签添加到 Agent 的全局 `ActiveTags` 列表中，技能结束时自动移除。其他技能的 `RequiredTags` 可以检查这些标签。

**生命周期**：
- `StartAbility()` → 添加到 `Runner.ActiveTags`
- `StopAbility()` → 从 `Runner.ActiveTags` 移除

**代码逻辑**：
```csharp
protected virtual void OnStartAbility()
{
    foreach (var tag in ActiveTags.Tags)
        Runner.ActiveTags.Add(tag);
}

protected virtual void OnStopAbility()
{
    foreach (var tag in ActiveTags.Tags)
        Runner.ActiveTags.Remove(tag);
}
```

**典型场景**：
- 站立 Idle 技能设置 `ActiveTags = [Mugen.Stand]` → 其他需要"站立状态"才能激活的技能可以通过 RequiredTags 检查

#### RequiredTags — 激活所需标签

**作用**：技能激活前检查 Agent 的全局 `ActiveTags` 中是否包含所有这些标签。**任何一个不满足则拒绝激活**，技能会被加入缓冲队列等待条件满足。

**执行时机**：`TryStartAbility` 中，是第一个检查项。

**代码逻辑**：
```csharp
// AnimancerAbilityAgent.TryStartAbility 中的前置条件检查
foreach (var requiredTag in abilityToStart.RequiredTags.Tags)
{
    bool isChild = false;
    foreach (var activeTag in ActiveTags)
    {
        if (activeTag.StartTagIs(requiredTag))  // 层级匹配
        {
            isChild = true;
            break;
        }
    }
    if (!isChild)
    {
        AddToBuffer(abilityToStart);  // 条件不满足，加入缓冲
        return false;
    }
}
```

**典型场景**：
- 行走技能设置 `RequiredTags = [Mugen.Stand]` → 必须在站立状态下才能行走

### 8.3 Agent 全局 Tag 列表

`AnimancerAbilityAgent` 维护三个全局 Tag 列表，由各技能动态修改：

```csharp
public class AnimancerAbilityAgent
{
    public List<string> ActiveTags;              // 当前激活的标签（由各技能的 ActiveTags 填充）
    public List<string> BlockAbilitiesWithTag;   // 全局阻止标签（由 Timeline Clip 动态添加/移除）
    public List<string> CanBufferAbilitiesTag;   // 允许缓冲的标签（由 Timeline Clip 动态添加/移除）
}
```

- **ActiveTags**：技能启动时添加，停止时移除。`RequiredTags` 检查的就是这个列表
- **BlockAbilitiesWithTag**：全局阻止列表，与技能自身的 `BlockAbilitiesWithTag` 同时生效。可通过 Timeline 的 `ModifyRuntimeBlockTagClip` 动态控制
- **CanBufferAbilitiesTag**：控制哪些标签的技能允许被缓冲（排队等待执行）。可通过 Timeline 的 `ModifyCanBuffAbilitiesTagClip` 动态控制

### 8.4 TryStartAbility 完整判定流程

```
TryStartAbility(abilityToStart)
│
├─ 1. RequiredTags 检查
│     遍历 abilityToStart.RequiredTags，检查 Agent.ActiveTags 中是否都满足
│     → 不满足：加入缓冲，返回 false
│
├─ 2. Agent 全局 BlockAbilitiesWithTag 检查
│     检查 abilityToStart.AbilityTags 是否被 Agent.BlockAbilitiesWithTag 阻止
│     → 被阻止：加入缓冲，返回 false
│
├─ 3. 其他已激活技能的 BlockAbilitiesWithTag 检查
│     遍历所有 Active 技能，检查它们的 BlockAbilitiesWithTag 是否阻止 abilityToStart
│     → 被阻止：加入缓冲，返回 false
│
├─ 4. CanStart() 自定义检查
│     调用技能行为树中的 AbilityCanStartNode 自定义条件
│     → 不通过：加入缓冲，返回 false
│
├─ 5. CancelAbilitiesWithTag 执行
│     遍历所有 Active 技能，取消 AbilityTags 匹配 abilityToStart.CancelAbilitiesWithTag 的技能
│
└─ 6. 启动技能
      调用 StartAbility()，将 ActiveTags 添加到 Agent.ActiveTags
```

### 8.5 Timeline Clip 动态 Tag 控制

在 `AbilityTimeline` 中有两种特殊 Clip，可以在技能执行过程中动态修改 Agent 的全局 Tag：

#### ModifyRuntimeBlockTagClip

在 Clip 生效期间向 `Agent.BlockAbilitiesWithTag` 添加标签，Clip 结束时移除。

```csharp
// Clip 启用时 → 添加阻止标签
public override void OnEnable()
{
    foreach (var tag in Tag.Tags)
        Ability.AbilityRunner.BlockAbilitiesWithTag.Add(tag);
}

// Clip 禁用时 → 移除阻止标签
public override void OnDisable()
{
    foreach (var tag in Tag.Tags)
        Ability.AbilityRunner.BlockAbilitiesWithTag.Remove(tag);
}
```

**使用场景**：攻击动画的前摇阶段阻止移动，后摇阶段解除阻止

#### ModifyCanBuffAbilitiesTagClip

在 Clip 生效期间向 `Agent.CanBufferAbilitiesTag` 添加标签，Clip 结束时移除。

```csharp
// Clip 启用时 → 允许缓冲这些标签的技能
public override void OnEnable()
{
    foreach (var tag in Tag.Tags)
        Ability.AbilityRunner.CanBufferAbilitiesTag.Add(tag);
}

// Clip 禁用时 → 取消缓冲许可
public override void OnDisable()
{
    foreach (var tag in Tag.Tags)
        Ability.AbilityRunner.CanBufferAbilitiesTag.Remove(tag);
}
```

**使用场景**：攻击动画的特定帧区间内允许输入缓冲下一个攻击指令

---

## 9. 编辑器工具

### 9.1 标签编辑窗口（GameplayTagEditWindow）

**菜单路径**: `Tools > Gameplay > GameplayTagEditWindow`

功能：
- **添加标签**：输入框输入完整路径（如 `Mugen.Attack.Punch`），自动创建缺失的父层级
- **删除标签**：右键菜单 → `Remove Tag`（含子标签）或 `Remove Tag without Children`（仅删当前节点）
- **重命名**：双击标签名称进入重命名模式
- **移动**：右键菜单 → `Move Tag` → 点击目标父节点；或 `Move to Root` 移到根层级
- **展开/折叠**：点击文件夹图标
- **引用检查**：删除前显示引用此标签的所有对象，支持跳转到场景中的引用对象

### 9.2 标签选择窗口（GameplayTagSelectWindow）

当在 Inspector 中点击 `GameplayTagContainer` 的选择按钮时弹出。

功能：
- 树形显示所有标签，用 Toggle 勾选/取消
- 支持 Single/Multi 模式
- 全部展开/折叠
- 清空已选标签
- 一键打开标签编辑窗口
- Missing 标签提示和修复

### 9.3 容器 PropertyDrawer

`GameplayTagContainerDrawer` 为 `GameplayTagContainer` 类型提供自定义 Inspector 绘制：

- 显示字段名称和已选标签列表
- 提供"选择"按钮打开 `GameplayTagSelectWindow`
- 标签变更后自动刷新显示

---

## 10. 当前项目已定义的标签

从 `GameplayTagData.asset` 中提取的当前标签树：

```
Mugen
├── Mugen.Jump
└── Mugen.Stand
    ├── Mugen.Stand.Idle
    ├── Mugen.Stand.Run
    └── Mugen.Stand.Walk
```

| 标签名 | 引用情况 |
|--------|---------|
| `Mugen` | 无引用 |
| `Mugen.Jump` | Ability Tags（某技能资产） |
| `Mugen.Stand` | Active Tags（某技能资产） |
| `Mugen.Stand.Idle` | Ability Tags（某技能资产） |
| `Mugen.Stand.Run` | 无引用 |
| `Mugen.Stand.Walk` | 无引用 |

---

## 11. 最佳实践

### 标签命名规范

```
{系统}.{分类}.{具体标签}

示例：
Mugen.Stand.Idle      ← 角色站立空闲状态
Mugen.Stand.Walk      ← 角色站立行走状态
Mugen.Jump            ← 角色跳跃状态
Buff.Speed            ← 速度增益
State.Stunned         ← 眩晕状态
```

### 使用建议

1. **优先通过编辑器添加标签**：使用 `GameplayTagEditWindow` 管理标签定义，避免手动修改 `GameplayTagData.asset`

2. **运行时动态标签用 `AddTagRuntime`**：运行时添加的标签不需要在编辑器中预定义，但建议预定义以保持一致性

3. **利用 Multi 属性控制互斥**：
   - 同级标签设为 `Multi = false`（默认）：选择一个子标签会自动取消同级其他选择
   - 设为 `Multi = true`：允许同时选择多个同级子标签

4. **删除前检查引用**：编辑器会自动显示引用信息，确认无影响后再删除

5. **利用层级匹配简化查询**：
   ```csharp
   // 检查是否在任何"站立"子状态中
   container.IsChildOf("Mugen.Stand");  // Idle、Walk、Run 都会匹配
   ```

6. **OnValueChanged 回调**：在容器值变化时触发 UI 刷新或逻辑更新
   ```csharp
   container.OnValueChanged += () => { /* 刷新UI或状态 */ };
   ```
