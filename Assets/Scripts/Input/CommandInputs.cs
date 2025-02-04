using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;
namespace SkillIssue.Inputs
{
    //Template
    [System.Serializable]
    public class CommandInputs : ICommandInput
    {
        public Character character;
        public bool pressed = false;
        private float buttonHeld;
        public void Update()
        {
            if(pressed)
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
        public virtual void InputPressed() { }
        public virtual void InputReleased() { }
        public virtual void InputHold(float time) { }
    }

    public class LightInput : CommandInputs
    {
        public string name = "Light";
        public override void InputPressed() 
        {
            character.inputHandler.attackInputs.Add(AttackInputs.Light);
        }
        public override void InputReleased() 
        {
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
            character.inputHandler.attackInputs.Add(AttackInputs.Heavy);
        }
        public override void InputReleased() 
        {
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
            character.inputHandler.attackInputs.Add(AttackInputs.Special);
        }
        public override void InputReleased() 
        { 
        
        }
        public override void InputHold(float time) 
        {
        
        }
    }
    public class MovementInput : CommandInputs
    {
        public Vector2 direction;
        public override void InputPressed()
        {

        }
        public override void InputReleased() 
        {
            direction = Vector2.zero;
        }
        public override void InputHold(float time)
        {

        }
    }
}

