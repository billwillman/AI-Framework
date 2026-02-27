using UnityEngine;

public class TimeScalerTrigger : TriggerBase
{
    [Space(10), Header("===== Custom =====")]
    public float Scale;
    public float Duration;
    public float BlendIn;
    public float BlendOut;

    protected override void DoAction(HitBox hitBox)
    {
        TimeMananger.Instance?.ChangeTimeScale(Scale, Duration, BlendIn, BlendOut);
    }
}