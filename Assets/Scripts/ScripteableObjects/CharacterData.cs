using NaughtyAttributes;
using SkillIssue;
using SkillIssue.Animations;
using SkillIssue.Inputs;
using System.Collections.Generic;
using System.Linq;
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

        // Load Animations from FBX
        string assetPath = AssetDatabase.GetAssetPath(fbxAsset);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        List<AnimationClip> clips = new List<AnimationClip>();
        List<AnimationData> animationDatas = new List<AnimationData>();
        foreach (Object asset in assets)
        {
            if (asset is AnimationClip && !asset.name.Contains("preview"))
            {
                clips.Add(asset as AnimationClip);
            }
        }

        foreach (AnimationClip clip in clips)
        {
            int actionID = 0;
            int animationID = 0;
            AnimationData animationData = new AnimationData();
            animationData.name = clip.name.Split("|")[1];
            System.Int32.TryParse(animationData.name.Split(".")[0], out actionID);
            System.Int32.TryParse(animationData.name.Split(".")[1], out animationID);
            animationData.actionID = actionID;
            animationData.animationID = animationID;
            animationData.SetAnimationClip(clip);
            AssetDatabase.CreateAsset(animationData, $"Assets/ScripteableObjects/{characterName}/{characterName}Animations/{animationData.name}.asset");
            animationDatas.Add(animationData);
        }
        characterAnimationsData = SortAnimationData(animations, animationDatas);
        AssetDatabase.CreateAsset(animations, $"Assets/ScripteableObjects/{characterName}/{characterName}AnimationsData.asset");
    }

    CharacterAnimationsData SortAnimationData(CharacterAnimationsData reference, List<AnimationData> animationDatas)
    {
        reference.standingClips = animationDatas.Where(c => c.actionID == 0).ToList().OrderBy(c => c.animationID).ToArray();
        reference.crouchingClip = animationDatas.FirstOrDefault(c => c.actionID == 1);
        reference.jumpingClips = animationDatas.Where(c => c.actionID == 2).ToList().OrderBy(c => c.animationID).ToArray();
        reference.stateTransitionClips = animationDatas.Where(c => c.actionID == 3).ToList().OrderBy(c => c.animationID).ToArray();
        reference.hitClips = animationDatas.Where(c => c.actionID == 4).ToList().OrderBy(c => c.animationID).ToArray();
        reference.wakeupClips = animationDatas.Where(c => c.actionID == 5).ToList().OrderBy(c => c.animationID).ToArray();
        reference.recoveryClips = animationDatas.Where(c => c.actionID == 6).ToList().OrderBy(c => c.animationID).ToArray();
        reference.cancelClips = animationDatas.Where(c => c.actionID == 7).ToList().OrderBy(c => c.animationID).ToArray();
        reference.blockingClips = animationDatas.Where(c => c.actionID == 8).ToList().OrderBy(c => c.animationID).ToArray();
        reference.blockBreakClips = animationDatas.Where(c => c.actionID == 9).ToList().OrderBy(c => c.animationID).ToArray();

        return reference;
    }
}
