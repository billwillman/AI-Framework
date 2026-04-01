using System;
using UnityEngine;
using TreeDesigner;
using Animancer;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class AnimancerAbility
{
    [NonSerialized]
    public AnimancerAbilityCanStartNode AnimancerAbilityCanStart;
    [NonSerialized]
    public OnAnimancerAbilityCancelNode OnAnimancerAbilityCancel;
}

/// <summary>
/// AnimancerAbility 是否可以开始的条件节点
/// </summary>
[NodeName("AnimancerAbilityCanStart")]
[NodePath("AnimancerAbility/Value/AnimancerAbilityCanStart")]
public class AnimancerAbilityCanStartNode : ValueNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Condition")]
    protected BoolPropertyPort m_Condition = new BoolPropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;
        if (Owner is AnimancerAbility animancerAbility)
            animancerAbility.AnimancerAbilityCanStart = this;
    }

    public bool GetValue()
    {
        InputValue();
        return m_Condition.Value;
    }

#if UNITY_EDITOR
    public override NodeCapabilities Capabilities => base.Capabilities | NodeCapabilities.Deletable | NodeCapabilities.Copiable;
    public override bool Single => true;
#endif
}

/// <summary>
/// AnimancerAbility 被取消时触发的事件节点
/// </summary>
[NodeName("OnAnimancerAbilityCancel")]
[NodePath("AnimancerAbility/Entry/OnAnimancerAbilityCancel")]
public class OnAnimancerAbilityCancelNode : EnterNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "Ability"), TreeDesigner.ReadOnly]
    protected AnimancerAbilityPropertyPort m_Ability = new AnimancerAbilityPropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;
        if (Owner is AnimancerAbility animancerAbility)
            animancerAbility.OnAnimancerAbilityCancel = this;
    }

    public void Trigger(AnimancerAbility ability)
    {
        m_Ability.Value = ability;
        UpdateNode();
    }

#if UNITY_EDITOR
    public override NodeCapabilities Capabilities => base.Capabilities | NodeCapabilities.Deletable | NodeCapabilities.Copiable;
    public override bool Single => true;
    protected override string GetNodeName()
    {
        return "OnAnimancerAbilityCancel";
    }
#endif
}

/// <summary>
/// AnimancerAbility 的属性端口
/// </summary>
[Serializable]
public class AnimancerAbilityPropertyPort : PropertyPort<AnimancerAbility>
{
}

/// <summary>
/// 通过 Animancer 播放 PlayableAssetTransitionAsset (Timeline)
/// </summary>
[NodeName("PlayAnimancerTimeline")]
[NodePath("AnimancerAbility/Action/PlayAnimancerTimeline")]
public class PlayAnimancerTimelineNode : CharacterActionNode
{
    [SerializeField]
    protected string m_OutputEdgeGUID;
    public string OutputGUID => m_OutputEdgeGUID;

    [NonSerialized]
    protected RunnableNode m_Child;
    public RunnableNode Child => m_Child;

    [SerializeField, ShowInPanel]
    protected PlayableAssetTransitionAsset m_TimelineAsset;

    [SerializeField, PropertyPort(PortDirection.Input, "FadeDuration")]
    protected FloatPropertyPort m_FadeDuration = new FloatPropertyPort() { Value = 0.25f };

    [SerializeField, PropertyPort(PortDirection.Output, "AnimancerState"), TreeDesigner.ReadOnly]
    protected AnimancerStatePropertyPort m_AnimancerState = new AnimancerStatePropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (!string.IsNullOrEmpty(m_OutputEdgeGUID) && m_Owner.GUIDEdgeMap.ContainsKey(m_OutputEdgeGUID))
            m_Child = m_Owner.GUIDEdgeMap[m_OutputEdgeGUID].EndNode as RunnableNode;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_Child = null;
    }

    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
        m_OutputEdgeGUID = string.Empty;
        m_Child = null;
    }

    public override void ResetNode()
    {
        base.ResetNode();
        m_Child?.ResetNode();
    }

    protected override State OnUpdate()
    {
        return State.Running;
    }

    protected override void DoAction()
    {
        if (Owner is AnimancerAbility animancerAbility && animancerAbility.AnimancerComponent != null)
        {
            AnimancerComponent animancer = animancerAbility.AnimancerComponent;
            
            AnimancerState state = animancer.PlayTimeline(m_TimelineAsset, m_FadeDuration.Value);
            m_AnimancerState.Value = state;

            if (state != null && m_Child != null)
            {
                state.Events.OnEnd -= OnDone;
                state.Events.OnEnd += OnDone;
            }
            else if (m_Child != null)
            {
                m_Child?.UpdateNode();
            }
        }
    }

    void OnDone()
    {
        if (m_AnimancerState.Value != null)
        {
            m_AnimancerState.Value.Events.OnEnd -= OnDone;
            m_Child?.UpdateNode();
        }
    }

#if UNITY_EDITOR
    public override void OnOutputLinked(BaseEdge edge)
    {
        base.OnOutputLinked(edge);
        m_OutputEdgeGUID = edge.GUID;
        m_Child = edge.EndNode as RunnableNode;
    }

    public override void OnOutputUnlinked(BaseEdge edge)
    {
        base.OnOutputUnlinked(edge);
        m_OutputEdgeGUID = string.Empty;
        m_Child = null;
    }
