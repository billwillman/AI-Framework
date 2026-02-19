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

        // ============ 动画配置 ============

        /// <summary>
        /// 动画名称
        /// </summary>
        public string animationName = "";

        /// <summary>
        /// 是否循环播放动画
        /// </summary>
        public bool isAnimationLooping = false;
    }
}
