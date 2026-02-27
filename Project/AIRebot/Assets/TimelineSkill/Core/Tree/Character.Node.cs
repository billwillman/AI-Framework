using UnityEngine;
using UnityEngine.Playables;
using TreeDesigner;
using Taco.Timeline;

public abstract class CharacterActionNode : ActionNode
{
    public PlatformCharacter Character => (Owner as ICharacterDerivative)?.Character;

    protected override void OnStart()
    {
        if (!Character)
        {
            return;
        }
        else
        {
            base.OnStart();
        }
    }
}

public abstract class CharacterValueNode : ValueNode
{
    public PlatformCharacter Character => (Owner as ICharacterDerivative)?.Character;

    protected sealed override void OutputValue()
    {
        base.OutputValue();
        if (Character)
            DoOuput();
    }
    public abstract void DoOuput();
}


#region Movement
[NodeName("EnableRootmotion")]
[NodePath("Character/Action/EnableRootmotion")]
public class EnableRootmotionNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Enable")]
    BoolPropertyPort m_Enable = new BoolPropertyPort();

    protected override void DoAction()
    {
        Character.EnableRootmotion(m_Enable.Value);
    }
}

[NodeName("UseGravity")]
[NodePath("Character/Action/UseGravity")]
public class UseGravityNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "UseGravity")]
    BoolPropertyPort m_UseGravity = new BoolPropertyPort();

    protected override void DoAction()
    {
        Character.EnableGravity(m_UseGravity.Value);
    }
}

[NodeName("AddForce")]
[NodePath("Character/Action/AddForce")]
public class AddForceNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Force")]
    Vector3PropertyPort m_Force = new Vector3PropertyPort();
    [SerializeField, ShowInPanel]
    ForceMode m_ForceMode;

    protected override void DoAction()
    {
        Character.AddForce(m_Force.Value, m_ForceMode);
    }
}

[NodeName("ClearVelocity")]
[NodePath("Character/Action/ClearVelocity")]
public class ClearVelocityNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "X")]
    BoolPropertyPort m_X = new BoolPropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "Y")]
    BoolPropertyPort m_Y = new BoolPropertyPort();

    protected override void DoAction()
    {
        float targetX = Character.GetVelocity().x;
        float targetY = Character.GetVelocity().y;
        if (m_X.Value)
            targetX = 0;
        if (m_Y.Value)
            targetY = 0;
        Character.SetVelocity(new Vector3(targetX, targetY, Character.GetVelocity().z));
    }
}

[NodeName("LaunchCharacter")]
[NodePath("Character/Action/LaunchCharacter")]
public class LaunchCharacterNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Force")]
    Vector3PropertyPort m_Velocity = new Vector3PropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "OverrideVertical")]
    BoolPropertyPort m_OverrideVertical = new BoolPropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "OverrideLateral")]
    BoolPropertyPort m_OverrideLateral = new BoolPropertyPort();

    protected override void DoAction()
    {
        Character.PauseGroundConstraint();
        Character.LaunchCharacter(m_Velocity.Value, m_OverrideVertical.Value, m_OverrideLateral.Value);
    }
}

[NodeName("AddAccelerationConrol")]
[NodePath("Character/Action/AddAccelerationConrol")]
public class AddAccelerationConrolNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "AccelerationConrol")]
    FloatPropertyPort m_AccelerationConrol = new FloatPropertyPort();

    protected override void DoAction()
    {
        Character.AddAccelerationConrol(m_AccelerationConrol.Value);
    }
}

[NodeName("RotateTo")]
[NodePath("Character/Action/RotateTo")]
public class RotateToNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Direction")]
    IntPropertyPort m_Direction = new IntPropertyPort();

    protected override void DoAction()
    {
        Character.RotateTo(m_Direction.Value);
    }
}

[NodeName("StopOnCharacter")]
[NodePath("Character/Action/StopOnCharacter")]
public class StopOnCharacterNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "StopOnCharacter")]
    BoolPropertyPort m_StopOnCharacter = new BoolPropertyPort();

    protected override void DoAction()
    {
        Character.StopOnCharacter = m_StopOnCharacter.Value;
    }
}

