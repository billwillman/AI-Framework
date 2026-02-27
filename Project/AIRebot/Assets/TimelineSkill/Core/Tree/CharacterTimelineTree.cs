using UnityEngine;
using TreeDesigner;
using Taco.Timeline;

[AcceptableNodePaths("Timeline", "Character")]
public class CharacterTimelineTree : TimelineRunningTree, ICharacterDerivative
{
    public PlatformCharacter Character => (TimelinePlayer as PlatformTimelinePlayer)?.PlatformCharacter;

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Assets/Create/Taco/Tree/CharacterTimelineTree")]
    public static void CreateCharacterTimelineTree()
    {
        CharacterTimelineTree tree = CreateInstance<CharacterTimelineTree>();
        tree.RootGUID = tree.CreateNode(typeof(RootNode)).GUID;

        var OnEnable = tree.CreateNode(typeof(TimelineEnterNode)) as TimelineEnterNode;
        OnEnable.EnterType = TimelineEnterNode.NodeEnterType.OnEnable;
        OnEnable.Position = new Vector2(0, 200);
        tree.OnEnableGUID = OnEnable.GUID;

        var OnDisable = tree.CreateNode(typeof(TimelineEnterNode)) as TimelineEnterNode;
        OnDisable.EnterType = TimelineEnterNode.NodeEnterType.OnDisable;
        OnDisable.Position = new Vector2(0, 400);
        tree.OnDisableGUID = OnDisable.GUID;

        var OnDestroy = tree.CreateNode(typeof(TimelineEnterNode)) as TimelineEnterNode;
        OnDestroy.EnterType = TimelineEnterNode.NodeEnterType.OnDestroy;
        OnDestroy.Position = new Vector2(0, 600);
        tree.OnDestroyGUID = OnDestroy.GUID;

        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New PlatformTimelineTree.asset");
        UnityEditor.AssetDatabase.CreateAsset(tree, assetPathAndName);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        UnityEditor.Selection.activeObject = tree;
    }
#endif
}