using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Taco.Timeline;

namespace Taco.Timeline.Mugen
{
    [TrackGroup("Character"), ScriptGuid("8752e06845a356f448b43716a9064631"), Color(165, 032, 025)]
    public class NewMugenImageAnimationTrack : CharacterTrack
    {

        public override void Bind() {
#if UNITY_EDITOR
            if (Timeline is CharacterTimeline characterTimeline) {
                var CharTimeline = Timeline as CharacterTimeline;
                if (CharTimeline != null && CharTimeline.TimelinePlayer != null && CharTimeline.Character == null) {
                    CharTimeline.Character = CharTimeline.TimelinePlayer.GetComponentInChildren<PlatformCharacter>();
                }
            }
#endif
            base.Bind();
        }

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

    [ScriptGuid("8752e06845a356f448b43716a9064631"), Color(165, 032, 025)]
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

        void SampleAnimComponent() {
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

        void ResetAnimComponent() {
            if (animComponent != null && animClip != null) {
                if (animComponent.enabled)
                    animComponent.enabled = false;
                if (animComponent.clip == animClip)
                    animComponent.clip = null;
            }
        }

        public override void OnEnable() {
           // SampleAnimComponent();
        }

        public override void OnDisable() {
            ResetAnimComponent();
        }

        public override void Evaluate(float deltaTime) {
            base.Evaluate(deltaTime);
            if (Active) {

            }
        }

#if UNITY_EDITOR

        [Button("Reset Clip Length")]
        void OnBtnClickAnimClipResetLength() {
            EndFrame = Length + StartFrame;
            FrameToTime();
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
