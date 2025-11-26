using SkillIssue;
using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterAttackManager : MonoBehaviour, IHitboxResponder
{
    private AttackData previousAttack;
    [SerializeField]
    public Hitbox[] hitboxes;
    private Character character;
    private AttackData currentAttack;
    private bool hit = false;
    int repeatedAttack = 0;
    int sameLimit = 3;
    [SerializeField]
    public Coroutine landCheck = null;

    public void Initialize(Character controllingChar, Hitbox[] hitboxes)
    {
        character = controllingChar;
        this.hitboxes = hitboxes;
    }

    public void Attack(AttackData attack, bool followup = false)
    {
        //check if can cancel
        if (character.GetCurrentActionState() == ActionStates.Attack && !followup)
        {
            if (!IsCancelable(attack))
            {
                return;
            }
        }
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.SetState(ColliderState.Closed);
            hitbox.SetResponder(this);
        }
        if (attack.GetAnimationClip() != null)
        {
            character.GetCharacterAnimation().PlayActionAnimation(attack.GetAnimationClip());
        }
        repeatedAttack = 0;
        character.Attack(attack);
        hit = false;
        previousAttack = attack;
        currentAttack = null;

        //Attack
    }

    public void ClearPreviousAttack()
    {
        currentAttack = null;
    }

    public void BoxCollisionedWith(Collider2D collider)
    {
        if (currentAttack != previousAttack)
        {
            currentAttack = previousAttack;
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox?.GetHitBy(previousAttack);
            hit = true;
            character.HitConnect(previousAttack);
        }
    }

    private bool IsCancelable(AttackData attack)
    {
        if (!hit)
        {
            return false;
        }
        if (attack.IsGrab())
        {
            return false;
        }


        if (attack.GetCancelTypes().ToList().Contains(CancelTypes.Self) && attack == previousAttack)
        {
            if (character.GetComboCount() >= sameLimit)
            {
                int count = character.GetComboCount() - 1;
                while (count >= character.GetComboCount() - sameLimit)
                {
                    if (attack == character.CurrentCombo[count])
                    {
                        repeatedAttack++;
                    }
                    count--;
                }
                if (repeatedAttack >= sameLimit)
                {
                    return false;
                }
            }
            return true;
        }

        if (attack != previousAttack)
        {
            repeatedAttack = 0;
        }

        if (!attack.GetCancelTypes().ToList().Contains(CancelTypes.Self) && attack == previousAttack)
        {
            return false;

        }

        foreach (AttackData cancelableAttack in previousAttack.GetCancelableAttacks())
        {
            if (attack == cancelableAttack)
            {
                return true;
            }
        }
        if (previousAttack.GetCancelTypes().ToList().Contains(CancelTypes.Special) && attack.IsSpecialMove())
            return true;

        return false;
    }

}
