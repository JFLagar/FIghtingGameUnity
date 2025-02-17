using SkillIssue.CharacterSpace;
using UnityEngine;
namespace SkillIssue.Inputs
{
    //Template
    [System.Serializable]
    public class CommandInputs : ICommandInput
    {
        public InputHandler InputHandler {  get; private set; }
        bool pressed = false;
        private float buttonHeld;
        public void SetInputHandler(InputHandler inputHandler)
        {
            InputHandler = inputHandler;
        }

        public virtual void InputPressed() { }
        public virtual void InputReleased() { }
    }

    public class LightInput : CommandInputs
    {
        public string name = "Light";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(InputType.Light, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(InputType.Light, false);
        }
    }
    public class MediumInput : CommandInputs
    {
        public string name = "Medium";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(InputType.Medium, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(InputType.Medium, false);
        }
    }
    public class HeavyInput : CommandInputs
    {
        public string name = "Heavy";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(InputType.Heavy, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(InputType.Heavy, false);
        }
    }
    public class UniqueInput : CommandInputs
    {
        public string name = "Unique";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(InputType.Unique, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(InputType.Unique, false);
        }
    }
    public class MovementInput : CommandInputs
    {
        public override void InputPressed()
        {

        }
        public override void InputReleased()
        {

        }
    }
}

