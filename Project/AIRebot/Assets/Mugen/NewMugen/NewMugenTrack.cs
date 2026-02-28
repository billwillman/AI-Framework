using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Taco.Timeline;

namespace Taco.Timeline.Mugen
{
    [TrackGroup("Character"), ScriptGuid("828a8311b89bab843b655b0181bf41e4")]
    public class NewMugenImageAnimationTrack : Track
    {
        [ShowInInspector, OnValueChanged("RebindTimeline")]
        public int actionNo = ImageAnimation._cNoVaildState;
    }
}
