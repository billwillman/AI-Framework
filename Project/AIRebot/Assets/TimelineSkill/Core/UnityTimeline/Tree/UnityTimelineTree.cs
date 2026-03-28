using UnityEngine;
using System;
using TreeDesigner;
using Taco.Timeline;

namespace UnityTimeline
{
    /// <summary>
    /// 用于驱动/控制 Unity 官方 PlayableDirector 的行为树。
    /// 作为 TreeClip 嵌套在 Taco.Timeline 中运行。
    /// </summary>
    [AcceptableNodePaths("UnityTimeline")]
    public class UnityTimelineTree : TimelineRunningTree
    {
        [NonSerialized]
        private IDirectorController m_DirectorController;

        /// <summary>
        /// 获取 PlayableDirector 的控制接口。
        /// </summary>
        public IDirectorController DirectorController => m_DirectorController;

        /// <summary>
        /// 设置 PlayableDirector 控制接口。
        /// </summary>
        public void SetDirectorController(IDirectorController controller)
        {
            m_DirectorController = controller;
        }

        public override void DisposeTree()
        {
            base.DisposeTree();
            m_DirectorController = null;
        }
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Assets/Create/Taco/Tree/UnityTimelineTree")]
        public static void CreateUnityTimelineTree()
        {
            UnityTimelineTree tree = CreateInstance<UnityTimelineTree>();
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
            string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New UnityTimelineTree.asset");
            UnityEditor.AssetDatabase.CreateAsset(tree, assetPathAndName);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.Selection.activeObject = tree;
        }
#endif
    }
}
