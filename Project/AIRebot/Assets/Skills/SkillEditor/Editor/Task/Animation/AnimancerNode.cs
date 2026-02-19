using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillEditor.Data;
using Animancer;

namespace SkillEditor.Editor
{
    public class AnimancerNode : SkillNodeBase<AnimancerNodeData>
    {
        protected override string GetNodeTitle() => "Animancer¶¯»­";
        protected override float GetNodeWidth() => 1020;

        protected override void CreateContent() {
        }

        public AnimancerNode(Vector2 position) : base(NodeType.Animancer, position) { }
    }
}
