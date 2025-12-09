using SkillIssue.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SkillIssue.StateMachineSpace
{
    public interface IState
    {
        public void OnEnter() { }
        public void Update() { }
        public void FixedUpdate() { }
        public void OnExit() { }
        public void OnAnimationEnd() { }
        public void ProcessInput(InputType inputType) { }
    }
    
}
