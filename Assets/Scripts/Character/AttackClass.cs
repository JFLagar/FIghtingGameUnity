using SkillIssue;
using SkillIssue.CharacterSpace;
using SkillIssue.StateMachineSpace;
using System.Collections;
using UnityEngine;

public class AttackClass : MonoBehaviour, IHitboxResponder
{
    private AttackData previousAttack;
    public Hitbox[] hitboxes;
    public Character character;
    private AttackData currentAttack;
    private bool hit = false;
    public int repeatedAttack = 0;
    public int sameLimit;
    [SerializeField]
    public Coroutine landCheck = null;
    public int waitFrame = 0;
    public int landFrame = 0;
    public void Attack(AttackData data, bool followup = false)
    {
        if (character.GetCurrentState() == States.Jumping)
        {
            landCheck = StartCoroutine(CheckForLandCancel(data));
        }
        //check if can cancel
        if (character.GetCurrentActionState() == ActionStates.Attack && !followup)
        {
            if (!IsCancelable(data))
            {
                character.SetStoredAttack(data);
                return;
            }
        }
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.state = ColliderState.Closed;
            hitbox.setResponder(this);
        }
        if (data.animation != null)
        {
            character.GetAnimator().speed = 1;
            character.GetCharacterAnimation().AddAnimation(AnimType.Attack, data.animation.name);
        }
        repeatedAttack = 0;
        character.SetActionState(ActionStates.Attack);
        hit = false;
        previousAttack = data;
        currentAttack = null;

        //Attack
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
        if (previousAttack.followUpAttack != null)
        {
            Attack(previousAttack.followUpAttack, true);
            return;
        }
        if (character.GetStoredAttack() != null)
        {
            character.PerformAttack(character.GetStoredAttack().attackType);
            return;
        }

    }

    public void StartCheckingCollisions()
    {
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.StartCheckingCollision();
        }
    }

    public void StopCheckingCollisions()
    {
        foreach (Hitbox hitbox in hitboxes)
        {
            hitbox.StopCheckingCollision();
        }
    }
    private bool IsCancelable(AttackData data)
    {
        if (character.GetCurrentActionState() == ActionStates.Landing)
        {
            character.SetStoredAttack(null);
            return true;
        }
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
        }
        if (data != previousAttack)
        {
            repeatedAttack = 0;
        }

        if (data == character.GetStoredAttack())
        {
            character.SetStoredAttack(null);
        }
        if (data.attackState == AttackState.Jumping)
        {
            return false;
        }
        if (!data.canceleableSelf && data == previousAttack)
        {
            return false;

        }

        foreach (AttackType canceltype in previousAttack.cancelableTypes)
        {
            if (data.attackType == canceltype)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator CheckForLandCancel(AttackData data)
    {
        waitFrame = 0;
        landFrame = 0;
        while (waitFrame < 10)
        {
            if (character.GetCurrentState() == States.Standing)
            {
                while (landFrame < 5)
                {
                    if (character.GetCurrentActionState() == ActionStates.Landing)
                    {
                        character.PerformAttack(data.attackType);
                    }
                    yield return null;
                    landFrame++;
                }
            }

            yield return null;
            waitFrame++;
        }
    }
}
