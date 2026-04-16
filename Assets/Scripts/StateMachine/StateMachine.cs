using SkillIssue.CharacterSpace;
using SkillIssue.Inputs;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace SkillIssue.StateMachineSpace
{

    public enum States
    {
        Standing,
        Crouching,
        Jumping
    }
    public class StateMachine
    {
        public bool IsMoveState {  get; private set; }
        StateNode currentState;
        IState previousMovementState;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();
        public States State { get; private set; }

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }
            currentState.State?.Update();
        }

        public void FixedUpdate()
        {
            currentState.State?.FixedUpdate();
        }

        // Set Default State
        public void SetState(IState state)
        {
            currentState = nodes[state.GetType()];
            currentState.State?.OnEnter();
        }

        void ChangeState(IState state)
        {
            if (state == currentState.State) return;

            var previousState = currentState;
            var nextState = nodes[state.GetType()].State;

            previousState.State?.OnExit();
            nextState?.OnEnter();
            currentState = nodes[state.GetType()];
            IsMoveState = CheckMoveState();
        }

        public bool CheckMoveState()
        {
            bool result = false;
            if (currentState.State is CrouchingState)
                return true;
            if (currentState.State is StandingState)
                return true;
            if (currentState.State is JumpingState)
                return true;

            return result;
        }

        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            foreach (var transition in currentState.Transitions)
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

        public IState GetState()
        {
            return currentState.State;
        }

        public void SetPreviousMovementState(IState state)
        {
            previousMovementState = state;
        }

        public IState GetPreviousMovementState()
        {
            return previousMovementState;
        }

        public bool CanAttack()
        {
            bool result = true;
            if (currentState.State is HitState)
                result = false;
            if (currentState.State is BlockState)
                result = false;
            return result;
        }

        public bool CanBlock()
        {
            bool result = true;
            if (currentState.State is HitState)
                result = false;
            if (currentState.State is AttackState)
                result = false;
            return result;
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
    }



    //Abstract class
    public class BaseState : IState
    {
        protected readonly Player player;
        protected readonly StateMachine stateMachine;

        protected BaseState(Player player, StateMachine stateMachine)
        {
            this.player = player;
            this.stateMachine = stateMachine;
        }
        // Debug methods
        public virtual void Update()
        {
            // Debug.Log("Update " + this.GetType().Name);
        }
        public virtual void OnEnter()
        {
             //Debug.Log("OnEnter " + this.GetType().Name);
        }
        public virtual void OnExit()
        {
             //Debug.Log("OnExit " + this.GetType().Name);
        }
        public virtual void FixedUpdate()
        {
            // Debug.Log("FixedUpdate " + this.GetType().Name);
        }

        public virtual void OnAnimationEnd()
        {
            //Debug.Log("OnAnimationEnd " + this.GetType().Name);
        }

        public virtual void ProcessInput(InputType inputType)
        {
            // Debug.Log(inputType + this.GetType().Name);
        }
    }

}