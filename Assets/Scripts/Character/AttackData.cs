
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using SkillIssue.Inputs;
using Unity.VisualScripting;
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
        public MotionInputs motionInput;

        [SerializeField, Unity.Collections.ReadOnly]
        private int TotalFrames;
        public int startupFrames;
        public int activeFrames;
        public int recoveryFrames;
        public int numberOfHitboxes;
        public int numberOfExtraHits;
        public int extraHitsDelayFrames;

        [Space]
        public int attackLevel = 0; //0 to 5
        public int extraHitstun;
        public int damage;
        public Vector2 push;
        public Vector2 movement;
        public int movementFrame;
        public float movementDuration;
        [Space]
        public bool launcher;
        public bool hardKnockdown;
        public bool dashCancel;
        public bool jumpCancel;
        public bool grab;
        public bool canceleableSelf;
        public InputType[] cancelableTypes;
        [Space]
        public AnimationClip animation;
        public string message;
        public AttackData followUpAttack;
        public AudioClip collideSound;

        private void OnValidate()
        {
            TotalFrames = startupFrames + activeFrames+recoveryFrames;
        }
    }
}