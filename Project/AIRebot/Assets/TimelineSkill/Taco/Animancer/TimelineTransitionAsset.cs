using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <inheritdoc/>
    [CreateAssetMenu(menuName = "Taco/Animancer/Timeline Transition", order = 100)]
    public class TimelineTransitionAsset : AnimancerTransitionAsset<TimelineTransition>
    {
        /// <inheritdoc/>
        [Serializable]
        public new class UnShared :
            UnShared<TimelineTransitionAsset, TimelineTransition, TimelineState>,
            TimelineState.ITransition
        { }
    }

    /// <summary>
    /// A serializable <see cref="ITransition"/> that creates a <see cref="TimelineState"/>
    /// to play a <see cref="Taco.Timeline.Timeline"/> asset through Animancer.
    /// </summary>
    [Serializable]
    public class TimelineTransition : AnimancerTransition<TimelineState>,
        TimelineState.ITransition, IAnimationClipCollection, ICopyable<TimelineTransition>
    {
        [SerializeField]
        private Taco.Timeline.Timeline _Timeline;

        /// <summary>The <see cref="Taco.Timeline.Timeline"/> asset to play.</summary>
        public ref Taco.Timeline.Timeline Timeline => ref _Timeline;

        /// <inheritdoc/>
        public override Object MainObject => _Timeline;

        /// <inheritdoc/>
        public override bool IsValid => _Timeline != null;

        /// <inheritdoc/>
        public override float MaximumDuration => _Timeline != null ? _Timeline.Duration : 0;

        /// <inheritdoc/>
        public override TimelineState CreateState()
        {
#if UNITY_ASSERTIONS
            if (_Timeline == null)
                throw new ArgumentException(
                    $"Unable to create {nameof(TimelineState)} because the" +
                    $" {nameof(TimelineTransition)}.{nameof(Timeline)} is null.");
#endif

            return State = new TimelineState(_Timeline);
        }

        /// <summary>Creates a new <see cref="TimelineTransition"/>.</summary>
        public TimelineTransition() { }

        /// <summary>Creates a new <see cref="TimelineTransition"/> with the specified timeline.</summary>
        public TimelineTransition(Taco.Timeline.Timeline timeline) => _Timeline = timeline;

        /// <summary>Gathers animation clips from the Timeline's animation tracks.</summary>
        void IAnimationClipCollection.GatherAnimationClips(ICollection<AnimationClip> clips)
        {
            if (_Timeline == null)
                return;

            foreach (var track in _Timeline.Tracks)
            {
                if (track is Taco.Timeline.AnimationTrack)
                {
                    foreach (var clip in track.Clips)
                    {
                        if (clip is Taco.Timeline.AnimationClip animClip && animClip.Clip != null)
                            clips.Add(animClip.Clip);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public virtual void CopyFrom(TimelineTransition copyFrom)
        {
            CopyFrom((AnimancerTransition<TimelineState>)copyFrom);

            if (copyFrom == null)
            {
                _Timeline = default;
                return;
            }

            _Timeline = copyFrom._Timeline;
        }
    }
}
