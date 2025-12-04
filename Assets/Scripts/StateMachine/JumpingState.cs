using SkillIssue.CharacterSpace;
using UnityEngine;

namespace SkillIssue.StateMachineSpace
{
    public class JumpingState : BaseState
    {
        public JumpingState(Character character) : base(character)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            Debug.Log("Enter Jumping");
        }

        public override void OnExit()
        {
            Debug.Log("Enter Jumping");
        }

        public override void Update()
        {
            base.Update();
        }
    }

}