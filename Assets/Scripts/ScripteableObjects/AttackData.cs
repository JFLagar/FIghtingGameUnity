
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using SkillIssue.Inputs;
using Unity.VisualScripting;
using NaughtyAttributes;
namespace SkillIssue
{
    public enum AttackAttribute
    {
        Mid,
        Low,
        High
    }
    public enum AttackState
    {
        Standing,
        Crouching,
        Jumping
    }
    [CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObjects/Attacks", order = 1)]
    public class AttackData : ScriptableObject
    {
        public AttackState attackState;
        public AttackAttribute attackAttribute;
        public InputType inputType;
        public bool IsSpecial;
        [ShowIf("IsSpecial")]
        public MotionInputs motionInput = MotionInputs.NONE;

        [Space]
        public int attackLevel = 0; //0 to 5
        public int damage;

        [Space]
        public bool launcher;
        public bool hardKnockdown;
        public bool dashCancel;
        public bool jumpCancel;
        public bool grab;
        public bool canceleableSelf;
        public bool specialCanceleable;
        public InputType[] cancelableTypes;

        [Space]
        public int extraHitstun;
        public Vector2 extraPush;

        [Space]
        public AnimationClip animation;
        public string message;
        public AttackData followUpAttack;
        public AudioClip collideSound;

    }
}