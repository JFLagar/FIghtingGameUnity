using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillIssue.Animations
{
    [Serializable]
    [CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "Scriptable Objects/CharacterAnimationsData")]
    public class CharacterAnimationsData : ScriptableObject
    {
        //0 Standing and Walking
        [SerializeField]
        [Tooltip("0: Standing, 1: FWalk, 2: BWalk, 3:FDash, 4:BDash")]
        public AnimationData[] standingClips;
        [SerializeField]
        [Tooltip("0: Jump, 1: JumpRise, 2: JumpFall")]
        public AnimationData[] jumpingClips;
        [SerializeField]
        public AnimationData crouchingClip;
        [SerializeField]
        [Tooltip("0: Standing, 1: Crouching, 2: Jumping, 3:Launcher, 4:Knockdown")]
        public AnimationData[] hitClips;
        [SerializeField]
        [Tooltip("0: Standing, 1: Crouching, 2: Land")]
        public AnimationData[] stateTransitionClips;
        [SerializeField]
        [Tooltip("0: Front, 1: Back, 2: Quick, 3: Roll")]
        public AnimationData[] wakeupClips;
        [SerializeField]
        [Tooltip("0: Air, 1: Stagger, 2: Throw")]
        public AnimationData[] recoveryClips;
        [SerializeField]
        [Tooltip("0: Pose, 1: Overdrive")]
        public AnimationData[] cancelClips;
        [SerializeField]
        [Tooltip("0: Standing, 1: Crouching, 2: Jumping")]
        public AnimationData[] blockingClips;
        [SerializeField]
        [Tooltip("0: Standing, 1: Crouching, 2: Jumping")]
        public AnimationData[] blockBreakClips;

        public AnimationData[] GetStandingClips() { return standingClips; }
        public AnimationData[] GetJumpingClips() { return jumpingClips; }
        public AnimationData GetCrouchingClip() { return crouchingClip; }
        public AnimationData[] GetBlockingClips() { return blockingClips; }
        public AnimationData[] GetHitClips() { return hitClips; }
        public AnimationData[] GetStateTransitionClips() { return stateTransitionClips; }
        public AnimationData[] GetWakeupClips() { return wakeupClips; }
        public AnimationData[] GetRecoveryClips() { return recoveryClips; }
        public AnimationData[] GetCancelClips() { return cancelClips; }

    }
    [Serializable]
    public class FrameEvent
    {
        [SerializeField]
        int frame = 1;
        [SerializeField]
        AnimationData.EventType type;
        [SerializeField]
        List<CollisionData> hitboxes = new List<CollisionData>();
        [SerializeField]
        List<CollisionData> hurtboxes = new List<CollisionData>();
        public int Frame { get { return frame; } set { frame = value; } }
        public List<CollisionData> Hitboxes() { return hitboxes; }
        public List<CollisionData> Hurtboxes() { return hurtboxes; }
        public AnimationData.EventType Type() { return type; }
    }

    [Serializable]
    public struct CollisionData
    {
        [SerializeField]
        ColliderState state;
        [SerializeField]
        Vector3 size;
        [SerializeField]
        Vector3 position;

        public ColliderState State() { return state; }
        public Vector3 Size() { return size; }
        public Vector3 Position() { return position; }
    }
}


