using SkillIssue;
using SkillIssue.CharacterSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementData", menuName = "Scriptable Objects/ElementData")]
public class ElementData : ScriptableObject
{
    [SerializeField]
    AttackData[] fireAttacks;
    [SerializeField]
    AttackData[] earthAttacks;
    [SerializeField]
    AttackData[] windAttacks;
    [SerializeField]
    AttackData[] waterAttacks;

    public AttackData[] GetElementAttackData(Element element)
    {
        switch (element)
        {
            case Element.Fire:
                return fireAttacks;
            case Element.Earth:
                return earthAttacks;
            case Element.Wind:
                return windAttacks;
            case Element.Water:
                return waterAttacks;
            default:
                break;
        }
        return null;
    }
}
