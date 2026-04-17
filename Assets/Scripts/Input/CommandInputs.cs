using SkillIssue.CharacterSpace;
using UnityEngine;
namespace SkillIssue.Inputs
{
    //Template
    [System.Serializable]
    public class CommandInputs : ICommandInput
    {
        public InputHandler InputHandler {  get; private set; }
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
            InputHandler.AddInput(InputType.Light, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Light, false);
        }
    }
    public class MediumInput : CommandInputs
    {
        public string name = "Medium";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Medium, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Medium, false);
        }
    }
    public class HeavyInput : CommandInputs
    {
        public string name = "Heavy";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Heavy, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Heavy, false);
        }
    }
    public class UniqueInput : CommandInputs
    {
        public string name = "Unique";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Unique, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Unique, false);
        }
    }

    public class UpInput : CommandInputs
    {
        public string name = "Up";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Up, true);
            InputHandler.WasYReleased = false;
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Up, false);
            InputHandler.WasYReleased = true;
        }
    }

    public class DownInput : CommandInputs
    {
        public string name = "Down";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Down, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Down, false);
        }
    }

    public class LeftInput : CommandInputs
    {
        public string name = "Left";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Left, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Left, false);
        }
    }

    public class RightInput : CommandInputs
    {
        public string name = "Right";
        public override void InputPressed()
        {
            InputHandler.AddInput(InputType.Right, true);
        }
        public override void InputReleased()
        {
            InputHandler.AddInput(InputType.Right, false);
        }
    }

}

