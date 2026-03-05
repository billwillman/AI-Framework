using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Taco.Timeline;

namespace Taco.Timeline.Mugen
{
    [TrackGroup("Ability"), ScriptGuid("e75a4b28054c19fc48904a387acd1feb"), Color(165, 032, 025)]
    public class NewMugenImageAnimationTrack : CharacterTrack
    {
#if UNITY_EDITOR
        // public override string Name => "ImageAnimationTrack";
        public override Type ClipType => typeof(NewMugenImageAnimationClip);
        public override Clip AddClip(UnityEngine.Object referenceObject, int frame) {
            NewMugenImageAnimationClip clip = new NewMugenImageAnimationClip(this, frame);
            m_Clips.Add(clip);
            return clip;
        }
#endif
    }

    [ScriptGuid("e75a4b28054c19fc48904a387acd1feb"), Color(165, 032, 025)]
    public class NewMugenImageAnimationClip : CharacterClip
    {
        [ShowInInspector, OnValueChanged("RebindTimeline", "OnCheckImageAnimationVaild")]
        public int actionNo = ImageAnimation._cNoVaildState;
        [ShowInInspector, OnValueChanged("RebindTimeline", "OnCheckImageAnimationVaild")]
        public UnityEngine.AnimationClip animClip = null;

        [System.NonSerialized]
        private UnityEngine.Animation animComponent = null;

        public override void Bind() {
            base.Bind();
            if (animClip != null && Character != null && Character is NewMugnCharacter) {
                var MugenChar = Character as NewMugnCharacter;
                if (MugenChar != null) {
                    animComponent = MugenChar.UnityAnimation;
                    animComponent.AddClip(animClip, animClip.name);
                }
            }
        }

        public override void Unbind() {
            if (animComponent != null && animClip != null) {
                if (animComponent.clip == animClip)
                    animComponent.clip = null;
                animComponent.RemoveClip(animClip);
            }
            animComponent = null;
            base.Unbind();
        }

        public override void OnEnable() {
            if (animComponent != null && animClip != null) {
                if (animComponent.enabled)
                    animComponent.enabled = false;
                animComponent.clip = animClip;
                float animDeltaTime = TargetTime - StartTime;
                if (animDeltaTime >= 0) {
                    var animState = animComponent[animClip.name];
                    if (animState != null) {
                        animState.time = animDeltaTime;
                        animComponent.Sample();
                    }
                }
            }
        }

        public override void OnDisable() {
            if (animComponent != null && animClip != null) {
                if (animComponent.enabled)
                    animComponent.enabled = false;
                if (animComponent.clip == animClip)
                    animComponent.clip = null;
            }
        }

#if UNITY_EDITOR

        [Button("Reset Clip Length")]
        void OnBtnClickAnimClipResetLength() {
            EndFrame = Length + StartFrame;
            RebindTimeline();
            // RepaintInspector();
        }

        void OnCheckImageAnimationVaild() {
            Invalid = actionNo == ImageAnimation._cNoVaildState || animClip == null;
        }

        public override ClipCapabilities Capabilities {
            get {
                return ClipCapabilities.Resizable | ClipCapabilities.ClipInable;
            }
        }

        public override int Length {
            get {
                if (animClip != null) {
                    int ret = Mathf.RoundToInt(animClip.length * TimelineUtility.FrameRate);
                    return ret;
                }
                return base.Length;
            }
        }

        public NewMugenImageAnimationClip(Track track, int frame) : base(track, frame) {
            CanSkip = true;
        }

        public override string Name => "ImageAnimationClip";
#endif
    }
}
