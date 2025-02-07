using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SkillIssue
{
    public enum AttackType
    {
        Light,
        Heavy,
        Special,
        Grab
    }
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
        public AttackType attackType;
        [Space]
        public int damage;
        public int hitstun;
        public int blockstun;
        public int proratio;
        public Vector2 push;
        public Vector2 movement;
        public float movementDuration;
        [Space]
        public bool launcher;
        public bool hardKnockdown;
        public bool dashCancel;
        public bool jumpCancel;
        public bool grab;
        public bool canceleableSelf;
        public AttackType[] cancelableTypes;
        [Space]
        public AnimationClip animation;
        public string message;
        public AttackData followUpAttack;
        public AudioClip collideSound;
    }
}