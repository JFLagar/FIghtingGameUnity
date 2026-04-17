using SkillIssue.CharacterSpace;
using UnityEngine;
namespace SkillIssue.Inputs
{
    //Template
    [System.Serializable]
    public class CommandInputs : ICommandInput
    {
        public InputHandler InputHandler {  get; private set; }
        public bool IsPressed { get; private set; }
        public void SetInputHandler(InputHandler inputHandler)
        {
            InputHandler = inputHandler;
        }

        public virtual void InputPressed() {
            IsPressed = true;
        }
        public virtual void InputReleased() {
            IsPressed = false;
        }
    }

    public class LightInput : CommandInputs
    {
        public string name = "Light";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Light, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Light, false);
            base.InputReleased();
        }
    }
    public class MediumInput : CommandInputs
    {
        public string name = "Medium";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Medium, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Medium, false);
            base.InputReleased();
        }
    }
    public class HeavyInput : CommandInputs
    {
        public string name = "Heavy";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Heavy, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Heavy, false);
            base.InputReleased();
        }
    }
    public class UniqueInput : CommandInputs
    {
        public string name = "Unique";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Unique, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Unique, false);
            base.InputReleased();
        }
    }

    public class UpInput : CommandInputs
    {
        public string name = "Up";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Up, true);
            InputHandler.WasYReleased = false;
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Up, false);
            InputHandler.WasYReleased = true;
            base.InputReleased();
        }
    }

    public class DownInput : CommandInputs
    {
        public string name = "Down";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Down, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Down, false);
            base.InputReleased();
        }
    }

    public class LeftInput : CommandInputs
    {
        public string name = "Left";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Left, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Left, false);
            base.InputReleased();
        }
    }

    public class RightInput : CommandInputs
    {
        public string name = "Right";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Right, true);
            base.InputPressed();
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Right, false);
            base.InputReleased();
        }
    }

}

