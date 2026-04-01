using System;
using UnityEngine;
using TreeDesigner;
using Taco.Gameplay;
using Animancer;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AcceptableNodePaths("Character", "AnimancerAbility")]
public partial class AnimancerAbility : Ability
{
    [NonSerialized]
    public AnimancerComponent AnimancerComponent;

    protected override void OnStartAbility()
    {
        base.OnStartAbility();
        if (AnimancerComponent == null && Character != null)
        {
            AnimancerComponent = Character.GetComponent<AnimancerComponent>();
        }
    }
}

#if UNITY_EDITOR
public partial class AnimancerAbility
{
    [MenuItem("Assets/Create/Taco/AnimancerAbility/AnimancerAbility")]
    public static void CreateAnimancerAbility()
    {
        AnimancerAbility tree = CreateInstance<AnimancerAbility>();
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

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New AnimancerAbility.asset");
        AssetDatabase.CreateAsset(tree, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = tree;
    }

    public override void CreateInternalExposedProperties()
    {
        base.CreateInternalExposedProperties();
    }
}
#endif
