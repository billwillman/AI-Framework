using UnityEngine;

public class AddForceTrigger : TriggerBase
{
    [Space(10), Header("===== Custom =====")]
    public ForceDirectionType DirectionType;
    public Vector3 Force;
    public int Direction 
    {
        get
        {
            switch (DirectionType)
            {
                case ForceDirectionType.WithSourceTransform:
                    return m_Source.Direction;
                case ForceDirectionType.WithSourceVelocity:
                    return m_Source.VelocityDirection;
                default:
                    return m_Source.Direction;
            }
        }
    }

    protected override void DoAction(HitBox hitBox)
    {
        if(DirectionType == ForceDirectionType.Explosion)
        {
            int direction = (hitBox.Source.GetPosition() - m_Source.GetPosition()).x > 0 ? 1 : -1;
            hitBox.AddExternalForce(new Vector3(Force.x * direction, Force.y, Force.z));
        }
        else
        {
            hitBox.AddExternalForce(new Vector3(Force.x * Direction, Force.y, Force.z));
        }
    }

    public enum ForceDirectionType
    {
        WithSourceTransform,
        WithSourceVelocity,
        Explosion,
    }
}
