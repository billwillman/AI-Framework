using UnityEngine;

public class ResetFrame : StateMachineBehaviour
{
    bool inited;
    PlatformTimelinePlayer timelinePlayer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!inited)
        {
            inited = true;
            timelinePlayer = animator.GetComponent<PlatformTimelinePlayer>();
        }
        timelinePlayer?.ResetFrame();
    }
}
