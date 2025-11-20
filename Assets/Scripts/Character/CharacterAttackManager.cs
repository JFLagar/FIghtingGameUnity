using SkillIssue;
using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System.Collections;
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

    public void Attack(AttackData data, bool followup = false)
    {
        //check if can cancel
        if (character.GetCurrentActionState() == ActionStates.Attack && !followup)
        {
            if (!IsCancelable(data))
            {
                return;
            }
        }
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.SetState(ColliderState.Closed);
            hitbox.SetResponder(this);
        }
        if (data.animation != null)
        {
            character.GetCharacterAnimation().PlayActionAnimation(data.animation);
        }
        repeatedAttack = 0;
        character.Attack(data);
        hit = false;
        previousAttack = data;
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

    private bool IsCancelable(AttackData data)
    {
        if (!hit)
        {
            return false;
        }
        if (data.grab)
        {
            return false;
        }


        if (data.canceleableSelf && data == previousAttack)
        {
            if (character.GetComboCount() >= sameLimit)
            {
                int count = character.GetComboCount() - 1;
                while (count >= character.GetComboCount() - sameLimit)
                {
                    if (data == character.GetCombo()[count])
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

        if (data != previousAttack)
        {
            repeatedAttack = 0;
        }

        if (data.attackState == AttackState.Jumping)
        {
            return false;
        }
        if (!data.canceleableSelf && data == previousAttack)
        {
            return false;

        }

        foreach (InputType canceltype in previousAttack.cancelableTypes)
        {
            if (character.GetCombo().Contains(data) && character.SameAttackSequence)
            {
                return false;
            }
            if (data.inputType == canceltype)
            {
                return true;
            }
            if (!previousAttack.IsSpecial && data.IsSpecial)
                return true;
        }
        return false;
    }

}
