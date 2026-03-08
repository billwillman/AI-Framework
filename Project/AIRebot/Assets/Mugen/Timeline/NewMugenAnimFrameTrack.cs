using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Taco.Timeline;

namespace Taco.Timeline.Mugen
{
    [TrackGroup("Character"), ScriptGuid("a1066dc4983be8144a7d778751f131b0"), Color(165, 032, 025)]
    public class NewMugenAnimFrameTrack : CharacterTrack
    {
#if UNITY_EDITOR
        public override Type ClipType => typeof(NewMugenAniFrameClip);
#endif
    }

    [System.Serializable]
    public struct NewMugenAnimFrameData
    {
        public float normalTime;
        public uint frameIndex;
    }

    [ScriptGuid("a1066dc4983be8144a7d778751f131b0"), Color(165, 032, 025)]
    public class NewMugenAniFrameClip : CharacterClip
    {
        [ShowInInspector, ReadOnly]
        public List<NewMugenAnimFrameData> Frames;

#if UNITY_EDITOR
        public NewMugenAniFrameClip(Track track, int frame) : base(track, frame) {
            CanSkip = true;
        }

        public override ClipCapabilities Capabilities {
            get {
                return ClipCapabilities.Resizable | ClipCapabilities.ClipInable;
            }
        }

        public override string Name => "MugenAnimFrameClip";
#endif
    }
}