[NodeName("AddExternalForce")]
[NodePath("Character/Action/AddExternalForce")]
public class AddExternalForceNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Force")]
    Vector3PropertyPort m_Force = new Vector3PropertyPort();

    protected override void DoAction()
    {
        Character.AddExternalForce(m_Force.Value);
    }
}

[NodeName("ClearExternalForce")]
[NodePath("Character/Action/ClearExternalForce")]
public class ClearExternalForceNode : CharacterActionNode
{
    protected override void DoAction()
    {
        Character.ClearExternalForce();
    }
}

[NodeName("GetMoveInput")]
[NodePath("Character/Value/GetMoveInput")]
public class GetMoveInputNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "MoveInput"), TreeDesigner.ReadOnly]
    protected Vector2PropertyPort m_MoveInput = new Vector2PropertyPort();

    public override void DoOuput()
    {
        m_MoveInput.Value = Character.MovementInput;
    }
}

[NodeName("GetDirectionInput")]
[NodePath("Character/Value/GetDirectionInput")]
public class GetDirectionInputNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "DirectionInput"), TreeDesigner.ReadOnly]
    protected IntPropertyPort m_DirectionInput = new IntPropertyPort();

    public override void DoOuput()
    {
        if(Character.MovementInput.x == 0)
        {
            m_DirectionInput.Value = Character.Direction;
        }
        else
        {
            m_DirectionInput.Value = Character.MovementInput.x > 0 ? 1 : -1;
        }
    }
}

[NodeName("GetDirection")]
[NodePath("Character/Value/GetDirection")]
public class GetDirectionNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "Direction"), TreeDesigner.ReadOnly]
    protected IntPropertyPort m_Direction = new IntPropertyPort();

    public override void DoOuput()
    {
        m_Direction.Value = Character.Direction;
    }
}

[NodeName("GetGrounded")]
[NodePath("Character/Value/GetGrounded")]
public class GetGroundedNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "Grounded"), TreeDesigner.ReadOnly]
    protected BoolPropertyPort m_Grounded = new BoolPropertyPort();

    public override void DoOuput()
    {
        m_Grounded.Value = Character.IsGrounded();
    }
}

[NodeName("GetGroundDistance")]
[NodePath("Character/Value/GetGroundDistance")]
public class GetGroundDistanceNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "GroundDistance"), TreeDesigner.ReadOnly]
    protected FloatPropertyPort m_GroundDistance = new FloatPropertyPort();

    public override void DoOuput()
    {
        m_GroundDistance.Value = Character.GroundDistance;
    }
}

[NodeName("OnGrounded")]
[NodePath("Character/Entry/OnGrounded")]
public class OnGroundedNode : EnterNode
{
    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;

        if (Owner is ICharacterDerivative characterDerivative)
        {
            characterDerivative.Character.Landed += OnTrigger;
        }
    }
    public override void Dispose()
    {
        if (Owner.User != null)
        {
            if (Owner is ICharacterDerivative characterDerivative)
            {
                characterDerivative.Character.Landed -= OnTrigger;
            }
        }
        base.Dispose();
    }

    void OnTrigger()
    {
        UpdateNode();
    }


#if UNITY_EDITOR

    public override NodeCapabilities Capabilities => base.Capabilities | NodeCapabilities.Deletable | NodeCapabilities.Copiable;
    public override bool Single => true;
    protected override string GetNodeName()
    {
        return "OnGrounded";
    }
#endif
}
#endregion

#region Input

[NodeName("OnInput")]
[NodePath("Character/Entry/OnInput")]
public class OnInputNode : EnterNode
{
    [SerializeField, ShowInPanel]
    protected string m_InputName;
    [SerializeField, ShowInPanel]
    protected InputPhase m_InputPhase;

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;

        if (Owner is ICharacterDerivative characterDerivative)
        {
            characterDerivative.Character.BindInput(m_InputName, m_InputPhase, OnTrigger);
        }
    }
    public override void Dispose()
    {
        if (Owner.User != null)
        {
            if (Owner is ICharacterDerivative characterDerivative)
            {
                characterDerivative.Character.UnbindInput(m_InputName, m_InputPhase, OnTrigger);
            }
        }
        base.Dispose();
    }

    void OnTrigger(UnityEngine.InputSystem.InputAction.CallbackContext callbackContext)
    {
        UpdateNode();
    }


