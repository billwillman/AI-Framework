using UnityEngine;
using UnityEngine.Playables;

namespace UnityTimeline
{
    [System.Serializable]
    public class UnityTimelineTreePlayable : PlayableAsset
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<UnityTimelineTreePlayBehaviour>.Create(graph);
        }
    }
}
