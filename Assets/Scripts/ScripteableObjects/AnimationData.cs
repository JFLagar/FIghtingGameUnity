using UnityEngine;
using UnityEngine.Events;
using System;

namespace SkillIssue.Animations
{
    [Serializable]
    public class AnimationData
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
        [SerializeField]
        private UnityEvent animationEvent;
        public AnimationClip AnimationClip() { return animationClip; }
        public FrameEvent[] FrameEvents() { return frameEvents;}
        public UnityEvent AnimationEvent() { return animationEvent;}
    }
}


