using SkillIssue.CharacterSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkillIssue.Inputs
{
    public enum InputType
    {
        Light,
        Heavy,
        Special,
        Movement
    }

    public enum MotionInputs
    {
        qcb,
        qcf,
        dd,
        bb,
        ff,
        dpf,
        dpb,
        du,
        NONE
    }

    [Serializable]
    public struct MotionInputStruct
    {
        public MotionInputs Input;
        public Vector2[] motions;
    }


    [Serializable]
    public struct InputStruct
    {
        public InputType InputType { get; private set; }
        public bool IsPressed { get; private set; }
        public float Time { get; private set; }
        public Vector2 Direction { get; private set; }
        //For recording
        public int Frame { get; private set; }
        public InputStruct(InputType input, bool pressed, float time, Vector2 direction, int frame)
        {
            InputType = input;
            IsPressed = pressed;
            Time = time;
            Direction = direction;
            Frame = frame;
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
        MotionInputStruct[] motionInputs;

        [Space]

        [SerializeField]
        Vector2 direction = Vector2.zero;
        [SerializeField]
        Vector2 currentInputDirection = Vector2.zero;
        [SerializeField]
        Queue<InputStruct> inputQueue = new Queue<InputStruct>();
        [SerializeField]
        Queue<InputStruct> motionInputQueue = new Queue<InputStruct>();
        public List<InputStruct> InputQueueList = new List<InputStruct>();
        public List<InputStruct> InputRecordingList = new List<InputStruct>();
        private List<InputStruct> InputReplayingList = new List<InputStruct>();
        [SerializeField]
        private float bufferTime = 0.8f;
        [SerializeField]
        private float motionBufferTime = 0.8f;

        private bool isReplaying = false;
        int replayFrame = 0;

        private GameManager gameManager;

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

        public void Update()
        {
            if (gameManager == null)
            {
                gameManager = Managers.Instance.GameManager;
            }
            if (isReplaying)
            {
                Debug.Log("Replay");
                CheckForReplayedInput();
                replayFrame++;
            }
            // DO the buffer
            CheckForMotionInputs();
            ProcessInputs();
            CleanupMotionBuffer();
        }

        private void CheckForReplayedInput()
        {
            if (InputReplayingList.Count == 0)
            {
                isReplaying = false;
                replayFrame = 0;
                Debug.Log("ReplayEnded");
                playerInput.ActivateInput();
            }
            InputStruct[] recordedInputs = InputReplayingList.FindAll(c => c.Frame == replayFrame).ToArray();
            foreach (var input in recordedInputs)
            {
                if (input.InputType == InputType.Movement)
                {
                    motionInputQueue.Enqueue(new InputStruct(input.InputType, input.IsPressed, Time.time, input.Direction, input.Frame));
                    MovementFunction(input.Direction);
                }
                else
                    inputQueue.Enqueue(new InputStruct(input.InputType, input.IsPressed, Time.time, input.Direction, input.Frame));
                Debug.Log(input.InputType + "Replayed" + replayFrame);
                InputReplayingList.Remove(input);
            }
        }

        public void StartPlayback()
        {
            playerInput.DeactivateInput();
            Debug.Log("ReplayStart");
            InputReplayingList.AddRange(InputRecordingList);
            replayFrame = 0;
            isReplaying = true;
        }

        void CheckForMotionInputs()
        {
            List<InputStruct> currentInputs = new List<InputStruct>();
            foreach( var input in motionInputQueue)
            {
                if (Time.time - input.Time <= bufferTime)
                    currentInputs.Add(input);
            }
            if (currentInputs.Count == 0)
            {
                return;
            }
            foreach (var motion in motionInputs)
            {
                if (IsSequencePartialMatch(currentInputs, motion.motions))
                {
                    character.SetMotionInput(motion.Input);
                    if (gameManager.IsRecording)
                        InputRecordingList.AddRange(currentInputs);
                    motionInputQueue.Clear();
                }
            }

        }

        bool IsSequencePartialMatch(List<InputStruct> inputs, Vector2[] motions)
        {
            InputStruct previousInput = new InputStruct(); 
            int seqIndex = 0;
            for (int i = 0; i < inputs.Count;  i++)
            {
                if (inputs[i].Direction == motions[seqIndex] && CheckForReleasedInput(inputs[i], previousInput))
                    seqIndex++;
                if (seqIndex >= motions.Length)
                    return true;
                previousInput = inputs[i];
            }

            return false;
        }

        bool CheckForReleasedInput(InputStruct input, InputStruct previousInput)
        {
            if (input.Direction != previousInput.Direction) return true;
            if (input.IsPressed != previousInput.IsPressed) return false;
            return true;
        }

        void ProcessInputs()
        {
            // Before this check for any double pressed input
            while (inputQueue.Count > 0)
            {
                var bufferedInput = inputQueue.Peek();
                                    if (gameManager.IsRecording)
                        InputRecordingList.Add(bufferedInput);
                if (Time.time - bufferedInput.Time <= bufferTime)
                {
                    PerformInput(bufferedInput);
                    inputQueue.Dequeue();
                }
                else
                {
                    inputQueue.Dequeue();
                }
            }
        }

        void CleanupMotionBuffer()
        {
            if (motionInputQueue.Count > 0 && Time.time - motionInputQueue.Peek().Time > motionBufferTime)
            {
                if (gameManager.IsRecording)
                    InputRecordingList.AddRange(motionInputQueue);
                motionInputQueue.Clear(); // Clear old motions
                character.SetMotionInput(MotionInputs.NONE);
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
            inputActions.Controls.Enable();
            inputActions.Controls.LightButton.performed += LightButton;
            inputActions.Controls.HeavyButton.performed += HeavyButton;
            inputActions.Controls.SpecialButton.performed += SpecialButton;
            inputActions.Controls.Start.performed += StartButton;
            inputActions.Controls.Select.performed += SelectButton;
            inputActions.Controls.MovementX.performed += MovementXDown;
            inputActions.Controls.MovementY.performed += MovementYDown;
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
            //direction.x = 0;
            //inputQueue.Enqueue(new InputStruct(InputType.Movement, false, Time.time, direction));
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
            if (context.started)
            motionInputQueue.Enqueue(new InputStruct(InputType.Movement, true, Time.time, direction, gameManager.RecordingFrame));
            if (context.canceled)
                motionInputQueue.Enqueue(new InputStruct(InputType.Movement, false, Time.time, direction, gameManager.RecordingFrame));
        }

        public void MovementYUp(InputAction.CallbackContext context)
        {
            //direction.y = 0;
            //inputQueue.Enqueue(new InputStruct(InputType.Movement, false, Time.time, direction));
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
            if (context.started)
                motionInputQueue.Enqueue(new InputStruct(InputType.Movement, true, Time.time, direction, gameManager.RecordingFrame));
            if (context.canceled)
                motionInputQueue.Enqueue(new InputStruct(InputType.Movement, false, Time.time, direction, gameManager.RecordingFrame));
        }

        public void MovementFunction(Vector2 direction)
        {
            this.direction = direction;
        }

        public void GrabButton(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                lightButton.InputPressed();
                heavyButton.InputPressed();
            }
            if (context.canceled)
            {
                lightButton.InputReleased();
                heavyButton.InputReleased();
            }

        }

        public void GrabFunction()
        {
            character.PerformAttack(AttackType.Grab);
        }

        public void LightButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                lightButton.InputPressed();
            if (context.canceled)
                lightButton.InputReleased();
        }

        public void LightFunction(bool IsPressed = true)
        {
            if (IsPressed)
            lightButton.InputPressed();
            else
                lightButton.InputReleased();
        }

        public void HeavyButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                heavyButton.InputPressed();
            if (context.canceled)
                heavyButton.InputReleased();
        }

        public void HeavyFunction(bool isPressed = true)
        {
            if (isPressed)
            heavyButton.InputPressed();
            else
                heavyButton.InputReleased();
        }

        public void SpecialButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                specialButton.InputPressed();
            if (context.canceled)
                specialButton.InputReleased();
        }

        public void SpecialFunction(bool isPressed = true)
        {
            if (isPressed)
            specialButton.InputPressed();
            else
                specialButton.InputReleased();
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

        public void AddAttackInput(InputType input, bool isPressed)
        {
            inputQueue.Enqueue(new InputStruct(input, isPressed, Time.time, Vector2.zero, gameManager.RecordingFrame));
            if (Managers.Instance.GameManager.IsRecording)
            {
                Debug.Log(input + "" + gameManager.RecordingFrame); 
            }
        }

        public void PerformInput(InputStruct input)
        {
            if (!input.IsPressed)
            {
                return;
            }

            switch (input.InputType)
            {
                case InputType.Light:
                    character.PerformAttack(AttackType.Light);
                    break;
                case InputType.Heavy:
                    character.PerformAttack(AttackType.Heavy);
                    break;
                case InputType.Special:
                    character.PerformAttack(AttackType.Special);
                    break;
                case InputType.Movement:
                    break;
            }

            character.SetMotionInput(MotionInputs.NONE);
        }

    }
}
