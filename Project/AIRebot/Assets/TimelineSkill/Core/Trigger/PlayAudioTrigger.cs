using UnityEngine;

public class PlayAudioTrigger : TriggerBase
{
    [Space(10), Header("===== Custom =====")]
    public AudioClip Clip;
    public float Volume;
    public float Speed;
    public float StartTime;

    protected override void DoAction(HitBox hitBox)
    {
        AudioManager.Instance.Play(Clip, Volume, Speed, StartTime);
    }
}
