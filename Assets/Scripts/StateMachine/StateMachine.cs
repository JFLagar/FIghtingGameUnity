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
        private Character character;

        State standingState = new StandingState();
        State crouchingState = new CrouchState();
        State jumpState = new JumpState();
        State currentState;
        public ActionStates CurrentAction {  get; private set; }
        public States State {  get; private set; }

        public void Initialize(Character controllingCharacter)
        {
            character = controllingCharacter;
            standingState.Setup(this, character);
            crouchingState.Setup(this, character);
            jumpState.Setup(this, character);

            currentState = standingState;
            currentState.Setup(this, character);
            currentState.EnterState();
        }

        public ActionStates GetActionState()
        {
            return CurrentAction;
        }

        public void SetCurrentActionState(ActionStates state)
        {
            CurrentAction = state;
        }

        public States GetState()
        {
            return State;
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
            State = states;
        }

        // Update is called once per frame
        public void StateMachineUpdate()
        {
            if (currentState.StateMachine == null)
            {
                currentState.Setup(this, character);
            }
            currentState.Update();
        }

    }

    //Abstract class
    public class State : IState
    {
        public StateMachine StateMachine { get; private set; }
        public Character Character { get; private set; }
        public virtual void Update()
        {
        }
        public virtual void EnterState()
        {
        }
        public virtual void ExitState()
        {
        }
        public void Setup(StateMachine stateMachine, Character character)
        {
            StateMachine = stateMachine;
            Character = character;
        }
    }

    public class StandingState : State
    {
        bool canAct;
        public override void Update()
        {
            if (!Character.IsGrounded)
            {
                ExitState();
            }
            canAct = (StateMachine.GetActionState() == ActionStates.None || StateMachine.GetActionState() == ActionStates.Attack);
            if (!canAct)
            {
                return;
            }
            if (Character.GetInputDirection().y != 0)
            {
                if (Character.GetInputDirection().y > 0 && !Character.CanJump())
                    return;
                ExitState();
            }

        }
        public override void EnterState()
        {
            Character.SetApplyGravity(false);
            Character.GetCharacterAnimation().ChangeMovementState(Character.GetCharacterAnimationsData().standingClips.FirstOrDefault());
            Character.ResetAirActions();
            StateMachine.SetCurrentState(this, States.Standing);

        }
        public override void ExitState()
        {
            if (Character.GetInputDirection().y > 0 || !Character.IsApplyingGravity)
            {
                if (Character.CanJump())
                    Character.PerformJump();
                StateMachine.GetJumpState().EnterState();
            }
            else
            {
                if (StateMachine.GetActionState() == ActionStates.None)
                {
                    Character.GetCharacterAnimation().PlayActionAnimation(Character.GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
                    StateMachine.GetCrouchingState().EnterState();
                }
            }
        }
    }
    public class CrouchState : State
    {
        bool action;
        public override void Update()
        {
            action = StateMachine.GetActionState() == ActionStates.None;
            if (!Character.IsGrounded)
                ExitState();
            if (!action)
            {
                return;
            }
            if (Character.GetInputDirection().y != -1)
            {
                ExitState();
            }
        }
        public override void EnterState()
        {
            Character.SetApplyGravity(false);
            StateMachine.SetCurrentState(this, States.Crouching);
            Character.GetCharacterAnimation().ChangeMovementState(Character.GetCharacterAnimationsData().crouchingClip);
        }
        public override void ExitState()
        {
            if (Character.IsGrounded)
            {
                if (StateMachine.GetActionState() == ActionStates.None)
                    Character.GetCharacterAnimation().PlayActionAnimation(Character.GetCharacterAnimationsData().stateTransitionClips.FirstOrDefault());
                StateMachine.GetStandingState().EnterState();
            }
            else
            {
                StateMachine.GetJumpState().EnterState();
            }
        }
    }
    public class JumpState : State
    {
        public override void Update()
        {

            if (!Character.IsStillInMovement() && !Character.IsApplyingGravity)
            {
                Character.SetApplyGravity(true);
                if (StateMachine.GetActionState() == ActionStates.Hit)
                    Character.GetCharacterAnimation().PlayActionAnimation(Character.GetCharacterAnimationsData().hitClips.Last());
            }
            if (Character.CanDoubleJump)
            {
                if (Character.GetInputDirection().y > 0)
                {
                    Character.PerformJump();
                    Character.SetDoubleJump(false);
                }
            }
            if (Character.WasYReleased())
            {
                Character.SetDoubleJump(true);
            }
            if (Character.IsApplyingGravity)
            {
                Character.ApplyGravity();
            }
            if (Character.IsGrounded && !Character.IsJumping)
                ExitState();

        }
        public override void EnterState()
        {
            Character.SetIsGrounded(false);
            StateMachine.SetCurrentState(this, States.Jumping);
        }
        public override void ExitState()
        {
            Character.FixPosition();
                Character.SetDoubleJump(false);
            if (StateMachine.GetActionState() == ActionStates.Attack)
            {
                StateMachine.SetCurrentActionState(ActionStates.None);
                Character.ResetAttackSequence();
            }
            if (StateMachine.GetActionState() == ActionStates.Hit)
                Character.GetCharacterAnimation().PlayActionAnimation(Character.GetCharacterAnimationsData().hitClips.Last());
            if (Character.GetInputDirection().y != -1)
            {
                    Character.GetCharacterAnimation().PlayActionAnimation(Character.GetCharacterAnimationsData().stateTransitionClips.FirstOrDefault());
                StateMachine.GetStandingState().EnterState();
            }
            else
            {
                    Character.GetCharacterAnimation().PlayActionAnimation(Character.GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
                StateMachine.GetCrouchingState().EnterState();
            }
        }
    }

}