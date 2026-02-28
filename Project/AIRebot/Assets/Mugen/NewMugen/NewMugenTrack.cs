using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Taco.Timeline;

namespace Taco.Timeline.Mugen
{
    [TrackGroup("Character"), ScriptGuid("828a8311b89bab843b655b0181bf41e4"), Color(165, 032, 025)]
    public class NewMugenImageAnimationTrack : CharacterClip
    {
        [ShowInInspector, OnValueChanged("RebindTimeline")]
        public int actionNo = ImageAnimation._cNoVaildState;
#if UNITY_EDITOR
        public NewMugenImageAnimationTrack(Track track, int frame) : base(track, frame) { }
#endif
    }
}
