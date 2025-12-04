using SkillIssue.CharacterSpace;
using UnityEngine;

namespace SkillIssue.StateMachineSpace
{
    public class BlockState : BaseState
    {
        public BlockState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            Debug.Log("Enter Block");
        }

        public override void OnExit()
        {
            Debug.Log("Exit Block");
        }

        public override void Update()
        {
            base.Update();
        }
    }
}