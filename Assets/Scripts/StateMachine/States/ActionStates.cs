using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace SkillIssue.StateMachineSpace
{
    public class AttackState : BaseState
    {
        public AttackState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            player._inputAction += ProcessInput;

            player._jumpAction += Jump;
            player._dashAction += Dash;
            player._quarterMeterAction += GuardBreak;
            player._inputAction += ProcessInput;
            player._overdriveAction += Overdrive;
            player._halfMeterAction += Rapid;
            player._onAnimationEnd += OnAnimationEnd;
        }

        public override void OnExit()
        {
            base.OnExit();
            player._inputAction -= ProcessInput;

            player._jumpAction -= Jump;
            player._dashAction -= Dash;
            player._quarterMeterAction -= GuardBreak;
            player._inputAction -= ProcessInput;
            player._overdriveAction -= Overdrive;
            player._halfMeterAction -= Rapid;
            player._onAnimationEnd -= OnAnimationEnd;
        }


        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }

        public override void ProcessInput(InputType inputType)
        {
            base.ProcessInput(inputType);
            switch(inputType)
            {
                case InputType.LMHU:
                    player.PerformOverdrive();
                    break;
                case InputType.LMH:
                    player.PerformHalfMeterAction();
                    break;
                case InputType.MH:
                    player.PerformQuarterMeterAction();
                    break;
                default:
                    player.PerformAttack(inputType);
                    break;
            }
        }

        void Jump()
        {
            if (!player.CanJump())
                return;
            player.StartJumping();
        }

        void Dash()
        {
            if (player.MovementDirectionX != player.FaceDir)
                return;
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().jumpingClips[2], Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
            player.ApplyForce(new Vector2(player.FaceDir * Managers.Instance.GameManager.GetCombatValues().GetDashMultiplier(), 0f),
                Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
        }

        void Overdrive()
        {
            Debug.Log("Overdrive");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().FirstOrDefault());
        }

        void Attack(InputType inputType)
        {

        }

        void GuardBreak()
        {
            Debug.Log("GuardBreak");
            // Perform here the attack
        }
        
        void Rapid()
        {
            Debug.Log("Rapid");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().FirstOrDefault());
        }
    }

    public class BlockState : BaseState
    {
        bool barrier = false;
        public BlockState(Player player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            player._halfMeterAction += AttackBreak;
            player._overdriveAction += Overdrive;
            player._inputAction += ProcessInput;
            player._onAnimationEnd += OnAnimationEnd;
        }

        public override void OnExit()
        {
            base.OnExit();
            player._halfMeterAction -= AttackBreak;
            player._overdriveAction -= Overdrive;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
        }

        public override void Update()
        {
            base.Update();
            if (barrier)
            {
                player.ConsumeBarrier();
            }
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
                case InputType.MH:
                    if (player.GetInputDirection().x == player.FaceDir)
                        player.PerformQuarterMeterAction();
                    else
                        ToggleBarrier(true);
                    break;
                default:
                    if(barrier == true)
                        ToggleBarrier(false);
                    break;
            }
        }

        void Overdrive()
        {
            Debug.Log("Overdrive");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().FirstOrDefault());
        }

        void AttackBreak()
        {
            Debug.Log("AttackBreak");
            //Perform here an attack
        }

        void ToggleBarrier(bool value)
        {
            barrier = value;
        }
    }

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
            base.OnEnter();
            player._overdriveAction += Burst;
            player._inputAction += ProcessInput;
            player._onAnimationEnd += OnAnimationEnd;
        }

        public override void OnExit()
        {
            base.OnExit();
            player._overdriveAction -= Burst;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }

        public override void ProcessInput(InputType inputType)
        {
            base.ProcessInput(inputType);
        }

        void Burst()
        {
            Debug.Log("Burst");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().LastOrDefault());
            //Perform here the attack
        }
    }
}