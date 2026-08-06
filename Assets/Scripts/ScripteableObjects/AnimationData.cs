using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField]
        [ReadOnly]
        public int actionID;
        [SerializeField]
        [ReadOnly]
        public int animationID;


        public void SetAnimationClip(AnimationClip clip)
        {
            animationClip = clip;
        }

        public AnimationClip AnimationClip() { return animationClip; }
        public List<FrameEvent> FrameEvents() { return frameEvents; }
    }
}


