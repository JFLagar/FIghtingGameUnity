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
    public class InputHandler : MonoBehaviour
    {
        public Character character;
        public CharacterAI ai;
        public bool aiControl = false;
        public bool controllerControl = false;
        public PlayerInput playerInput;
        NewControls inputActions;
        [SerializeField]
        public List<CommandInputs> directionInputs = new List<CommandInputs>();
        public KeyCode[] inputs;
        private LightInput lightButton = new LightInput();
        private HeavyInput heavyButton = new HeavyInput();
        private SpecialInput specialButton = new SpecialInput();
        public MovementInput movementInput = new MovementInput();
        // Start is called before the first frame update
        public CommandInputs movement;
        public CommandInputs input;
        public Vector2 direction = Vector2.zero;
        public List<AttackInputs> attackInputs = new List<AttackInputs>();

        // Update is called once per frame
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            if (aiControl)
            {
                ai.Initiate(this);
            }
            movementInput.character = character;
            lightButton.character = character;
            heavyButton.character = character;
            specialButton.character = character;


            inputActions = new NewControls();
            if (!aiControl)
                playerInput.SwitchCurrentControlScheme(playerInput.defaultControlScheme, Keyboard.current);
            //MapActions(true);
        }

        void Update()
        {
            if (attackInputs.Count == 1)
            {
                PerformInput(attackInputs[0]);
            }
            if (aiControl)
            {
                direction = ai.dir;
            }
            if (character.currentAction == StateMachineSpace.ActionStates.Hit || character.currentAction == StateMachineSpace.ActionStates.Block)
            {
                attackInputs.Clear();
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
            movementInput.direction = Vector2.zero;
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
                attackInputs.Clear();
            }
        }

        public void GrabFunction()
        {
            character.PerformAttack(AttackType.Grab);
            attackInputs.Clear();
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

        public void PerformInput(AttackInputs input)
        {
            switch (input)
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
            attackInputs.Clear();
        }
    }
}
