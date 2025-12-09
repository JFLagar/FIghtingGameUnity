using SkillIssue;
using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour, IHitboxResponder
{
    private AttackData previousAttack;
    [SerializeField]
    public Hitbox[] hitboxes;
    private Player player;
    private AttackData currentAttack;
    private bool hit = false;
    int repeatedAttack = 0;
    int sameLimit = 3;
    [SerializeField]
    public Coroutine landCheck = null;

    public void Initialize(Player player, Hitbox[] hitboxes)
    {
        this.player = player;
        this.hitboxes = hitboxes;
    }

    public void ProcessAttack(AttackData attack, bool followup = false)
    {
        //check if can cancel
        if (player.GetCurrentState() is AttackState && !followup)
        {
            if (!IsCancelable(attack))
            {
                return;
            }
            player.SetApplyGravity(false);
        }
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.SetState(ColliderState.Closed);
            hitbox.SetResponder(this);
        }
        if (attack.GetAnimationClip() != null)
        {
            player.GetCharacterAnimation().PlayActionAnimation(attack.GetAnimationClip());
        }
        repeatedAttack = 0;
        player.PerformAttack(attack);
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
            player.HitConnect(previousAttack);
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
            if (player.GetComboCount() >= sameLimit)
            {
                int count = player.GetComboCount() - 1;
                while (count >= player.GetComboCount() - sameLimit)
                {
                    if (attack == player.CurrentCombo[count])
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
