using SkillIssue.CharacterSpace;
using UnityEngine;

namespace SkillIssue.StateMachineSpace
{
    public class CrouchingState : BaseState
    {
        public CrouchingState(Player player) : base(player)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            Debug.Log("Enter Crouching");
        }

        public override void OnExit()
        {
            Debug.Log("Exit Crouching");
        }

        public override void Update()
        {
            base.Update();
        }
    }

}