using SkillIssue.CharacterSpace;
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
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
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
    }

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
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
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
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnAnimationEnd()
        {
            base.OnAnimationEnd();
        }
    }
}