#if UNITY_EDITOR

    public override NodeCapabilities Capabilities => base.Capabilities | NodeCapabilities.Deletable | NodeCapabilities.Copiable;
    public override bool Single => true;
    protected override string GetNodeName()
    {
        return "OnInput";
    }
#endif
}

[NodeName("GetInputPhase")]
[NodePath("Character/Value/GetInputPhase")]
public class GetInputPhaseNode : CharacterValueNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "InputName")]
    protected StringPropertyPort m_InputName = new StringPropertyPort();
    [SerializeField, ShowInPanel]
    protected InputPhase m_InputPhase;
    [SerializeField, PropertyPort(PortDirection.Output, "Result"), TreeDesigner.ReadOnly]
    protected BoolPropertyPort m_Result = new BoolPropertyPort();

    public override void DoOuput()
    {
        m_Result.Value = Character.GetInputPhase(m_InputName.Value, m_InputPhase);
    }
}
#endregion

#region Animation
[NodeName("PlayTimeline")]
[NodePath("Character/Action/PlayTimeline")]
[Output("Output", PortCapacity.Single)]
public class PlayTimelineNode : CharacterActionNode
{
    [SerializeField]
    protected string m_OutputEdgeGUID;
    public string OutputGUID => m_OutputEdgeGUID;

    [System.NonSerialized]
    protected RunnableNode m_Child;
    public RunnableNode Child => m_Child;

    [SerializeField, ShowInPanel]
    Timeline TimelinePrefab;

    [SerializeField, PropertyPort(PortDirection.Output, "Timeline"), TreeDesigner.ReadOnly]
    protected TimelinePropertyPort m_Timeline = new TimelinePropertyPort();

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

        if (m_Timeline.Value)
        {
            m_Timeline.Value.OnDone -= OnDone;
        }
    }
    protected override State OnUpdate()
    {
        return State.Running;
    }

    protected override void DoAction()
    {
        if (TimelinePrefab)
        {
            m_Timeline.Value = Object.Instantiate(TimelinePrefab);
            m_Timeline.Value.OnDone += () =>
            {
                Character.TimelinePlayer.RemoveTimeline(m_Timeline.Value);
                Object.Destroy(m_Timeline.Value);
            };
            m_Timeline.Value.OnDone += OnDone;
            if (m_Timeline.Value is AbilityTimeline abilityTimeline && Owner is Ability ability)
            {
                abilityTimeline.Character = Character;
                abilityTimeline.Ability = ability;
            }
            Character.TimelinePlayer.AddTimeline(m_Timeline.Value);
        }
    }

    void OnDone()
    {
        if (m_Timeline.Value)
        {
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

[NodeName("StopTimeline")]
[NodePath("Character/Action/StopTimeline")]
public class StopTimelineNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Timeline"), TreeDesigner.ReadOnly]
    protected TimelinePropertyPort m_Timeline = new TimelinePropertyPort();

    protected override void DoAction()
    {
        if (m_Timeline.Value)
        {
            Character.TimelinePlayer.RemoveTimeline(m_Timeline.Value);
        }
    }
}


[NodeName("SetStateTime")]
[NodePath("Character/Action/SetStateTime")]
public class SetStateTimeNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "StateName")]
    StringPropertyPort m_StateName = new StringPropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "StateTime")]
    FloatPropertyPort m_StateTime = new FloatPropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "StateLayer")]
    IntPropertyPort m_StateLayer = new IntPropertyPort();

    protected override void DoAction()
    {
        Character.TimelinePlayer.SetStateTime(m_StateName.Value, m_StateTime.Value, m_StateLayer.Value);
    }
}

[NodeName("SetCtrlPlayState")]
[NodePath("Character/Action/SetCtrlPlayState")]
public class SetCtrlPlayStateNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "State")]
    BoolPropertyPort m_PlayState = new BoolPropertyPort();

    protected override void DoAction()
    {
        if (m_PlayState.Value)
        {
            Character.TimelinePlayer.AnimationRootPlayable.GetInput(0).Play();
        }
        else
        {
            Character.TimelinePlayer.AnimationRootPlayable.GetInput(0).Pause();
        }
    }
}

