using UnityEngine;
using TreeDesigner;

public class Ability_Locomotion : Ability
{
    public override void InitTree(object user)
    {
        base.InitTree(user);
        Character.AbilityRunner.TryStartAbility(this);
    }
    public override void UpdateAbility(float deltaTime)
    {
        base.UpdateAbility(deltaTime);
        if (Character.MovementInput.x != 0)
        {
            int targetDirection = Character.MovementInput.x > 0 ? 1 : -1;
            if (Character.RootMotionWeight <= 0.1f)
            {
                Character.RotateTo(targetDirection);
            }
        }
    }
    public override void InactiveUpdate()
    {
        base.InactiveUpdate();
        if (Character.MovementInput.x != 0)
        {
            AbilityRunner.TryStartAbility(this);
        }
    }
    protected override void OnStartAbility()
    {
        base.OnStartAbility();
    }
    protected override void OnStopAbility()
    {
        base.OnStopAbility();
    }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Taco/Ability/Ability_Locomotion")]
    public static void CreateAbility_Locomotion()
    {
        Ability_Locomotion tree = CreateInstance<Ability_Locomotion>();
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
#endif
}