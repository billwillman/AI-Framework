using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class UnityTimelinePlayableBehaviour : PlayableBehaviour
{
    [System.NonSerialized]
    public UnityTimeline.UnityTimelineTree RuntimeTree = null;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {

    }
}
