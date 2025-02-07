using SkillIssue.CharacterSpace;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkillIssue.Inputs
{
    public enum AttackInputs
    {
        Light,
        Heavy,
        Special
    }

    public struct AttackInputStruct
    {
        public AttackInputs InputType { get; private set; }
        public bool IsPressed { get; private set; }
        public AttackInputStruct(AttackInputs input, bool pressed)
        {
            InputType = input;
            IsPressed = pressed;
        }
    }

    public class InputHandler : MonoBehaviour
    {
        Character character;
        CharacterAI ai;
        bool aiControl = false;
        bool controllerControl = false;
        PlayerInput playerInput;
        NewControls inputActions;
        private LightInput lightButton = new LightInput();
        private HeavyInput heavyButton = new HeavyInput();
        private SpecialInput specialButton = new SpecialInput();
        MovementInput movementInput = new MovementInput();

        [Space]

        [SerializeField]
        Vector2 direction = Vector2.zero;
        [SerializeField]
        List<AttackInputStruct> attackInputsList = new();
        [SerializeField]
        private List<CommandInputs> directionInputs = new();

        // Update is called once per frame
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        public PlayerInput GetPlayerInput()
        {
            return playerInput;
        }

        public void Initialize(Character controllingCharacter)
        {
            character = controllingCharacter;

            if (aiControl)
            {
                ai.Initiate(this);
            }
            movementInput.SetInputHandler(this);
            lightButton.SetInputHandler(this);
            heavyButton.SetInputHandler(this);
            specialButton.SetInputHandler(this);


            inputActions = new NewControls();
            if (!aiControl)
                playerInput.SwitchCurrentControlScheme(playerInput.defaultControlScheme, Keyboard.current);
            //MapActions(true);
        }

        public Vector2 GetDirection()
        {
            return direction;
        }

        public Character GetCharacter()
        {
            return character;
        }

        void Update()
        {
            if (attackInputsList.Count == 1)
            {
                PerformInput(attackInputsList[0]);
            }
            if (aiControl)
            {
                direction = ai.dir;
            }
            if (character.GetCurrentActionState() == StateMachineSpace.ActionStates.Hit || character.GetCurrentActionState() == StateMachineSpace.ActionStates.Block)
            {
                attackInputsList.Clear();
            }
        }

        public void ResetAI()
        {
            if (!aiControl)
            {
                aiControl = true;
                ai.Initiate(this);
            }
            else
            {
                aiControl = false;
                ai.AiReset();
            }
        }

        public void MapActions(bool player)
        {
            {
                inputActions.Controls.Enable();
                inputActions.Controls.LightButton.performed += LightButton;
                inputActions.Controls.HeavyButton.performed += HeavyButton;
                inputActions.Controls.SpecialButton.performed += SpecialButton;
                inputActions.Controls.Start.performed += StartButton;
                inputActions.Controls.Select.performed += SelectButton;
                inputActions.Controls.MovementX.performed += MovementXDown;
                inputActions.Controls.MovementX.canceled += MovementXUp;
                inputActions.Controls.MovementY.performed += MovementYDown;
                inputActions.Controls.MovementY.canceled += MovementYUp;
            }

        }

        private void NavigateUI(InputAction.CallbackContext obj)
        {
            throw new NotImplementedException();
        }

        private void Cancel(InputAction.CallbackContext obj)
        {
            throw new NotImplementedException();
        }

        private void Confirm(InputAction.CallbackContext obj)
        {
            throw new NotImplementedException();
        }

        public void MovementXUp(InputAction.CallbackContext context)
        {
            direction.x = 0;
        }

        public void MovementXDown(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            switch (value)
            {
                case 0:
                    direction.x = 0;
                    break;
                case < 0:
                    direction.x = -1;
                    break;
                case > 0:
                    direction.x = 1;
                    break;
            }
        }

        public void MovementYUp(InputAction.CallbackContext context)
        {
            direction.y = 0;
        }

        public void MovementYDown(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            switch (value)
            {
                case 0:
                    direction.y = 0;
                    break;
                case < 0:
                    direction.y = -1;
                    break;
                case > 0:
                    direction.y = 1;
                    break;
            }
        }

        public void GrabButton(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                character.PerformAttack(AttackType.Grab);
                attackInputsList.Clear();
            }
        }

        public void GrabFunction()
        {
            character.PerformAttack(AttackType.Grab);
            attackInputsList.Clear();
        }

        public void LightButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                lightButton.InputPressed();
        }

        public void LightFunction()
        {
            lightButton.InputPressed();
        }

        public void HeavyButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                heavyButton.InputPressed();
        }

        public void HeavyFunction()
        {
            heavyButton.InputPressed();
        }

        public void SpecialButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                specialButton.InputPressed();
        }

        public void SpecialFunction()
        {
            specialButton.InputPressed();
        }

        public void MovementUp(InputAction.CallbackContext context)
        {
            direction = Vector2.zero;
        }

        public void StartButton(InputAction.CallbackContext context)
        {
            Managers.Instance.GameManager.PauseGame();
        }

        public void SelectButton(InputAction.CallbackContext context)
        {
            if (Managers.Instance.GameManager.IsTraining())
                Managers.Instance.GameManager.ResetPosition();
        }

        public void AddAttackInput(AttackInputs input, bool isPressed)
        {
            attackInputsList.Add(new AttackInputStruct(input, isPressed));
        }

        public void PerformInput(AttackInputStruct input)
        {
            if (!input.IsPressed)
            {
                attackInputsList.Remove(input);
                return;
            }

            switch (input.InputType)
            {
                case AttackInputs.Light:
                    character.PerformAttack(AttackType.Light);
                    break;
                case AttackInputs.Heavy:
                    character.PerformAttack(AttackType.Heavy);
                    break;
                case AttackInputs.Special:
                    character.PerformAttack(AttackType.Special);
                    break;
            }
            attackInputsList.Clear();
        }

    }
}
