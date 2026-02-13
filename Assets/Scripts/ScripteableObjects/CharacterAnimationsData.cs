using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillIssue.Animations
{
    [CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "Scriptable Objects/CharacterAnimationsData")]
    public class CharacterAnimationsData : ScriptableObject
    {
        //0 Standing and Walking
        [SerializeField]
        public AnimationData[] standingClips;
        [SerializeField]
        public AnimationData[] jumpingClips;
        [SerializeField]
        public AnimationData crouchingClip;
        [SerializeField]
        public AnimationData[] hitClips;
        [SerializeField]
        public AnimationData[] stateTransitionClips;
        [SerializeField]
        public AnimationData[] wakeupClips;
        [SerializeField]
        public AnimationData[] recoveryClips;
        [SerializeField]
        public AnimationData[] cancelClips;
        [SerializeField]
        public AnimationData[] blockingClips;

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
        int frame;
        [SerializeField]
        AnimationData.EventType type;
        [SerializeField]
        List<CollisionData> hitboxes = new List<CollisionData>();
        [SerializeField]
        List<CollisionData> hurtboxes = new List<CollisionData>();
        public int Frame{ get { return frame; } set { frame = value; } }
        public List<CollisionData> Hitboxes() { return hitboxes; }
        public List<CollisionData> Hurtboxes() { return hurtboxes; }
        public AnimationData.EventType Type() { return type; }
    }

    [Serializable]
    public struct CollisionData
    {
        [SerializeField]
        Vector3 size;
        [SerializeField]
        Vector3 position;

        public Vector3 Size() { return size; }
        public Vector3 Position() { return position; }
    }
}