#endif
}

/// <summary>
/// 通过 Animancer 播放 AnimationClip
/// </summary>
[NodeName("PlayAnimancerClip")]
[NodePath("AnimancerAbility/Action/PlayAnimancerClip")]
public class PlayAnimancerClipNode : CharacterActionNode
{
    [SerializeField]
    protected string m_OutputEdgeGUID;
    public string OutputGUID => m_OutputEdgeGUID;

    [NonSerialized]
    protected RunnableNode m_Child;
    public RunnableNode Child => m_Child;

    [SerializeField, ShowInPanel]
    protected AnimationClip m_Clip;

    [SerializeField, PropertyPort(PortDirection.Input, "FadeDuration")]
    protected FloatPropertyPort m_FadeDuration = new FloatPropertyPort() { Value = 0.25f };

    [SerializeField, PropertyPort(PortDirection.Input, "Speed")]
    protected FloatPropertyPort m_Speed = new FloatPropertyPort() { Value = 1f };

    [SerializeField, PropertyPort(PortDirection.Output, "AnimancerState"), TreeDesigner.ReadOnly]
    protected AnimancerStatePropertyPort m_AnimancerState = new AnimancerStatePropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (!string.IsNullOrEmpty(m_OutputEdgeGUID) && m_Owner.GUIDEdgeMap.ContainsKey(m_OutputEdgeGUID))
            m_Child = m_Owner.GUIDEdgeMap[m_OutputEdgeGUID].EndNode as RunnableNode;
    }

    public override void Dispose()
    {
        base.Dispose();
        m_Child = null;
    }

    public override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
        m_OutputEdgeGUID = string.Empty;
        m_Child = null;
    }

    public override void ResetNode()
    {
        base.ResetNode();
        m_Child?.ResetNode();
    }

    protected override State OnUpdate()
    {
        return State.Running;
    }

    protected override void DoAction()
    {
        if (Owner is AnimancerAbility animancerAbility && animancerAbility.AnimancerComponent != null && m_Clip != null)
        {
            AnimancerComponent animancer = animancerAbility.AnimancerComponent;
            AnimancerState state = animancer.Play(m_Clip, m_FadeDuration.Value);
            state.Speed = m_Speed.Value;
            m_AnimancerState.Value = state;

            if (m_Child != null)
            {
                state.Events.OnEnd -= OnDone;
                state.Events.OnEnd += OnDone;
            }
            else
            {
                m_Child?.UpdateNode();
            }
        }
        else if (m_Child != null)
        {
            m_Child?.UpdateNode();
        }
    }

    void OnDone()
    {
        if (m_AnimancerState.Value != null)
        {
            m_AnimancerState.Value.Events.OnEnd -= OnDone;
            m_Child?.UpdateNode();
        }
    }

#if UNITY_EDITOR
    public override void OnOutputLinked(BaseEdge edge)
    {
        base.OnOutputLinked(edge);
        m_OutputEdgeGUID = edge.GUID;
        m_Child = edge.EndNode as RunnableNode;
    }

    public override void OnOutputUnlinked(BaseEdge edge)
    {
        base.OnOutputUnlinked(edge);
        m_OutputEdgeGUID = string.Empty;
        m_Child = null;
    }
#endif
}

/// <summary>
/// 停止 Animancer 动画
/// </summary>
[NodeName("StopAnimancer")]
[NodePath("AnimancerAbility/Action/StopAnimancer")]
public class StopAnimancerNode : CharacterActionNode
{
    protected override State OnUpdate()
    {
        return State.Running;
    }

    protected override void DoAction()
    {
        if (Owner is AnimancerAbility animancerAbility && animancerAbility.AnimancerComponent != null)
        {
            animancerAbility.AnimancerComponent.Stop();
        }
    }
}

/// <summary>
/// 获取 AnimancerState
/// </summary>
[NodeName("GetAnimancerState")]
[NodePath("AnimancerAbility/Value/GetAnimancerState")]
public class GetAnimancerStateNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Key")]
    protected StringPropertyPort m_Key = new StringPropertyPort();

    [SerializeField, PropertyPort(PortDirection.Output, "AnimancerState"), TreeDesigner.ReadOnly]
    protected AnimancerStatePropertyPort m_AnimancerState = new AnimancerStatePropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
    }

    public override void DoOuput()
    {
        if (Owner is AnimancerAbility animancerAbility && animancerAbility.AnimancerComponent != null)
        {
            AnimancerComponent animancer = animancerAbility.AnimancerComponent;
            if (animancer.States.TryGet(m_Key.Value, out AnimancerState state))
            {
                m_AnimancerState.Value = state;
            }
        }
    }

#if UNITY_EDITOR
    public override bool Single => true;
#endif
}

/// <summary>
/// AnimancerState 属性端口
/// </summary>
[Serializable]
public class AnimancerStatePropertyPort : PropertyPort<AnimancerState>
{
}


