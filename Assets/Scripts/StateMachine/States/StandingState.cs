using SkillIssue.CharacterSpace;
using UnityEngine;

namespace SkillIssue.StateMachineSpace
{
    public class StandingState : BaseState
    {
        public StandingState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            Debug.Log("Enter Standing");
        }

        public override void OnExit()
        {
            Debug.Log("Exit Standing");
        }

        public override void Update()
        {
            base.Update();
        }
    }

}