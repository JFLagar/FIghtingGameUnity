using SkillIssue;
using SkillIssue.CharacterSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAttackData", menuName = "Scriptable Objects/CharacterAttackData")]
public class CharacterAttackData : ScriptableObject
{
    [SerializeField]
    public AttackData[] standingAttacks;
    [SerializeField]
    public AttackData[] crouchingAttacks;
    [SerializeField]
    public AttackData[] jumpingAttacks;
    [SerializeField]
    public AttackData[] forwardAttacks;
    [SerializeField]
    public AttackData[] specialAttacks;
    [SerializeField]
    public AttackData[] grabs;

    public AttackData[] GetStandingAttacks() {  return standingAttacks; }
    public AttackData[] GetCrouchingAttacks() { return crouchingAttacks;}
    public AttackData[] GetJumpingAttacks() { return jumpingAttacks;}
    public AttackData[] GetForwardAttacks() { return forwardAttacks;}
    public AttackData[] GetSpecialAttacks() { return specialAttacks;}
    public AttackData[] GetGrabs() {  return grabs;}

}
