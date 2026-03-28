using UnityEngine.Playables;

namespace UnityTimeline
{
    /// <summary>
    /// PlayableDirector 的封装实现类。
    /// 内部持有 PlayableDirector 实例，实现 IDirectorController 接口。
    /// </summary>
    public class PlayableDirectorController : IDirectorController
    {
        private PlayableDirector m_Director;

        public PlayableDirectorController(PlayableDirector director)
        {
            m_Director = director;
        }

        public void Play()
        {
            m_Director?.Play();
        }

        public void Pause()
        {
            m_Director?.Pause();
        }

        public void Stop()
        {
            m_Director?.Stop();
        }

        public double time
        {
            get => m_Director != null ? m_Director.time : 0;
            set { if (m_Director != null) m_Director.time = value; }
        }

        public DirectorState state
        {
            get
            {
                if (m_Director == null)
                    return DirectorState.Stopped;

                switch (m_Director.state)
                {
                    case PlayState.Playing:
                        return DirectorState.Playing;
                    case PlayState.Paused:
                        return DirectorState.Paused;
                    default:
                        return DirectorState.Stopped;
                }
            }
        }

        public bool IsValid => m_Director != null;

        public void SetSpeed(double speed)
        {
            if (m_Director != null && m_Director.playableGraph.IsValid())
            {
                var rootPlayable = m_Director.playableGraph.GetRootPlayable(0);
                rootPlayable.SetSpeed(speed);
            }
        }
    }
}
