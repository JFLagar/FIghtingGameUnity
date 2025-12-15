using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
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
            if (player.IsApplyingGravity)
                player.ApplyGravity();
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
            if (player.GetInputDirection().y > 0)
                player.PerformJump();
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
                    if (!player.PerformOverdrive())
                        Attack(InputType.Unique);
                    break;
                case InputType.LMH:
                    if (!player.PerformHalfMeterAction())
                        Attack(InputType.Heavy);
                    break;
                case InputType.MH:
                    if (!player.PerformQuarterMeterAction())
                        Attack(InputType.Heavy);
                    break;
                default:
                    Attack(inputType);
                    break;
            }
        }

        void Attack(InputType inputType)
        {
            if (inputType == InputType.Heavy || inputType == InputType.Unique)
                return;
            AttackData attackData;
            if (inputType == InputType.LU)
            {
                if (!player.IsGrounded)
                    attackData = player.GetCharacterData().GetGrabData()[1];
                else
                    attackData = player.GetCharacterData().GetGrabData()[0];
                player.ProcessAttack(attackData);
                return;
            }
            if (!player.IsGrounded)
            {
                attackData = player.GetCharacterData().GetJumpAttacks()[(int)inputType];
                player.ProcessAttack(attackData);
                return;
            }

            if (player.GetInputDirection().y < 0)
                attackData = player.GetCharacterData().GetCrouchingAttacks()[(int)inputType];
            else if (player.GetInputDirection().x == player.FaceDir)
                attackData = player.GetCharacterData().GetForwardAttacks()[(int)inputType];
            else
                attackData = player.GetCharacterData().GetStandingAttacks()[((int)inputType)];

            player.ProcessAttack(attackData);
        }

        void Jump()
        {
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
            player._blockAction += Block;

            Block();    
        }

        public override void OnExit()
        {
            base.OnExit();
            player._halfMeterAction -= AttackBreak;
            player._overdriveAction -= Overdrive;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
            player._blockAction -= Block;
        }

        private void Block()
        {
            AttackData attack = player.HitAttack;
            player.PlaySound(attack.GetCollideAudioClip());
            Vector2 dir = CalculateHitPush(attack);
            Vector2 blockDir = new(dir.x, 0);

            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().blockingClips[0]);
            if (player.IsAgainstTheWall && player.FaceDir != player.WallFaceDirection && attack.GetProjectileData() == null)
            {
                player.ApplyCounterPush(-blockDir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
            }
            else
                player.ApplyForce(blockDir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
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
                    if (barrier == true)
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

        private Vector2 CalculateHitPush(AttackData attack)
        {
            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            Vector2 result = new Vector2();
            result.x = ((attackLevel) + attack.GetExtraPush().x) * -player.FaceDir;
            if (attack.CausesLaunch() || player.IsKnockedDown() || stateMachine.GetPreviousMovementState() is JumpingState)
            {
                if (result.y == 0)
                {
                    result.y = 1;
                }
                result.y = (attackLevel) + attack.GetExtraPush().y + Managers.Instance.GameManager.GetCombatValues().GetHitVerticalBase();
            }

            return result;
        }
    }

    public class HitState : BaseState
    {
        public HitState(Player player, StateMachine stateMachine) : base(player, stateMachine)
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
            player._overdriveAction += Burst;
            player._inputAction += ProcessInput;
            player._onAnimationEnd += OnAnimationEnd;
            player._hitAction += Hit;

            Hit();
        }

        public override void OnExit()
        {
            base.OnExit();
            player._overdriveAction -= Burst;
            player._inputAction -= ProcessInput;
            player._onAnimationEnd -= OnAnimationEnd;
            player._hitAction -= Hit;
        }

        void Hit()
        {
            AttackData attack = player.HitAttack;
            Vector2 dir = CalculateHitPush(attack);
            player.PlaySound(attack.GetCollideAudioClip()); ;
            if (attack.CausesLaunch() || player.IsKnockedDown())
            {
                player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().hitClips[2], CalculateHitstun(attack));
            }
            else
            {
                if (attack.IsGrab())
                {
                    player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().hitClips[0]);
                }
                else
                {
                    if (stateMachine.GetPreviousMovementState() is JumpingState)
                    {
                        player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().hitClips[0], CalculateHitstun(attack));
                    }
                    else
                        player.GetCharacterAnimation().PlayHitAnimation(player.GetCharacterAnimationsData().hitClips[0], CalculateHitstun(attack));
                }
            }
            player.StartHitstopCoroutine();
            // Use Event in UI to show HP changes


            //if its a projectile dont push back the attacking character
            if (player.IsAgainstTheWall && player.FaceDir != player.WallFaceDirection && attack.GetProjectileData() == null)
            {
                player.ApplyCounterPush(-dir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
            }
            player.ApplyForce(dir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
            player.PerformRecovery();
        }

        public override void ProcessInput(InputType inputType)
        {
            base.ProcessInput(inputType);
            if (inputType == InputType.LMHU)
                player.PerformOverdrive();
        }

        void Burst()
        {
            Debug.Log("Burst");
            player.GetCharacterAnimation().PlayActionAnimation(player.GetCharacterAnimationsData().GetCancelClips().LastOrDefault());
            //Perform here the attack
        }

        private int CalculateHitstun(AttackData attack)
        {
            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            int result = (attackLevel * 2) + Managers.Instance.GameManager.GetCombatValues().GetHitstunBase() + attack.GetExtraHitstun(); //attacklevel + hitstunbase(10) + extra
            return result;
        }

        private Vector2 CalculateHitPush(AttackData attack)
        {
            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            Vector2 result = new Vector2();
            result.x = ((attackLevel) + attack.GetExtraPush().x) * -player.FaceDir;
            if (attack.CausesLaunch() || player.IsKnockedDown() || stateMachine.GetPreviousMovementState() is JumpingState)
            {
                if (result.y == 0)
                {
                    result.y = 1;
                }
                result.y = (attackLevel) + attack.GetExtraPush().y + Managers.Instance.GameManager.GetCombatValues().GetHitVerticalBase();
            }

            return result;
        }
    }
}