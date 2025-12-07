using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using System.Linq;
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
            player.CharacterMove();
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            player.GetCharacterAnimation().QueueMovementState(player.GetCharacterAnimationsData().standingClips.FirstOrDefault());
            player.ResetAirActions();

            player._jumpAction += Jump;
            player._dashAction += Dash;
            player._overdriveAction += Overdrive;
            player._inputAction += ProcessInput;
            player._onAnimationEnd += OnAnimationEnd;
        }

        public override void OnExit()
        {
            base.OnExit();

            player._jumpAction -= Jump;
            player._dashAction -= Dash;
            player._overdriveAction -= Overdrive;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
            stateMachine.SetPreviousMovementState(this);
        }

        public override void Update()
        {
            player.CheckAndFlipCharacterModel();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }

        public override void ProcessInput(InputType inputType)
        {
            base.ProcessInput(inputType);

            switch (inputType)
            {
                case InputType.NONE:
                    break;
                case InputType.LMHU:
                    player.PerformOverdrive();
                    break;
                default:
                    Attack(inputType);
                    break;
            }
        }

        void Attack(InputType inputType)
        {
            AttackData attackData;
            if (inputType == InputType.LU)
            {
                attackData = player.GetCharacterData().GetGrabData()[0];
                player.ProcessAttack(attackData);
                return;
            }
            if (player.GetInputDirection().x == player.FaceDir)
                attackData = player.GetCharacterData().GetForwardAttacks()[(int)inputType];
            else
                attackData = player.GetCharacterData().GetStandingAttacks()[((int)inputType)];
            player.ProcessAttack(attackData);
        }

        void Jump()
        {
            if (!player.CanJump())
                return;
            player.StartJumping();
        }

        void Dash()
        {
            if (player.GetInputDirection().x == player.FaceDir)
                player.SetRunning(true);
            else
            {
                player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().standingClips.LastOrDefault(), Managers.Instance.GameManager.GetCombatValues().GetAirDashAnimationDuration());
                player.ApplyForce(new Vector2(-player.FaceDir * 2, 0.5f), Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
            }
        }

        void Overdrive()
        {
            Debug.Log("Overdrive");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().FirstOrDefault());
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

            player._overdriveAction += Overdrive;
            player._inputAction += ProcessInput;
            player._onAnimationEnd += OnAnimationEnd;
        }

        public override void OnExit()
        {
            base.OnExit();
            player._overdriveAction -= Overdrive;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
            stateMachine.SetPreviousMovementState(this);
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

        public override void ProcessInput(InputType inputType)
        {
            base.ProcessInput(inputType);
            switch (inputType)
            {
                case InputType.NONE:
                    break;
                case InputType.LMHU:
                    player.PerformOverdrive();
                    break;
                default:
                    Attack(inputType);
                    break;
            }
        }

        void Attack(InputType inputType)
        {
            AttackData attackData;
            if (inputType == InputType.LU)
            {
                attackData = player.GetCharacterData().GetGrabData()[0];
                player.ProcessAttack(attackData);
                return;
            }
            attackData = player.GetCharacterData().GetCrouchingAttacks()[((int)inputType)];
            player.ProcessAttack(attackData);
        }

        void Overdrive()
        {
            Debug.Log("Overdrive");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().FirstOrDefault());
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
            player._jumpAction += Jump;
            player._dashAction += Dash;
            player._overdriveAction += Overdrive;
            player._inputAction += ProcessInput;
            player._onAnimationEnd += OnAnimationEnd;
        }

        public override void OnExit()
        {
            base.OnExit();

            player._jumpAction -= Jump;
            player._dashAction -= Dash;
            player._overdriveAction -= Overdrive;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
            stateMachine.SetPreviousMovementState(this);
        }

        public override void Update()
        {
            if (!player.IsStillInMovement() && !player.IsApplyingGravity)
            {
                player.SetApplyGravity(true);
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

        public override void ProcessInput(InputType inputType)
        {
            base.ProcessInput(inputType);
            switch (inputType)
            {
                case InputType.NONE:
                    break;
                case InputType.LMHU:
                    player.PerformOverdrive();
                    break;
                default:
                    Attack(inputType);
                    break;
            }
        }

        void Jump()
        {
            Debug.Log("Jump-Jump");
            if (player.CanDoubleJump && player.CanJump())
            {
                if (player.AirActions > 0)
                    player.SetAirActions(player.AirActions - 1);
                else
                    return;
                player.StartJumping();
                player.SetDoubleJump(false);
            }
            else
                return;
        }

        void Dash()
        {
            if (player.AirActions <= 0)
                return;

            if (player.GetInputDirection().x == player.FaceDir)
            {
                player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().jumpingClips[2], Managers.Instance.GameManager.GetCombatValues().GetAirDashAnimationDuration());
                player.ApplyForce(new Vector2(player.FaceDir * Managers.Instance.GameManager.GetCombatValues().GetDashMultiplier(), 0.1f),
                    Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                player.SetAirActions(player.AirActions - 1);
            }
            else
            {
                player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().standingClips.LastOrDefault(), Managers.Instance.GameManager.GetCombatValues().GetAirDashAnimationDuration());
                player.ApplyForce(new Vector2(-player.FaceDir, 0.1f), Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                player.SetAirActions(player.AirActions - 1);
            }
        }

        void Attack(InputType inputType)
        {
            AttackData attackData;
            if (inputType == InputType.LU)
            {
                attackData = player.GetCharacterData().GetGrabData()[1];
                player.ProcessAttack(attackData);
                return;
            }
            attackData = player.GetCharacterData().GetJumpAttacks()[((int)inputType)];
            player.ProcessAttack(attackData);
        }

        void Overdrive()
        {
            Debug.Log("Overdrive");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().FirstOrDefault());
        }
    }

}