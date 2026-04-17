using UnityEngine;
using SkillIssue.Inputs;
using SkillIssue.Animations;
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
        [ShowIf("IsSpecialMove")]
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
        [SerializeField]
        private bool cinematicAttack;

        [Space]

        [SerializeField]
        private CancelTypes[] cancelTypes;
        [SerializeField]
        private AttackData[] canceableAttacks;

        [Space]

        [SerializeField]
        private int extraHitstun;
        [SerializeField]
        private Vector2 extraPush;

        [Space]

        [SerializeField]
        private AnimationData animation;
        [SerializeField]
        private AttackData followUpAttack;
        [SerializeField]
        private ProjectileData followUpProjectile;
        [SerializeField]
        private AudioClip collideSound;
        [SerializeField]
        private Vector2 movementDirection;

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
        public bool IsCinematic() { return cinematicAttack; }

        public CancelTypes[] GetCancelTypes() { return cancelTypes; }
        public AttackData[] GetCancelableAttacks() { return canceableAttacks; }

        public int GetExtraHitstun() { return extraHitstun;}
        public Vector2 GetExtraPush() { return extraPush;}

        public AnimationData GetAnimationClip() {  return animation; }
        public AttackData GetFollowUpAttackData() { return followUpAttack; }
        public AudioClip GetCollideAudioClip() { return collideSound; }
        public ProjectileData GetProjectileData() { return followUpProjectile; }

        public Vector2 GetMovementDirection() { return movementDirection; }
    }
}