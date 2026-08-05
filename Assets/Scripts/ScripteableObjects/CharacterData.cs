using NaughtyAttributes;
using System.Collections.Generic;
using SkillIssue;
using SkillIssue.Animations;
using SkillIssue.Inputs;
using UnityEditor;
using UnityEngine;

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
    [SerializeField]
    private GameObject fbxAsset;

    [Space]

    [SerializeField]
    CharacterAttackData attackData;

    [SerializeField]
    [ReadOnly]
    CharacterAnimationsData characterAnimationsData;

    public CharacterModel GetCharacterModel() { return characterModel; }
    public string GetCharacterName() { return characterName; }
    public int GetMaxHP() { return maxHP; }
    public float GetMovementSpeed() { return speed; }
    public float GetRunSpeed() { return runSpeed; }
    public int GetAirActions() { return airActions; }
    public AttackData[] GetGrabData() { return attackData.grabs; }
    public AttackData[] GetStandingAttacks() { return attackData.standingAttacks; }
    public AttackData[] GetCrouchingAttacks() { return attackData.crouchingAttacks; }
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

    [Button]
    void GenerateAnimationData()
    {
        CharacterAnimationsData animations = new CharacterAnimationsData();
        AssetDatabase.CreateAsset(animations, $"Assets/ScripteableObjects/{characterName}/{characterName}AnimationsData.asset");
        // Load Animations from FBX
        string assetPath = AssetDatabase.GetAssetPath(fbxAsset);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        List<AnimationClip> clips = new List<AnimationClip>();
        foreach (Object asset in assets)
        {
            if (asset is AnimationClip)
            {
                clips.Add(asset as AnimationClip);
            }
        }
        foreach (AnimationClip clip in clips)
        {
            AnimationData animationData = new AnimationData();
            animationData.name = clip.name.Split("|")[1];
            AssetDatabase.CreateAsset(animationData, $"Assets/ScripteableObjects/{characterName}/{characterName}Animations/{animationData.name}.asset");
        }
        characterAnimationsData = animations;
    }
}
