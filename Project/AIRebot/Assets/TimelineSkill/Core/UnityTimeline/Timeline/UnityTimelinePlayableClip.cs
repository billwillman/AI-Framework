using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class UnityTimelinePlayableClip : PlayableAsset, ITimelineClipAsset
{
    public UnityTimelinePlayableBehaviour template = new UnityTimelinePlayableBehaviour ();

    // public ExposedReference<UnityTimeline.UnityTimelineTree> timelineTree;
    public UnityTimeline.UnityTimelineTree timelineTree;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<UnityTimelinePlayableBehaviour>.Create (graph, template);
        UnityTimelinePlayableBehaviour clone = playable.GetBehaviour ();
        if (clone.RuntimeTree != null) {
            if (Application.isPlaying)
                GameObject.Destroy(clone.RuntimeTree);
            else
                GameObject.DestroyImmediate(clone.RuntimeTree);
            clone.RuntimeTree = null;
        }
        if (timelineTree != null)
            clone.RuntimeTree = GameObject.Instantiate(timelineTree);
        else
            clone.RuntimeTree = null;
        return playable;
    }
}
