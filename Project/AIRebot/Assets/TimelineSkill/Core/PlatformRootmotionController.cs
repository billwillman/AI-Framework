using EasyCharacterMovement;
using Taco.Timeline;

public class PlatformRootmotionController : RootMotionController
{
    TimelinePlayer timelinePlayer;

    public override void Awake()
    {
        base.Awake();
        timelinePlayer = GetComponent<TimelinePlayer>();
        timelinePlayer.OnEvaluated += SetRootMotionVelocity;
    }
    public override void OnAnimatorMove() { }

    public void SetRootMotionVelocity()
    {
        animRootMotionVelocity = CalcAnimRootMotionVelocity();
    }
}
