using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityTimeline
{
    [TrackClipType(typeof(UnityTimelineTreePlayable))]
    [TrackBindingType(typeof(PlayableDirector))]
    public class UnityTimelineTreeTrackAsset : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return Playable.Null;
        }
    }
}
