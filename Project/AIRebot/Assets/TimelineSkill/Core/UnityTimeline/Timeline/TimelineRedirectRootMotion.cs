using UnityEngine;
using Animancer;

namespace UnityTimeline
{
    /// <summary>
    /// 继承 RedirectRootMotionToTransform，额外暴露位置和旋转的补偿变量。
    /// 在 UnityTimelineTree 中 JumpToTime 后调用 SetCompensationPosition/Rotation 来修正位置跳变。
    /// </summary>
    [AddComponentMenu("TimelineSkill/Timeline Redirect Root Motion")]
    public class TimelineRedirectRootMotion : RedirectRootMotionToTransform
    {
        [SerializeField]
        [Tooltip("位置补偿值，设置后会直接覆盖 Target.position")]
        private Vector3 m_CompensationPosition;

        [SerializeField]
        [Tooltip("旋转补偿值（欧拉角），设置后会直接覆盖 Target.rotation")]
        private Vector3 m_CompensationRotationEuler;

        /// <summary>是否启用补偿模式。启用后 OnAnimatorMove 会先应用补偿值再叠加 delta。</summary>
        public bool CompensationEnabled { get; set; }

        /// <summary>获取当前补偿位置</summary>
        public Vector3 CompensationPosition => m_CompensationPosition;

        /// <summary>获取当前补偿旋转（欧拉角）</summary>
        public Vector3 CompensationRotationEuler => m_CompensationRotationEuler;

        /// <summary>
        /// 设置位置补偿值，下一帧 OnAnimatorMove 生效时以此作为基线。
        /// </summary>
        public void SetCompensationPosition(Vector3 position)
        {
            m_CompensationPosition = position;
            CompensationEnabled = true;
        }

        /// <summary>
        /// 设置旋转补偿值（欧拉角），下一帧 OnAnimatorMove 生效时以此作为基线。
        /// </summary>
        public void SetCompensationRotation(Vector3 eulerAngles)
        {
            m_CompensationRotationEuler = eulerAngles;
            CompensationEnabled = true;
        }

        /// <summary>
        /// 同时设置位置和旋转补偿值。
        /// </summary>
        public void SetCompensation(Vector3 position, Vector3 rotationEuler)
        {
            m_CompensationPosition = position;
            m_CompensationRotationEuler = rotationEuler;
            CompensationEnabled = true;
        }

        /// <summary>
        /// 清除补偿，恢复正常 RootMotion 累加模式。
        /// </summary>
        public void ClearCompensation()
        {
            CompensationEnabled = false;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (Animator != null && !Animator.applyRootMotion)
                Animator.applyRootMotion = true;
        }

        protected override void OnAnimatorMove()
        {
            if (!ApplyRootMotion)
                return;

            if (CompensationEnabled)
            {
                // 补偿模式：以设定值为基线，再叠加本帧 delta
                Target.position = m_CompensationPosition + Animator.deltaPosition;
                Target.rotation = Quaternion.Euler(m_CompensationRotationEuler) * Animator.deltaRotation;
                
                // 单次生效后自动关闭
                CompensationEnabled = false;
            }
            else
            {
                // 正常模式：与父类一致
                Target.position += Animator.deltaPosition;
                Target.rotation *= Animator.deltaRotation;
            }
        }
    }
}
