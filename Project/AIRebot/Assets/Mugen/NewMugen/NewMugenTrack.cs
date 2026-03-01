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
        [ShowInInspector, OnValueChanged("RebindTimeline")]
        public int actionNo = ImageAnimation._cNoVaildState;
        [ShowInInspector, OnValueChanged("RebindTimeline")]
        public UnityEngine.AnimationClip animClip = null;
#if UNITY_EDITOR

        public override ClipCapabilities Capabilities {
            get {
                return ClipCapabilities.Resizable | ClipCapabilities.ClipInable;
            }
        }

        public override int Length {
            get {
                if (animClip != null) {
                    int ret = Mathf.RoundToInt(animClip.length / TimelineUtility.FrameRate);
                    return ret;
                }
                return base.Length;
            }
        }

        public NewMugenImageAnimationClip(Track track, int frame) : base(track, frame) { }

        public override string Name => "ImageAnimationClip";
#endif
    }
}
