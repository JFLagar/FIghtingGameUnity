using System;
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
        public AnimationData[] blockingClips;
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

    [CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData")]
    public class AnimationData : ScriptableObject
    {
        [SerializeField]
        private AnimationClip animationClip;
        [SerializeField]
        private int endFrame;
        [SerializeField]
        private FrameData[] frames;
        public AnimationClip AnimationClip() { return animationClip; }
        public int EndFrame() { return endFrame; }
        public FrameData[] Frames() { return frames; }
    }
    [Serializable]
    public struct FrameData
    {
        [SerializeField]
        int frame;
        [SerializeField]
        CollisionData[] hitboxes;
        [SerializeField]
        CollisionData[] hurtboxes;
        public int Frame() { return frame; }
        public CollisionData[] Hitboxes() { return hitboxes; }
        public CollisionData[] Hurtboxes() { return hurtboxes; }
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


