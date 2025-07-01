using SkillIssue.CharacterSpace;
using SkillIssue;
using UnityEngine;
using SkillIssue.Inputs;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    string characterName;
    [SerializeField]
    int maxHP = 1000;
    [SerializeField]
    float speed = 200;
    [SerializeField]
    int airActions = 1;
    [SerializeField]
    float jumpForce = 1;
    [SerializeField]
    float forceSpeed = 1;

    [Space]

    [SerializeField]
    Element element;
    [SerializeField]
    ElementData elementData;
    [SerializeField]
    AttackData grab;
    [SerializeField]
    AttackData[] standingAttacks;
    [SerializeField]
    AttackData[] crouchingAttacks;
    [SerializeField]
    AttackData[] jumpAttacks;
    [SerializeField]
    AttackData[] specialAttacks;

    [SerializeField]
    CharacterAnimationsData characterAnimationsData;

    public string GetCharacterName() {  return characterName; }
    public int GetMaxHP() { return maxHP; }
    public float GetMovementSpeed() { return speed; }
    public int GetAirActions() { return airActions; }
    public Element GetElement() { return element; }
    public AttackData GetGrabData() { return grab; }
    public AttackData[] GetStandingAttacks() {  return standingAttacks; }
    public AttackData[] GetCrouchingAttacks() {  return crouchingAttacks; }
    public AttackData[] GetJumpAttacks() { return jumpAttacks; }
    public AttackData[] GetSpecialAttacks()
    {
        return elementData.GetElementAttackData(element);
    }
    public CharacterAnimationsData GetCharacterAnimationsData()
    {
        return characterAnimationsData;
    }

    public float GetForceSpeed()
    {
        return forceSpeed;
    }

    public float GetJumpPower()
    {
        return jumpForce;
    }

    public AttackData FindSpecialAttack(MotionInputs motion, InputType inputType)
    {
        foreach (AttackData special in specialAttacks)
        {
            //if (special.motionInput == motion && special.inputType == inputType)
            //    return special;
            if (special.motionInput == motion)
                return special;
        }
        return null;
    }
}
