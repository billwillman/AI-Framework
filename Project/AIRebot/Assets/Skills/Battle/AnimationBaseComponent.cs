using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillEditor.Data;

namespace SkillEditor.Runtime
{

    public abstract class AnimationBaseComponent : MonoBehaviour
    {
        [Header("引用")]
        [SerializeField] protected AbilitySystemComponent _asc;

        // 缓存的标签（通过 GameplayTagLibrary 引用，tag 重命名/删除时编译期即可发现）
        private GameplayTag _cachedStunTag = GameplayTagLibrary.Buff_DeBuff_Stun;

        // 当前状态
        [HideInInspector]
        public bool _isStunned;

        /// <summary>
        /// 标签添加时的回调
        /// </summary>
        private void TagAdded(GameplayTag tag) {
            if (!_isStunned && tag == _cachedStunTag) {
                _isStunned = true;
                OnTagAdd(tag, true);
            } else
                OnTagAdd(tag, false);
        }

        /// <summary>
        /// 标签移除时的回调
        /// </summary>
        private void TagRemoved(GameplayTag tag) {
            if (_isStunned && tag == _cachedStunTag && !_asc.OwnedTags.HasTag(_cachedStunTag)) {
                _isStunned = false;
                OnTagRemoved(tag, true);
            } else
                OnTagRemoved(tag, false);
        }

        protected virtual void OnTagAdd(GameplayTag tag, bool isStunnedChg = false) {}
        protected virtual void OnTagRemoved(GameplayTag tag, bool isStunnedChg = false) { }


        protected virtual void Awake() {
            // 自动获取组件
            if (_asc == null) {
                _asc = GetComponent<Unit>().ownerASC;
            }
        }

        protected virtual void OnEnable() {
            if (_asc?.OwnedTags == null) return;

            // 注册标签事件监听
            _asc.OwnedTags.OnTagAdded += TagAdded;
            _asc.OwnedTags.OnTagRemoved += TagRemoved;
        }

        protected virtual void OnDisable() {
            if (_asc?.OwnedTags == null) return;

            // 取消注册
            _asc.OwnedTags.OnTagAdded -= TagAdded;
            _asc.OwnedTags.OnTagRemoved -= TagRemoved;
        }

        public virtual void PlayAnimation(string name, bool loop) { }
    }
}
