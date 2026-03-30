# Unity Timeline 支持 RootMotion 详解

## 一、Timeline 默认不支持 Root Motion 的原因

Unity Timeline 默认播放动画时**不会应用 Root Motion**，因为 Timeline 的 Animation Track 会直接控制物体的位置/旋转，覆盖了 Animator 的 Root Motion 行为。

---

## 二、让 Timeline 支持 Root Motion 的方案

### 方案一：使用 Animation Track 的内置选项（推荐）

在 Timeline 的 **Animation Track** 上有相关设置：

1. 选中 Timeline 中的 **Animation Track**
2. 在 Inspector 面板中找到 **Track Offsets** 设置
3. 将其设置为 **Apply Scene Offsets**（应用场景偏移）
4. 确保绑定的 Animator 组件上 **Apply Root Motion** 已勾选

### 方案二：自定义 Playable 实现 Root Motion

如果内置选项不满足需求，可以编写自定义 PlayableBehaviour 来处理 Root Motion：

```csharp
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class RootMotionPlayable : PlayableBehaviour
{
    private Animator m_Animator;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_Animator = playerData as Animator;
        if (m_Animator == null) return;

        m_Animator.applyRootMotion = true;
    }
}
```

### 方案三：通过脚本在 PlayableDirector 播放时强制启用

最简单的做法，在 `PlayableDirector` 播放期间强制保持 Root Motion：

```csharp
using UnityEngine;
using UnityEngine.Playables;

public class TimelineRootMotionEnabler : MonoBehaviour
{
    public PlayableDirector director;
    public Animator animator;

    void OnEnable()
    {
        if (director != null)
        {
            director.played += OnPlayed;
            director.stopped += OnStopped;
        }
    }

    void OnDisable()
    {
        if (director != null)
        {
            director.played -= OnPlayed;
            director.stopped -= OnStopped;
        }
    }

    void OnPlayed(PlayableDirector pd)
    {
        if (animator != null)
            animator.applyRootMotion = true;
    }

    void OnStopped(PlayableDirector pd)
    {
        // 根据需求决定是否关闭
    }
}
```

---

## 三、Animation Track 的 Track Offsets 详解

Track Offsets 决定了动画播放时**如何计算物体的位置和旋转**，共有以下几种模式：

### 1. Auto（自动）

- **行为**：Timeline 根据上下文自动选择使用哪种模式
- **规则**：
  - 如果 Track 上**有手动设置过位置/旋转偏移**，则等同于 `Apply Transform Offsets`
  - 如果**没有设置过偏移**，则等同于 `Apply Scene Offsets`
- **适用场景**：不确定该用哪个时的默认选项

### 2. Apply Transform Offsets（应用变换偏移）

- **行为**：Track **自己管理**物体的位置和旋转，动画播放时物体会被移动到 **Track 定义的偏移位置**
- **具体表现**：
  - 物体的位置/旋转由 Track 上设置的 **Position / Rotation Offset** 决定
  - 动画从 Track 指定的偏移位置开始播放
  - **Root Motion 被覆盖**，因为位置完全由 Track 控制
- **适用场景**：需要精确控制角色在某个固定位置播放动画（如过场动画中角色站在特定位置）

```
播放位置 = Track 偏移值 + 动画内部位移
         （忽略物体在场景中的实际位置）
```

### 3. Apply Scene Offsets（应用场景偏移）

- **行为**：以物体**在场景中当前的 Transform** 作为起始点来播放动画
- **具体表现**：
  - 动画从物体**当前所在位置**开始
  - **Root Motion 可以正常生效**
  - 如果物体在播放前被移动到了新位置，动画就从新位置开始
- **适用场景**：需要 Root Motion 驱动移动的场景，或角色位置不固定的情况

```
播放位置 = 物体当前场景位置 + Root Motion 位移
```

### 对比总结

| 特性 | Apply Transform Offsets | Apply Scene Offsets | Auto |
|------|------------------------|--------------------|----|
| **位置基准** | Track 上设定的偏移值 | 物体当前场景位置 | 自动判断 |
| **Root Motion** | ❌ 被覆盖 | ✅ 正常生效 | 取决于实际模式 |
| **物体会跳到指定位置** | ✅ 是 | ❌ 不会 | 取决于实际模式 |
| **适合固定位置演出** | ✅ | ❌ | — |
| **适合动态位置/移动** | ❌ | ✅ | — |

### 选择建议

- **过场动画（Cutscene）** 中角色需要站在精确位置 → **Apply Transform Offsets**
- **需要 Root Motion** 或角色位置不固定 → **Apply Scene Offsets**
- **不确定** → 先用 **Auto**，出问题再手动切换

---

## 四、常见问题排查

1. **角色不移动** → 检查 Animator 组件的 `Apply Root Motion` 是否勾选
2. **位置跳回原点** → Animation Track 的 `Track Offsets` 可能设置为 `Apply Transform Offsets`，改为 `Apply Scene Offsets`
3. **多个 Clip 之间位置不连续** → 确保 Clip 之间的 `Clip Transform Offsets` 设置为 `Auto`，让后一个 Clip 从前一个 Clip 结束位置继续
4. **Root Motion 动画本身没有位移数据** → 检查 FBX 导入设置中动画是否包含 Root Motion 曲线（在模型导入的 Animation Tab 中查看）
