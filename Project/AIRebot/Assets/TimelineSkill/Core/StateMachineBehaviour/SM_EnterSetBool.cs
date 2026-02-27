using UnityEngine;
using Taco.Timeline;

public class SM_EnterSetBool : StateMachineBehaviour
{
    public string paramName;
    public bool paramValue;

    bool inited;
    TimelinePlayer timelinePlayer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!inited)
        {
            inited = true;
            timelinePlayer = animator.GetComponent<TimelinePlayer>();
        }
        timelinePlayer?.SetBool(paramName, paramValue);
    }
}
