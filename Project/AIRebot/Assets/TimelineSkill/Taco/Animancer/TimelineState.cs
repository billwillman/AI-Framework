using System;
using System.Text;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Animancer
{
    /// <summary>
    /// An <see cref="AnimancerState"/> which plays a <see cref="Taco.Timeline.Timeline"/> asset
    /// by grafting its Playable sub-graph into Animancer's PlayableGraph.
    /// </summary>
    public class TimelineState : AnimancerState, ICopyable<TimelineState>
    {
        /// <summary>An <see cref="ITransition{TState}"/> that creates a <see cref="TimelineState"/>.</summary>
        public interface ITransition : ITransition<TimelineState> { }

        #region Fields and Properties

        private Taco.Timeline.Timeline _Timeline;

        /// <summary>The <see cref="Taco.Timeline.Timeline"/> which this state plays.</summary>
        public Taco.Timeline.Timeline Timeline
        {
            get => _Timeline;
            set => ChangeMainObject(ref _Timeline, value);
        }

        /// <summary>The <see cref="Taco.Timeline.Timeline"/> which this state plays.</summary>
        public override Object MainObject
        {
            get => _Timeline;
            set => _Timeline = (Taco.Timeline.Timeline)value;
        }

        /// <summary>The duration of the Timeline asset.</summary>
        public override float Length => _Timeline != null ? _Timeline.Duration : 0;

        /// <summary>IK cannot be dynamically enabled on a <see cref="TimelineState"/>.</summary>
        public override void CopyIKFlags(AnimancerNode copyFrom) { }

        /// <summary>IK cannot be dynamically enabled on a <see cref="TimelineState"/>.</summary>
        public override bool ApplyAnimatorIK
        {
            get => false;
            set
            {
#if UNITY_ASSERTIONS
                if (value)
                    OptionalWarning.UnsupportedIK.Log(
                        $"IK cannot be dynamically enabled on a {nameof(TimelineState)}.", Root?.Component);
#endif
            }
        }

        /// <summary>IK cannot be dynamically enabled on a <see cref="TimelineState"/>.</summary>
        public override bool ApplyFootIK
        {
            get => false;
            set
            {
#if UNITY_ASSERTIONS
                if (value)
                    OptionalWarning.UnsupportedIK.Log(
                        $"IK cannot be dynamically enabled on a {nameof(TimelineState)}.", Root?.Component);
#endif
            }
        }

        #endregion

        #region Methods

        /// <summary>Creates a new <see cref="TimelineState"/> to play the given timeline.</summary>
        public TimelineState(Taco.Timeline.Timeline timeline)
        {
            if (timeline == null)
                throw new ArgumentNullException(nameof(timeline));

            _Timeline = timeline;
        }

        /// <summary>
        /// Creates the Playable graph structure:
        /// ScriptPlayable (drives Timeline.Evaluate) -> AnimationLayerMixerPlayable (animation root for Timeline tracks)
        /// </summary>
        protected override void CreatePlayable(out Playable playable)
        {
            var handle = ScriptPlayable<TimelinePlayableBehaviour>.Create(Root._Graph);
            var behaviour = handle.GetBehaviour();

            var animRoot = AnimationLayerMixerPlayable.Create(Root._Graph);
            handle.AddInput(animRoot, 0, 1);

            _Timeline.Init();
            _Timeline.Bind(Root._Graph, animRoot);
            behaviour.Timeline = _Timeline;

            playable = handle;
        }

        /// <inheritdoc/>
        protected override void OnSetIsPlaying()
        {
            // Pause/resume child playables when state play status changes
            var inputCount = _Playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                var child = _Playable.GetInput(i);
                if (!child.IsValid())
                    continue;

                if (IsPlaying)
                    child.Play();
                else
                    child.Pause();
            }
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            if (_Timeline != null && _Timeline.Binding)
                _Timeline.Unbind();

            _Timeline = null;
            base.Destroy();
        }

        /// <inheritdoc/>
        public override AnimancerState Clone(AnimancerPlayable root)
        {
            var clone = new TimelineState(_Timeline);
            clone.SetNewCloneRoot(root);
            ((ICopyable<TimelineState>)clone).CopyFrom(this);
            return clone;
        }

        /// <inheritdoc/>
        void ICopyable<TimelineState>.CopyFrom(TimelineState copyFrom)
        {
            ((ICopyable<AnimancerState>)this).CopyFrom(copyFrom);
        }

        /// <inheritdoc/>
        protected override void AppendDetails(StringBuilder text, string separator)
        {
            base.AppendDetails(text, separator);

            text.Append(separator).Append($"{nameof(Timeline)}: ");
            text.Append(AnimancerUtilities.ToStringOrNull(_Timeline));

            if (_Timeline != null)
            {
                text.Append(separator).Append($"Duration: {_Timeline.Duration:F3}s");
                text.Append(separator).Append($"Binding: {_Timeline.Binding}");
            }
        }

        #endregion
    }

    /// <summary>
    /// A <see cref="PlayableBehaviour"/> that drives <see cref="Taco.Timeline.Timeline.Evaluate"/>
    /// during the PlayableGraph evaluation, ensuring non-animation tracks (particles, signals, etc.) work.
    /// </summary>
    public class TimelinePlayableBehaviour : PlayableBehaviour
    {
        public Taco.Timeline.Timeline Timeline;

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            Timeline?.Evaluate(info.deltaTime);
        }
    }
}
