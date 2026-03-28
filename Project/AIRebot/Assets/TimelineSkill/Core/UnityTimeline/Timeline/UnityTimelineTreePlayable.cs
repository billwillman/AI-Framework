using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using System.ComponentModel;
#endif

namespace UnityTimeline
{
    [System.Serializable]
#if UNITY_EDITOR
    [DisplayName("TimelineTree")]
#endif
    public class UnityTimelineTreePlayable : PlayableAsset
    {
        public ClipCaps clipCaps { get { return ClipCaps.None; } }
        public UnityTimelineTree timelineTree = null;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Playable ret = ScriptPlayable<UnityTimelineTreePlayBehaviour>.Create(graph);
            return ret;
        }
    }
}
