using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animancer;

namespace SkillEditor.Data
{
    [Serializable]
    public class AnimancerNodeData : AnimationNodeBaseData
    {
        public Animancer.AnimancerTransitionAssetBase Data = null;
    }
}
