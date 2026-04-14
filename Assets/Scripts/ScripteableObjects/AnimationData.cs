using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

namespace SkillIssue.Animations
{
    [CreateAssetMenu(fileName = "CharacterAnimationsData", menuName = "Scriptable Objects/AnimationsData")]
    public class AnimationData : ScriptableObject
    {
        public enum EventType
        {
            CollisionBox, //Change hitboxes/Hurtboxes
            AnimationEnd, //Call Anim End
            Movement,
            MovementEnd,
            Projectile
        }
        [SerializeField]
        private AnimationClip animationClip;
        [SerializeField]
        List<FrameEvent> frameEvents = new List<FrameEvent>();
        public AnimationClip AnimationClip() { return animationClip; }
        public List<FrameEvent> FrameEvents() { return frameEvents;}
    }
}


