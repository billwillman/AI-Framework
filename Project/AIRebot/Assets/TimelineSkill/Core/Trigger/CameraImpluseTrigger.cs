using UnityEngine;
using Cinemachine;

public class CameraImpluseTrigger : TriggerBase
{
    [Space(10), Header("===== Custom =====")]
    public CinemachineImpulseDefinition.ImpulseTypes ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;
    public CinemachineImpulseDefinition.ImpulseShapes ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump;
    public AnimationCurve ImpulseCurve;
    public float ImpulseDuration = 0.1f;
    public Vector3 ImpulseVelocity;

    CinemachineImpulseDefinition m_ImpulseDefinition;
    protected override void DoAction(HitBox hitBox)
    {
        if(m_ImpulseDefinition == null)
        {
            m_ImpulseDefinition = new CinemachineImpulseDefinition
            {
                m_ImpulseChannel = 1,
                m_ImpulseShape = ImpulseShape,
                m_CustomImpulseShape = ImpulseCurve,
                m_ImpulseDuration = ImpulseDuration,
                m_ImpulseType = ImpulseType,
                m_DissipationDistance = 100,
                m_DissipationRate = 0.25f,
                m_PropagationSpeed = 343
            };
        }
        m_ImpulseDefinition.CreateEvent(transform.position, ImpulseVelocity);
    }
}
