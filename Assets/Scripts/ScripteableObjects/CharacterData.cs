using SkillIssue;
using UnityEngine;
using SkillIssue.Inputs;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    private CharacterModel characterModel;
    [SerializeField]
    string characterName;
    [SerializeField]
    int maxHP = 1000;
    [SerializeField]
    float speed = 1;
    [SerializeField]
    float runSpeed = 3;
    [SerializeField]
    int airActions = 1;
    [SerializeField]
    float jumpForce = 1;
    [SerializeField]
    float gravity = 1;

    [Space]

    [SerializeField]
    CharacterAttackData attackData;

    [SerializeField]
    CharacterAnimationsData characterAnimationsData;

    public CharacterModel GetCharacterModel() { return characterModel; }
    public string GetCharacterName() {  return characterName; }
    public int GetMaxHP() { return maxHP; }
    public float GetMovementSpeed() { return speed; }
    public float GetRunSpeed() {  return runSpeed; }
    public int GetAirActions() { return airActions; }
    public AttackData[] GetGrabData() { return attackData.grabs; }
    public AttackData[] GetStandingAttacks() { return attackData.standingAttacks; }
    public AttackData[] GetCrouchingAttacks() {  return attackData.crouchingAttacks; }
    public AttackData[] GetJumpAttacks() { return attackData.jumpingAttacks; }
    public AttackData[] GetForwardAttacks() { return attackData.forwardAttacks; }
    public AttackData[] GetSpecialAttacks() { return attackData.specialAttacks; }

    public CharacterAnimationsData GetCharacterAnimationsData()
    {
        return characterAnimationsData;
    }

    public float GetGravity()
    {
        return gravity;
    }

    public float GetJumpPower()
    {
        return jumpForce;
    }

    public AttackData FindSpecialAttack(MotionInputs motion, InputType inputType, bool jumping = false)
    {
        if (jumping)
        {
            foreach (AttackData special in GetSpecialAttacks())
            {
                if (special.GetMotionInput() == motion && special.GetInputType() == inputType && special.GetAttackState() == SkillIssue.StateMachineSpace.States.Jumping)
                    return special;
            }
            return null;
        }

        foreach (AttackData special in GetSpecialAttacks())
        {
            if (special.GetMotionInput() == motion && special.GetInputType() == inputType)
               return special;
        }
        return null;
    }
}
