using SkillIssue.CharacterSpace;
using UnityEngine.Animations;
using UnityEngine;
using SkillIssue;
using Unity.VisualScripting;

public class CharacterModel : MonoBehaviour
{
    private Character character;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Hurtbox[] hurtboxes;
    [SerializeField]
    private Hitbox[] hitboxes;
    [SerializeField]
    private Transform collisions;
    public void Initialize(Character character)
    {
        this.character = character;
        foreach (Hurtbox hurtbox in hurtboxes)
        {
            hurtbox.character = character;
            hurtbox.gameObject.layer = LayerMask.NameToLayer(character.GetHurtboxLayerMask());
        }
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.gameObject.layer = character.GetHitboxLayerMask();
            hitbox.targetMask = character.GetHitboxTargetMask();
        }

    }

    public Transform GetCollisions()
    { 
        return collisions; 
    }

    public Hitbox[] GetHitboxes()
    {
        return hitboxes;
    }

    public Hurtbox[] GetHurtboxes()
    {
        return hurtboxes;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void ApplyAttackForce()
    {
        character.ApplyAttackForce();
    }

    public void ClearPreviousAttack()
    {
       character.ClearPreviousAttack();
    }

    public void AnimEvent()
    {
        character.AnimEvent();
    }

    public void PlaySound(AudioClip clip = null)
    {
        character.PlaySound(clip);
    }
}
