using SkillIssue.CharacterSpace;
using UnityEngine;

namespace SkillIssue.StateMachineSpace
{
    public class HitState : BaseState
    {
        public HitState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            Debug.Log("Enter Hit");
        }

        public override void OnExit()
        {
            Debug.Log("Exit Hit");
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            
        }
    }
}