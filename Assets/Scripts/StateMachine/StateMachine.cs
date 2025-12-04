using SkillIssue.CharacterSpace;
using System;
using System.Collections.Generic;
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
    public class StateMachine
    {
        StateNode currentState;
        StateNode previousState;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();
        public ActionStates CurrentAction { get; private set; }
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

            previousState = currentState;
            var nextState = nodes[state.GetType()].State;

            previousState.State?.OnExit();
            nextState?.OnEnter();
            currentState = nodes[state.GetType()];
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

        public void OnAnimationEnd()
        {
            currentState.State?.OnAnimationEnd();
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
        public virtual void Update()
        {
            Debug.Log("Update " + this.GetType().Name);
        }
        public virtual void OnEnter()
        {
            Debug.Log("OnEnter " + this.GetType().Name);
        }
        public virtual void OnExit()
        {
            Debug.Log("OnExit " + this.GetType().Name);
        }
        public virtual void FixedUpdate()
        {
            Debug.Log("FixedUpdate " + this.GetType().Name);
        }

        public virtual void OnAnimationEnd()
        {
            Debug.Log("OnAnimationEnd " + this.GetType().Name);
        }
    }

}