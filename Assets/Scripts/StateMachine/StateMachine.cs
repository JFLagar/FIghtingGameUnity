using SkillIssue.CharacterSpace;
using System;
using System.Collections.Generic;
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
    public class StateMachine
    {
        StateNode current;
        private Player player;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();
        public ActionStates CurrentAction { get; private set; }
        public States State { get; private set; }

        public void Initialize(Player controllingPlayer)
        {
            player = controllingPlayer;
            StandingState standingState = new StandingState(controllingPlayer);
            CrouchingState crouchingState = new CrouchingState(controllingPlayer);
            JumpingState jumpingState = new JumpingState(controllingPlayer);

            AddAnyTransition(jumpingState, new FuncPredicate(() => !player.IsGrounded));
    
            AddTransition(jumpingState, standingState, new FuncPredicate(() => player.IsGrounded));
            AddTransition(crouchingState, standingState, new FuncPredicate(() => player.GetInputDirection().y >= 0));
            AddTransition(standingState, crouchingState, new FuncPredicate(()=> player.GetInputDirection().y < 0));

            SetState(standingState);
        }

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }
            current.State?.Update();
        }

        public void FixedUpdate()
        {
            current.State?.FixedUpdate();
        }

        // Set Default State
        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        void ChangeState(IState state)
        {
            if (state == current.State) return;

            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState?.OnEnter();
            current = nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            foreach (var transition in current.Transitions)
            { 
                if (transition.Condition.Evaluate())
                    return transition; 
            }

            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());
            if (node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }

        class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }

        //OLD All these calls should be working inside the state class
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
    }



    //Abstract class
    public class BaseState : IState
    {
        protected readonly Player player;

        protected BaseState(Player player)
        {
            this.player = player;
        }
        public virtual void Update()
        {
        }
        public virtual void OnEnter()
        {
        }
        public virtual void OnExit()
        {
        }
        public virtual void FixedUpdate()
        {

        }
    }

}