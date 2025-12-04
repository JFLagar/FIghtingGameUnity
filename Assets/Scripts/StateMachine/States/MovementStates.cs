using SkillIssue.CharacterSpace;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.Text;

namespace SkillIssue.StateMachineSpace
{
    public class StandingState : BaseState
    {
        public StandingState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            player.CharacterMove();
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            player.GetCharacterAnimation().QueueMovementState(player.GetCharacterAnimationsData().standingClips.FirstOrDefault());
            player.ResetAirActions();
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            player.CheckAndFlipCharacterModel();
            if (player.GetInputDirection().y > 0 && player.CanJump())
                player.PerformJump();      
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }
    }

    public class CrouchingState : BaseState
    {
        public CrouchingState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            //Handle Movement
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
            player.GetCharacterAnimation().QueueMovementState(player.GetCharacterAnimationsData().crouchingClip);
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            //Handle Animations
            player.CheckAndFlipCharacterModel();
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }
    }

    public class JumpingState : BaseState
    {
        public JumpingState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            if (player.IsApplyingGravity)
                player.ApplyGravity();
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            player.SetDoubleJump(false);
        }

        public override void OnExit()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            if (!player.IsStillInMovement() && !player.IsApplyingGravity)
            {
                player.SetApplyGravity(true);
            }
            if (player.CanDoubleJump && player.CanJump())
            {
                if (player.GetInputDirection().y > 0)
                {
                    player.PerformJump();
                    player.SetDoubleJump(false);
                }
            }
            if (player.WasYReleased())
            {
                player.SetDoubleJump(true);
            }
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }
    }

}