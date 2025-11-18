using NaughtyAttributes;
using SkillIssue.CharacterSpace;
using System.Linq;
using UnityEngine;
namespace SkillIssue.StateMachineSpace
{
    public enum ActionStates
    {
        None, // Default state
        Landing, //To avoid instawalking
        Attack, //Can go back to None or proper Attack, getting hit here will trigger counterhit
        Block, //Goes back to None
        Hit, //King = Overrides all States and Goes back to None
    }
    public enum States
    {
        Standing,
        Crouching,
        Jumping
    }
    public class StateMachine : MonoBehaviour
    {
        [SerializeField]
        Character character;

        State standingState = new StandingState();
        State crouchingState = new CrouchState();
        State jumpState = new JumpState();
        State currentState;
        [SerializeField]
        [ReadOnly]
        ActionStates currentAction = 0;
        States state = 0;

        public void Initialize(Character controllingCharacter)
        {
            character = controllingCharacter;
            standingState.stateMachine = this;
            crouchingState.stateMachine = this;
            jumpState.stateMachine = this;

            currentState = standingState;
            currentState.stateMachine = this;
            currentState.EnterState();
        }

        public Character GetCharacter()
        {
            return character;
        }

        public ActionStates GetActionState()
        {
            return currentAction;
        }

        public void SetCurrentActionState(ActionStates state)
        {
            currentAction = state;
        }

        public States GetState()
        {
            return state;
        }

        public State GetCurrentState()
        {
            return currentState;
        }

        public State GetStandingState()
        {
            return standingState;
        }

        public State GetJumpState()
        {
            return jumpState;
        }

        public State GetCrouchingState()
        {
            return crouchingState;
        }

        public void SetCurrentState(State newState, States states)
        {
            currentState = newState;
            state = states;
        }

        // Update is called once per frame
        public void StateMachineUpdate()
        {
            if (currentState.stateMachine == null)
            {
                currentState.stateMachine = this;
            }
            currentState.Update();
        }

    }

    //Abstract class
    public class State : IState
    {
        public StateMachine stateMachine;
        public virtual void Update()
        { }
        public virtual void EnterState()
        {
        }
        public virtual void ExitState()
        {
        }
    }

    public class StandingState : State
    {
        bool canAct;
        public override void Update()
        {
            if (!stateMachine.GetCharacter().IsGrounded())
            {
                ExitState();
            }
            canAct = (stateMachine.GetActionState() == ActionStates.None || stateMachine.GetActionState() == ActionStates.Attack);
            if (!canAct)
            {
                return;
            }
            if (stateMachine.GetCharacter().GetInputDirection().y > 0 && stateMachine.GetCharacter().CanJump())
            {
                stateMachine.GetCharacter().PerformJump();
                //jump
                ExitState();
            }

        }
        public override void EnterState()
        {
            stateMachine.GetCharacter().SetApplyGravity(false);
            stateMachine.GetCharacter().GetCharacterAnimation().ChangeMovementState(stateMachine.GetCharacter().GetCharacterAnimationsData().standingClips.FirstOrDefault());
            stateMachine.GetCharacter().ResetAirActions();
            stateMachine.SetCurrentState(this, States.Standing);

        }
        public override void ExitState()
        {
            if (stateMachine.GetCharacter().GetInputDirection().y > 0 || !stateMachine.GetCharacter().IsGrounded())
            {
                stateMachine.GetCharacter().SetIsJumping(true);
                stateMachine.GetJumpState().EnterState();
            }
            else
            {
                if (stateMachine.GetActionState() == ActionStates.None)
                {
                    stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
                    stateMachine.GetCrouchingState().EnterState();
                }
            }
        }
    }
    public class CrouchState : State
    {
        bool action;
        public override void Update()
        {
            action = stateMachine.GetActionState() == ActionStates.None;
            if (!stateMachine.GetCharacter().IsGrounded())
                ExitState();
            if (!action)
            {
                return;
            }
            if (stateMachine.GetCharacter().GetInputDirection().y != -1)
            {
                ExitState();
            }
        }
        public override void EnterState()
        {
            stateMachine.GetCharacter().SetApplyGravity(false);
            stateMachine.SetCurrentState(this, States.Crouching);
            stateMachine.GetCharacter().GetCharacterAnimation().ChangeMovementState(stateMachine.GetCharacter().GetCharacterAnimationsData().crouchingClip);
        }
        public override void ExitState()
        {
            if (stateMachine.GetCharacter().IsGrounded())
            {
                if (stateMachine.GetActionState() == ActionStates.None)
                    stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().stateTransitionClips.FirstOrDefault());
                stateMachine.GetStandingState().EnterState();
            }
            else
            {
                stateMachine.GetCharacter().SetIsJumping(true);
                stateMachine.GetJumpState().EnterState();
            }
            stateMachine.GetCharacter().CharacterMove();
        }
    }
    public class JumpState : State
    {
        public override void Update()
        {

            if (!stateMachine.GetCharacter().IsStillInMovement() && !stateMachine.GetCharacter().GetApplyGravity())
            {
                stateMachine.GetCharacter().SetApplyGravity(true);
                if (stateMachine.GetActionState() == ActionStates.Hit)
                    stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().hitClips.Last());
            }
            if (stateMachine.GetCharacter().CanDoubleJump())
            {
                if (stateMachine.GetCharacter().GetInputDirection().y > 0)
                {
                    stateMachine.GetCharacter().PerformJump();
                    stateMachine.GetCharacter().SetDoubleJump(false);
                }
            }
            if (stateMachine.GetCharacter().WasYReleased())
            {
                stateMachine.GetCharacter().SetDoubleJump(true);
            }
            if (stateMachine.GetCharacter().GetApplyGravity())
            {
                stateMachine.GetCharacter().ApplyGravity();
            }
            if (stateMachine.GetCharacter().IsGrounded() && !stateMachine.GetCharacter().IsJumping())
                ExitState();

        }
        public override void EnterState()
        {
            stateMachine.GetCharacter().SetIsGrounded(false);
            stateMachine.SetCurrentState(this, States.Jumping);
        }
        public override void ExitState()
        {
            stateMachine.GetCharacter().FixPosition();
            stateMachine.GetCharacter().SetDoubleJump(false);
            if (stateMachine.GetActionState() == ActionStates.Attack)
                stateMachine.SetCurrentActionState(ActionStates.None);
            if (stateMachine.GetActionState() == ActionStates.Hit)
                stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().hitClips.Last());
            if (stateMachine.GetCharacter().GetInputDirection().y != -1)
            {
                    stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().stateTransitionClips.FirstOrDefault());
                stateMachine.GetStandingState().EnterState();
            }
            else
            {
                    stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
                stateMachine.GetCrouchingState().EnterState();
            }
        }
    }

}