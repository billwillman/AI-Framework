using UnityEngine;
using TreeDesigner;

public class Ability_Jump : Ability
{
    protected override void OnStartAbility()
    {
        Character.Jump();
        base.OnStartAbility();
    }
    protected override void OnStopAbility()
    {
        Character.StopJumping();
        base.OnStopAbility();
    }

    public override bool CanStart()
    {
        return base.CanStart() && Character.CanJump();
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
        if (Duration > Character.jumpMaxHoldTime)
            Character.AbilityRunner.TryStopAbility(this);
    }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Taco/Ability/Ability_Jump")]
    public static void CreateAbility_Jump()
    {
        Ability_Jump tree = CreateInstance<Ability_Jump>();
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