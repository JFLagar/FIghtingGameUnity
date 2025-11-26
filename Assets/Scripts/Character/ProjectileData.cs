using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using SkillIssue.Inputs;
using Unity.VisualScripting;
using NaughtyAttributes;
using SkillIssue.StateMachineSpace;
namespace SkillIssue
{
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
        [SerializeField]
        AttackData attackData;
        [SerializeField]
        Vector2 trajectory;
        [SerializeField]
        Sprite sprite;
        [SerializeField]
        AnimationClip animationClip;
        [SerializeField]
        float duration;
        [SerializeField]
        int hitpoints;
        [SerializeField]
        float speed;

        public AttackData GetAttackData() { return attackData; }
        public Sprite GetSprite() { return sprite; }
        public Vector2 GetTrajectory() { return trajectory; }
        public AnimationClip GetAnimationClip() { return animationClip; }
        public float GetDuration() { return duration; }
        public int GetHitPoints() { return hitpoints; }
        public float GetSpeed() { return speed; }
    }
}
