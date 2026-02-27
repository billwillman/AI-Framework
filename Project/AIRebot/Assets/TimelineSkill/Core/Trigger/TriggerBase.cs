using UnityEngine;

public abstract class TriggerBase : MonoBehaviour
{
    [Header("===== Base =====")]
    public float Delay;

    protected PlatformCharacter m_Source;
    bool m_Triggered;
    float m_TargetTime;
    HitBox m_HitBox;

    void Update()
    {
        if(m_Triggered && Time.realtimeSinceStartup >= m_TargetTime)
        {
            DoAction(m_HitBox);
            m_Triggered = false;
        }
    }

    public void Init(PlatformCharacter character)
    {
        enabled = true;
        m_Source = character;
    }
    public void Trigger(HitBox hitBox)
    {
        if (!enabled) return;

        if(Delay > 0)
        {
            m_Triggered = true;
            m_TargetTime = Time.realtimeSinceStartup + Delay;
            m_HitBox = hitBox;
        }
        else
        {
            DoAction(hitBox);
        }
    }

    protected abstract void DoAction(HitBox hitBox);
}
