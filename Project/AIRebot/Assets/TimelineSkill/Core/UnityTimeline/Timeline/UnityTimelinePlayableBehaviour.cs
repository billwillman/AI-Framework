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

    public override void OnPlayableDestroy(Playable playable) {
        if (RuntimeTree != null) {
            if (Application.isPlaying)
                GameObject.DestroyImmediate(RuntimeTree);
            else
                GameObject.Destroy(RuntimeTree);
            RuntimeTree = null;
        }
    }
}
