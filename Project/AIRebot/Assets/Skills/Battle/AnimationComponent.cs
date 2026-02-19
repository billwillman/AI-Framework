using UnityEngine;
using SkillEditor.Data;
using Spine;
using Spine.Unity;

namespace SkillEditor.Runtime
{
    /// <summary>
    /// 控制效果动画处理器 - 监听标签事件，控制动画播放
    /// 独立于 Buff，通过标签系统驱动
    ///
    /// 使用方式：挂载到角色上，配置 ASC 引用
    /// </summary>
    public class AnimationComponent : AnimationBaseComponent
    {
        
         private SkeletonAnimation _animation;

        protected override void Awake() {
            base.Awake();
            if (_animation == null)
                _animation = GetComponent<SkeletonAnimation>();
        }

        protected override void OnTagAdd(GameplayTag tag, bool isStunnedChg) {
            if (isStunnedChg) {
                PlayAnimation("Stun", true);
            }
        }

        protected override void OnTagRemoved(GameplayTag tag, bool isStunnedChg) {
            if (isStunnedChg) {
                PlayAnimation("Stand", true);
            }
        }

        public override void PlayAnimation(string name,bool loop)
        {
            // 检查当前是否已经在播放这个动画
            var current = _animation.AnimationState.GetCurrent(0);
            if (current != null && current.Animation.Name == name)
                return;

            _animation.AnimationState.SetAnimation(0,name,loop);
        }


    }
}
