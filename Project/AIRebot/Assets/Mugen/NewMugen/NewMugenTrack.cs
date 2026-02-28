using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Taco.Timeline;

namespace Taco.Timeline.Mugen
{
    [TrackGroup("Ability"), ScriptGuid("f6e6bb52a7c30ce4789f78dcd9059c85"), Color(165, 032, 025)]
    public class NewMugenImageAnimationTrack : CharacterTrack
    {
        [ShowInInspector, OnValueChanged("RebindTimeline")]
        public int actionNo = ImageAnimation._cNoVaildState;
        [ShowInInspector, OnValueChanged("RebindTimeline")]
        public UnityEngine.AnimationClip animClip = null;
    }
}
