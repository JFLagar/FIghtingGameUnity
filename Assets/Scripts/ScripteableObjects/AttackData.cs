
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using SkillIssue.Inputs;
using Unity.VisualScripting;
using NaughtyAttributes;
using SkillIssue.StateMachineSpace;
namespace SkillIssue
{
    public enum AttackAttribute
    {
        Mid,
        Low,
        High
    }

    public enum CancelTypes
    {
        Dash,
        Jump,
        Self,
        Special,
        Super
    }

    [CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/Attacks", order = 1)]
    public class AttackData : ScriptableObject
    {
        [SerializeField]
        private States attackState;
        [SerializeField]
        private AttackAttribute attackAttribute;
        [SerializeField]
        private InputType inputType;
        [SerializeField]
        private bool isSpecialMove;
        [ShowIf("IsSpecial")]
        [SerializeField]
        private MotionInputs motionInput = MotionInputs.NONE;

        [Space]

        [SerializeField]
        private int attackLevel = 0; //0 to 5
        [SerializeField]
        private int damage;

        [Space]

        [SerializeField]
        private bool isGrab;
        [SerializeField]
        private bool causesLaunch;
        [SerializeField]
        private bool causesHardKnockdown;

        [Space]

        [SerializeField]
        private CancelTypes[] cancelTypes;
        [SerializeField]
        private AttackData[] canceableUniqueAttacks;

        [Space]

        [SerializeField]
        private int extraHitstun;
        [SerializeField]
        private Vector2 extraPush;

        [Space]

        [SerializeField]
        private AnimationClip animation;
        [SerializeField]
        private AttackData followUpAttack;
        [SerializeField]
        private AudioClip collideSound;

        public States GetAttackState() { return attackState; }
        public AttackAttribute GetAttackAttribute() { return attackAttribute; }
        public InputType GetInputType() { return inputType; }
        public bool IsSpecialMove() { return isSpecialMove; }
        public MotionInputs GetMotionInput() { return motionInput; }

        public int GetAttackLevel() { return attackLevel; }
        public int GetDamage() { return damage; }

        public bool IsGrab() { return isGrab; }
        public bool CausesLaunch() { return causesLaunch; }
        public bool CausesHardKnockdown () { return causesHardKnockdown; }

        public CancelTypes[] GetCancelTypes() { return cancelTypes; }
        public AttackData[] GetCancelableUniqueAttacks() { return canceableUniqueAttacks; }

        public int GetExtraHitstun() { return extraHitstun;}
        public Vector2 GetExtraPush() { return extraPush;}

        public AnimationClip GetAnimationClip() {  return animation; }
        public AttackData GetFollowUpAttackData() { return followUpAttack; }
        public AudioClip GetCollideAudioClip() { return collideSound; }


    }
}