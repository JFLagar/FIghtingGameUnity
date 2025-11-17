using SkillIssue;
using SkillIssue.CharacterSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAttackData", menuName = "Scriptable Objects/CharacterAttackData")]
public class CharacterAttackData : ScriptableObject
{
    public AttackData[] standingAttacks;
    public AttackData[] crouchingAttacks;
    public AttackData[] jumpingAttacks;
    public AttackData[] forwardAttacks;
    public AttackData[] specialAttacks;
    public AttackData[] grabs;

}
