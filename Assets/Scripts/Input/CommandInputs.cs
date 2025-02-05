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
        public void Update()
        {
            if (pressed)
            {
                buttonHeld += (0.1f * Time.deltaTime);
            }
            if (buttonHeld >= 0.1)
            {
                InputHold(buttonHeld);
            }

        }
        public void Activate(bool isUp)
        {
            if (pressed == false)
            {
                InputPressed();
                pressed = true;
            }
            if (isUp == true)
            {
                buttonHeld = 0;
                pressed = false;
                InputReleased();
            }

        }
        public void SetInputHandler(InputHandler inputHandler)
        {
            InputHandler = inputHandler;
        }

        public virtual void InputPressed() { }
        public virtual void InputReleased() { }
        public virtual void InputHold(float time) { }
    }

    public class LightInput : CommandInputs
    {
        public string name = "Light";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(AttackInputs.Light, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(AttackInputs.Light, false);
        }
        public override void InputHold(float time)
        {
        }
    }
    public class HeavyInput : CommandInputs
    {
        public string name = "Heavy";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(AttackInputs.Heavy, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(AttackInputs.Heavy, false);
        }
        public override void InputHold(float time)
        {
        }
    }
    public class SpecialInput : CommandInputs
    {
        public string name = "Special";
        public override void InputPressed()
        {
            InputHandler.AddAttackInput(AttackInputs.Special, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddAttackInput(AttackInputs.Special, false);
        }
        public override void InputHold(float time)
        {

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
        public override void InputHold(float time)
        {

        }
    }
}

