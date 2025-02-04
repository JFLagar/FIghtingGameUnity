using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SkillIssue.StateMachineSpace
{
    public interface IState
    {
        public void EnterState() { }
        public void ExitState() { }
    }
}
