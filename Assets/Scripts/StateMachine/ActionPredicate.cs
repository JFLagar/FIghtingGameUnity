using System;

namespace SkillIssue.StateMachineSpace
{
    public class ActionPredicate : IPredicate
    {
        public bool Flag { get; set; }
        //Need to implement a event based predicated 
        public ActionPredicate(ref Action eventAction) => eventAction = () => { Flag = true; };
        public bool Evaluate ()
        {
            bool result = Flag;
            Flag = false;
            return result;
        }

        public void SetFlag()
        {
            Flag = true;
        }
    }

    public class ActionWithFuncPredicate : IPredicate
    {
        public bool Flag;
        private readonly Func<bool> _func;

        public ActionWithFuncPredicate(ref Action eventAction, Func<bool> func)
        {
            _func = func;
            eventAction += () => { Flag = true; };
        }

        public bool Evaluate()
        {
            bool result = Flag;
            Flag = false;
            return result && _func.Invoke();
        }
    }
}

