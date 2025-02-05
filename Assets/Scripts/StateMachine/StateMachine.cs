using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using UnityEngine;
namespace SkillIssue.StateMachineSpace
{
    public enum ActionStates
    {
        None, // Default state
        Landing, //To avoid instawalking
        Attack, //Can go back to None or proper Attack, getting hit here will trigger counterhit
        Block, //Goes back to None
        Hit //King = Overrides all States and Goes back to None
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
        bool action;
        private float yvalue;
        public override void Update()
        {
            if (!stateMachine.GetCharacter().GetIsGrounded())
            {
                yvalue = 1;
                ExitState();
            }
            action = (stateMachine.GetActionState() == ActionStates.None || stateMachine.GetActionState() == ActionStates.Hit);
            if (!action)
            {
                return;
            }
            if (stateMachine.GetActionState() == ActionStates.None)
            {
                if (stateMachine.GetCharacter().GetInputDirection().y != 0)
                {
                    yvalue = stateMachine.GetCharacter().GetInputDirection().y;
                    if (yvalue > 0)
                        stateMachine.GetCharacter().ApplyForce(new Vector2(stateMachine.GetCharacter().GetInputDirection().x, 1f), stateMachine.GetCharacter().GetJumpPower());
                    //jump
                    ExitState();
                }
                stateMachine.GetCharacter().CharacterMove();
            }

        }
        public override void EnterState()
        {
            stateMachine.GetCharacter().SetApplyGravity(false);
            stateMachine.SetCurrentState(this, States.Standing);

        }
        public override void ExitState()
        {

            if (yvalue == 1)
            {
                stateMachine.GetCharacter().SetIsJumping(true);
                stateMachine.GetJumpState().EnterState();
            }
            else
            {
                stateMachine.GetCrouchingState().EnterState();
            }
        }
    }
    public class CrouchState : State
    {
        bool action;
        public override void Update()
        {
            action = stateMachine.GetActionState() == ActionStates.None;
            if (!stateMachine.GetCharacter().GetIsGrounded())
                ExitState();
            if (!action)
            {
                return;
            }
            if (stateMachine.GetCharacter().GetInputDirection().y != -1)
            {
                ExitState();
            }
            //Gets Input for blocking
            stateMachine.GetCharacter().CharacterMove();
        }
        public override void EnterState()
        {
            stateMachine.SetCurrentState(this, States.Crouching);
            if (stateMachine.GetActionState() == ActionStates.None)
                stateMachine.GetCharacter().GetCharacterAnimation().AddAnimation(AnimType.Movement, "StandToCrouch");
            stateMachine.GetCharacter().GetAnimator().SetBool("Crouching", true);
        }
        public override void ExitState()
        {
            if (stateMachine.GetCharacter().GetIsGrounded())
            {
                stateMachine.GetStandingState().EnterState();
                stateMachine.GetCharacter().GetAnimator().SetBool("Crouching", false);
            }
            else
            {
                stateMachine.GetJumpState().EnterState();
            }
            stateMachine.GetCharacter().CharacterMove();
        }
    }
    public class JumpState : State
    {
        public override void Update()
        {

            if (!stateMachine.GetCharacter().IsStillInMovement())
                stateMachine.GetCharacter().SetApplyGravity(true);
            stateMachine.GetCharacter().ApplyGravity();
            if (stateMachine.GetCharacter().GetIsGrounded() && !stateMachine.GetCharacter().GetIsJumping())
                ExitState();

        }
        public override void EnterState()
        {
            stateMachine.GetCharacter().SetIsGrounded(false);
            stateMachine.SetCurrentState(this, States.Jumping);
            if (stateMachine.GetActionState() == ActionStates.None || stateMachine.GetActionState() == ActionStates.Landing)
                stateMachine.GetCharacter().GetCharacterAnimation().AddAnimation(AnimType.Movement, "JumpStart");
            stateMachine.GetCharacter().GetAnimator().SetBool("Jumping", true);
        }
        public override void ExitState()
        {
            stateMachine.GetCharacter().FixPosition();
            stateMachine.GetStandingState().EnterState();
            stateMachine.GetCharacter().GetAnimator().SetBool("Jumping", false);
            if (stateMachine.GetActionState() != ActionStates.Hit)
            {
                stateMachine.SetCurrentActionState(ActionStates.Landing);
                stateMachine.GetCharacter().GetCharacterAnimation().AddAnimation(AnimType.Landing, "LandingRecovery");
            }
        }
    }
}