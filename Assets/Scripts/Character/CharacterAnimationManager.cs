using DG.Tweening;
using SkillIssue.CharacterSpace;
using SkillIssue.StateMachineSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AnimType
{
    Hit,
    Attack,
    Landing,
    Movement
}
public class CharacterAnimationManager : MonoBehaviour
{
    public string[] animNames;
    Animator animator;
    IDictionary<AnimType, string> animations = new Dictionary<AnimType, string>();

    private Character character;

    public void Initialize(Character character, Animator animator)
    {
        this.character = character;
        this.animator = animator;
        AttachAnimationEvent();
    }

    public void PlayAnimation(string[] m_animations)
    {
        foreach(string animationName in m_animations)
        {
            if (animationName != null)
            {               
                animator.Play(animationName);
                animations.Clear();
            }
        }
    }

    public void ClearAnimations()
    {
        animations.Clear();
        animator.ResetTrigger("Recovery");
        if (character.GetCurrentState() != States.Jumping)
            animator.Play(character.GetCurrentState().ToString());
        else
            animator.Play("JumpFall");
        animator.SetBool("PlayingAnim", true);
    }

    public void AddAnimation(AnimType type, string animName)
    {
        animations.TryAdd(type, animName);
        animations.TryGetValue(AnimType.Hit, out animNames[0]);
        animations.TryGetValue(AnimType.Attack, out animNames[2]);
        animations.TryGetValue(AnimType.Landing, out animNames[1]);
        animations.TryGetValue(AnimType.Movement, out animNames[3]);
        PlayAnimation(animNames);
        animator.SetBool("PlayingAnim", type != AnimType.Attack);
    }

    private void AttachAnimationEvent()
    {
        // Get all AnimationEndListeners attached to animation states
        AnimationEndEvent[] behaviours = animator.GetBehaviours<AnimationEndEvent>();

        foreach (AnimationEndEvent listener in behaviours)
        {
            listener.OnAnimationEndEvent += character.AnimEnd; // Subscribe to event
        }
    }

}

