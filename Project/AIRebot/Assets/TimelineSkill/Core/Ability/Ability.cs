using System;
using UnityEngine;
using TreeDesigner;
using Taco.Gameplay;

[AcceptableNodePaths("Character", "Ability")]
public partial class Ability : OneRootTree, ICharacterDerivative
{
    [ShowInInspector]
    public GameplayTagContainer AbilityTags;
    [ShowInInspector]
    public GameplayTagContainer CancelAbilitiesWithTag;
    [ShowInInspector]
    public GameplayTagContainer BlockAbilitiesWithTag;

    [ShowInInspector]
    public GameplayTagContainer ActiveTags;
    [ShowInInspector]
    public GameplayTagContainer RequiredTags;

    [SerializeField]
    protected string m_OnStartGUID;
    public string OnStartGUID { get => m_OnStartGUID; set => m_OnStartGUID = value; }

    [SerializeField]
    protected string m_OnStopGUID;
    public string OnStopGUID { get => m_OnStopGUID; set => m_OnStopGUID = value; }


    public AbilityRunner AbilityRunner { get; private set; }
    public PlatformCharacter Character { get; private set; }

    protected BoolExposedProperty m_Active;
    public bool Active => m_Active.Value;

    protected FloatExposedProperty m_Duration;
    public float Duration => m_Duration.Value;


    protected EnterNode m_OnStart;
    protected EnterNode m_OnStop;

    [NonSerialized]
    public AbilityCanStartNode AbilityCanStart;
    [NonSerialized]
    public OnAbilityCancelNode OnAbilityCancel;


    public override void InitTree(object user)
    {
        AbilityRunner = user as AbilityRunner;
        Character = AbilityRunner.Owner;

        base.InitTree(user);
        if (!string.IsNullOrEmpty(m_OnStartGUID))
            m_OnStart = m_GUIDNodeMap[m_OnStartGUID] as EnterNode;
        if (!string.IsNullOrEmpty(m_OnStopGUID))
            m_OnStop = m_GUIDNodeMap[m_OnStopGUID] as EnterNode;

        m_Active = GetExposedProperty<BoolExposedProperty>("Active");
        m_Duration = GetExposedProperty<FloatExposedProperty>("Duration");

        
    }
    public override void DisposeTree()
    {
        base.DisposeTree();
        m_OnStart = null;
        m_OnStop = null;
    }
    public override void OnReset()
    {
        base.OnReset();
        m_OnStart.ResetNode();
        m_OnStop.ResetNode();
    }
    public override State OnUpdate()
    {
        m_Root.DeltaTime = DeltaTime;
        m_Root.UpdateNode();
        return State.Running;
    }

    public virtual bool CanStart()
    {
        if(AbilityCanStart != null)
            return AbilityCanStart.GetValue();
        else
            return true;
    }
    public virtual void StartAbility()
    {
        m_Active.Value = true;
        m_Duration.Value = 0;
        ResetTree();
        OnStartAbility();
    }
    public virtual void StopAbility()
    {
        m_Active.Value = false;
        OnStopAbility();
        OnStop();
    }
    public virtual void UpdateAbility(float deltaTime)
    {
        m_Duration.Value += deltaTime;
        UpdateTree(deltaTime);
    }
    public virtual void InactiveUpdate() { }

    public virtual void CancelAbility(Ability abilityCancelBy)
    {
        OnAbilityCancel?.Trigger(abilityCancelBy);
    }

    protected virtual void OnStartAbility()
    {
        foreach (var tag in ActiveTags.Tags)
        {
            AbilityRunner.ActiveTags.Add(tag);
        }
        m_OnStart?.UpdateNode();
    }
    protected virtual void OnStopAbility()
    {
        foreach (var tag in ActiveTags.Tags)
        {
            AbilityRunner.ActiveTags.Remove(tag);
        }
        m_OnStop?.UpdateNode();
    }
}

public partial class Ability
{
#if UNITY_EDITOR

    public override bool CheckInit()
    {
        bool dirty = base.CheckInit();
        if (!string.IsNullOrEmpty(m_OnStartGUID))
            m_OnStart = m_GUIDNodeMap[m_OnStartGUID] as EnterNode;
        if (!string.IsNullOrEmpty(m_OnStopGUID))
            m_OnStop = m_GUIDNodeMap[m_OnStopGUID] as EnterNode;
        return dirty;
    }

    [UnityEditor.MenuItem("Assets/Create/Taco/Ability/Ability")]
    public static void CreateAbility()
    {
        Ability tree = CreateInstance<Ability>();
        tree.RootGUID = tree.CreateNode(typeof(RootNode)).GUID;

        var OnEnable = tree.CreateNode(typeof(EnterNode)) as EnterNode;
        OnEnable.NodeName = "OnStart";
        OnEnable.Position = new Vector2(0, 200);
        tree.OnStartGUID = OnEnable.GUID;

        var OnDisable = tree.CreateNode(typeof(EnterNode)) as EnterNode;
        OnDisable.NodeName = "OnStop";
        OnDisable.Position = new Vector2(0, 400);
        tree.OnStopGUID = OnDisable.GUID;

        tree.CreateInternalExposedProperties();

        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New Ability.asset");
        UnityEditor.AssetDatabase.CreateAsset(tree, assetPathAndName);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        UnityEditor.Selection.activeObject = tree;
    }

    public virtual void CreateInternalExposedProperties()
    {
        CreateInternalExposedProperty(typeof(BoolExposedProperty), "Active", false);
        CreateInternalExposedProperty(typeof(FloatExposedProperty), "Duration", false);
    }
#endif
}

[NodeName("AbilityCanStart")]
[NodePath("Ability/Value/AbilityCanStart")]
public class AbilityCanStartNode : ValueNode
{
    [SerializeField, PropertyPort(PortDirection.Input, "Condition")]
    protected BoolPropertyPort m_Condition = new BoolPropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;
        if (Owner is Ability ability)
            ability.AbilityCanStart = this;
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

[NodeName("OnAbilityCancel")]
[NodePath("Ability/Entry/OnAbilityCancel")]
public class OnAbilityCancelNode : EnterNode
{
    [SerializeField, PropertyPort(PortDirection.Output, "Ability"), TreeDesigner.ReadOnly]
    protected AbilityPropertyPort m_Ability = new AbilityPropertyPort();

    public override void Init(BaseTree tree)
    {
        base.Init(tree);
        if (Owner.User == null) return;
        if (Owner is Ability ability)
            ability.OnAbilityCancel = this;
    }

    public void Trigger(Ability ability)
    {
        m_Ability.Value = ability;
        UpdateNode();
    }

#if UNITY_EDITOR

    public override NodeCapabilities Capabilities => base.Capabilities | NodeCapabilities.Deletable | NodeCapabilities.Copiable;

    public override bool Single => true;
    protected override string GetNodeName()
    {
        return "OnAbilityCancel";
    }
#endif
}