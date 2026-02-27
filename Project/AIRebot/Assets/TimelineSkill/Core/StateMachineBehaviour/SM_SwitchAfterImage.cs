using UnityEngine;

public class SwitchAfterImage : StateMachineBehaviour
{
    public Vector2 ValidTime;
    public int[] TargetSubmesh;

    bool inited;
    AfterImageController afterImageController;
    
    float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!inited)
        {
            inited = true;
            afterImageController = animator.GetComponent<AfterImageController>();
        }
        //afterImageController?.SwitchAutoCreate(true);
        timer = 0;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //afterImageController?.SwitchAutoCreate(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(ValidTime != Vector2.zero)
        {
            if(timer >= ValidTime.x && timer <= ValidTime.y)
                afterImageController?.EnableAfterImage(true, TargetSubmesh);
            else
                afterImageController?.EnableAfterImage(false, TargetSubmesh);

            timer += Time.deltaTime;
        }
    }
}
