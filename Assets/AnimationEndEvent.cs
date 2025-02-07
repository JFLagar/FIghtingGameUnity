using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class AnimationEndEvent : StateMachineBehaviour
{
    public UnityAction OnAnimationEndEvent; // Event to notify MonoBehaviour

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter");
    }
    public override void OnStateExit(UnityEngine.Animator animator, UnityEngine.AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Debug.Log("Ended");
        OnAnimationEndEvent?.Invoke();
    }

}
