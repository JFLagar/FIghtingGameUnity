using UnityEngine;
using UnityEngine.Events;
using System;

namespace SkillIssue.Animations
{
    [CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "Scriptable Objects/AnimationsData")]
    public class AnimationData : ScriptableObject
    {
        public enum EventType
        {
            Open, //Change hitboxes/Hurtboxes
            Close, //Call Anim End
            Movement,
            MovementEnd,
            Projectile
        }
        [SerializeField]
        private AnimationClip animationClip;
        [SerializeField]
        FrameEvent[] frameEvents;
        public AnimationClip AnimationClip() { return animationClip; }
        public FrameEvent[] FrameEvents() { return frameEvents;}
    }
}