[NodeName("AddFrameRate")]
[NodePath("Character/Action/AddFrameRate")]
public class AddFrameRateNode : CharacterActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "State")]
    IntPropertyPort m_DeltaFrameRate = new IntPropertyPort();

    protected override void DoAction()
    {
        Character.TimelinePlayer.FrameRate += m_DeltaFrameRate.Value;
    }
}
#endregion

#region Ability
[NodeName("StartAbility")]
[NodePath("Character/Action/StartAbility")]
public class StartAbilityNode : CharacterActionNode
{
    public enum TargetType { Self, Other }

    [SerializeField, EnumMenu("TargetType", "OnNodeChangedCallback")]
    protected TargetType m_TargetType;

    [SerializeField, PropertyPort(PortDirection.Input, "Ability"), TreeDesigner.ShowIf("m_TargetType", TargetType.Other)]
    protected StringPropertyPort m_AbilityName = new StringPropertyPort();

    protected override void DoAction()
    {
        switch (m_TargetType)
        {
            case TargetType.Self:
                if (Owner is Ability ability)
                {
                    Character.AbilityRunner.TryStartAbility(ability);
                }
                break;
            case TargetType.Other:

                break;
        }
    }
}

[NodeName("StopAbility")]
[NodePath("Character/Action/StopAbility")]
public class StopAbilityNode : CharacterActionNode
{
    public enum TargetType { Self, Other }

    [SerializeField, EnumMenu("TargetType", "OnNodeChangedCallback")]
    protected TargetType m_TargetType;

    [SerializeField, PropertyPort(PortDirection.Input, "Ability"), TreeDesigner.ShowIf("m_TargetType", TargetType.Other)]
    protected StringPropertyPort m_AbilityName = new StringPropertyPort();

    protected override void DoAction()
    {
        switch (m_TargetType)
        {
            case TargetType.Self:
                if (Owner is Ability ability)
                {
                    Character.AbilityRunner.TryStopAbility(ability);
                }
                break;
            case TargetType.Other:

                break;
        }
    }
}

[NodeName("OnAbilityStop")]
[NodePath("Character/Entry/OnAbilityStop")]
public class OnAbilityStopNode : EnterNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "Ability"), TreeDesigner.ReadOnly]
    protected AbilityPropertyPort m_Ability = new AbilityPropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;

        if (Owner is Ability ability)
        {
            ability.AbilityRunner.OnAbilitySop += OnTrigger;
        }
        else if (Owner is ICharacterDerivative characterDerivative)
        {
            characterDerivative.Character.AbilityRunner.OnAbilitySop += OnTrigger;
        }
    }
    public override void Dispose()
    {
        if (Owner.User != null)
        {
            if (Owner is Ability ability)
            {
                ability.AbilityRunner.OnAbilitySop -= OnTrigger;
            }
            else if (Owner is ICharacterDerivative characterDerivative)
            {
                characterDerivative.Character.AbilityRunner.OnAbilitySop -= OnTrigger;
            }
        }
        base.Dispose();
    }

    void OnTrigger(Ability ability)
    {
        m_Ability.Value = ability;
        UpdateNode();
    }


#if UNITY_EDITOR

    public override NodeCapabilities Capabilities => base.Capabilities | NodeCapabilities.Deletable | NodeCapabilities.Copiable;

    protected override string GetNodeName()
    {
        return "OnAbilityStop";
    }
#endif
}

[NodeName("GetSelf")]
[NodePath("Ability/Value/GetSelf")]
public class AbilityGetSelf : ValueNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "Ability"), TreeDesigner.ReadOnly]
    protected AbilityPropertyPort m_Ability = new AbilityPropertyPort();

    protected override void OutputValue()
    {
        base.OutputValue();
        m_Ability.Value = Owner as Ability;
    }
}
#endregion

#region Utility
[NodeName("ShowText")]
[NodePath("Base/Action/ShowText")]
public class ShowTextNode : ActionNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Text")]
    StringPropertyPort m_Text = new StringPropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "Text")]
    FloatPropertyPort m_Duration = new FloatPropertyPort();
    [SerializeField, PropertyPort(PortDirection.Input, "Text")]
    FloatPropertyPort m_EaseOut = new FloatPropertyPort();

    protected override void DoAction()
    {
        ShowDebugManager.Instance.Show(m_Text.Value, m_Duration.Value, m_EaseOut.Value);
    }
}
#endregion