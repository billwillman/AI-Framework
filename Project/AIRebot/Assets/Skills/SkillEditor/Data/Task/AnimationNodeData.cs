using System;
using System.Collections.Generic;
using Spine.Unity;


namespace SkillEditor.Data
{
    /// <summary>
    /// 动画节点数据 - 用于播放动画并在时间轴上触发效果和Cue
    /// </summary>
    [Serializable]
    public class AnimationNodeData : AnimationNodeBaseData
    {
        // ============ Spine资源 ============

        /// <summary>
        /// Spine骨骼数据资源
        /// </summary>
        public SkeletonDataAsset skeletonDataAsset;
    }
}
