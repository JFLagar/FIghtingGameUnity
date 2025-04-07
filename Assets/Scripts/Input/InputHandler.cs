using SkillIssue.CharacterSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace SkillIssue.Inputs
{
    public enum InputType
    {
        Light,
        Medium,
        Heavy,
        Unique,
        Movement,
        LU,
        MH,
        LMH,
        LMHU
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
    public class BufferedInput
    {
        public InputType InputType { get; private set; }
        public bool IsPressed { get; private set; }
        public float Time { get; private set; }
        public Vector2 Direction { get; private set; }
        //For recording
        public int Frame { get; private set; }
        public BufferedInput(InputType input, bool pressed, float time, Vector2 direction, int frame)
        {
            InputType = input;
            IsPressed = pressed;
            Time = time;
            Direction = direction;
            Frame = frame;
        }

        public void SetInputType(InputType input)
        { InputType = input; }
    }


    public class InputHandler : MonoBehaviour, InputActions.IControlsActions
    {
        Character character;
        CharacterAI ai;
        bool aiControl = false;
        bool controllerControl = false;
        PlayerInput playerInput;
        [SerializeField]
        InputActions inputActions;
        private LightInput lightButton = new LightInput();
        private MediumInput mediumButton = new MediumInput();
        private HeavyInput heavyButton = new HeavyInput();
        private UniqueInput uniqueButton = new UniqueInput();
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
        Queue<BufferedInput> inputQueue = new Queue<BufferedInput>();
        [SerializeField]
        Queue<BufferedInput> motionInputQueue = new Queue<BufferedInput>();
        public List<BufferedInput> InputQueueList = new List<BufferedInput>();
        public List<BufferedInput> InputRecordingList = new List<BufferedInput>();
        private List<BufferedInput> InputReplayingList = new List<BufferedInput>();
        [SerializeField]
        private float bufferTime = 0.8f;
        [SerializeField]
        private float motionBufferTime = 0.8f;
        [SerializeField]
        private float simultaneousThreshold = 0.1f;

        private bool isReplaying = false;
        int replayFrame = 0;

        private GameManager gameManager;

        public PlayerInput GetPlayerInput()
        {
            return playerInput;
        }

        public void Initialize(Character controllingCharacter)
        {
            character = controllingCharacter;

            movementInput.SetInputHandler(this);
            lightButton.SetInputHandler(this);
            mediumButton.SetInputHandler(this);
            heavyButton.SetInputHandler(this);
            uniqueButton.SetInputHandler(this);

            playerInput = transform.parent.GetComponent<PlayerInput>();

            //if (!aiControl)
            //    playerInput.SwitchCurrentControlScheme(playerInput.defaultControlScheme, Keyboard.current);

            MapActions(true);
        }

        void MapActions(bool player)
        {
            //Remove when local player support is added
            if (character.isPlayer2)
                return;
            inputActions = new InputActions();

            inputActions.Controls.Enable();
            inputActions.Controls.LightButton.performed += LightButton;
            inputActions.Controls.MediumButton.performed += MediumButton;
            inputActions.Controls.HeavyButton.performed += HeavyButton;
            inputActions.Controls.UniqueButton.performed += UniqueButton;
            inputActions.Controls.Start.performed += StartButton;
            inputActions.Controls.Select.performed += SelectButton;
            inputActions.Controls.MovementX.performed += MovementXDown;
            inputActions.Controls.MovementY.performed += MovementYDown;
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            //Debug.Log(context.action.name + playerInput.name);
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
            InputQueueList = inputQueue.ToList();
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
            if (!CheckForSimultaneousInputs())
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
            BufferedInput[] recordedInputs = InputReplayingList.FindAll(c => c.Frame == replayFrame).ToArray();
            foreach (var input in recordedInputs)
            {
                if (input.InputType == InputType.Movement)
                {
                    motionInputQueue.Enqueue(new BufferedInput(input.InputType, input.IsPressed, Time.time, input.Direction, input.Frame));
                    MovementFunction(input.Direction);
                }
                else
                    inputQueue.Enqueue(new BufferedInput(input.InputType, input.IsPressed, Time.time, input.Direction, input.Frame));
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
            List<BufferedInput> currentInputs = new List<BufferedInput>();
            foreach (var input in motionInputQueue)
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

        bool IsSequencePartialMatch(List<BufferedInput> inputs, Vector2[] motions)
        {
            BufferedInput previousInput = null;
            int seqIndex = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                if (inputs[i].Direction == motions[seqIndex] && CheckForReleasedInput(inputs[i], previousInput))
                    seqIndex++;
                if (seqIndex >= motions.Length)
                    return true;
                previousInput = inputs[i];
            }

            return false;
        }

        bool CheckForReleasedInput(BufferedInput input, BufferedInput previousInput)
        {
            if (previousInput == null) return true;
            if (input.Direction != previousInput.Direction) return true;
            if (input.IsPressed != previousInput.IsPressed) return false;
            return true;
        }

        bool CheckForSimultaneousInputs()
        {
            if (inputQueue.Count == 0)
                return false;
            BufferedInput lightInput = null;
            BufferedInput mediumInput = null;
            BufferedInput heavyInput = null;
            BufferedInput uniqueInput = null;

            BufferedInput simultPressInput = null;
            foreach (var input in inputQueue)
            {
                switch (input.InputType)
                {
                    case InputType.Light:
                        if (input.IsPressed)
                            lightInput = input;
                        break;
                    case InputType.Medium:
                        if (input.IsPressed)
                            mediumInput = input;
                        break;
                    case InputType.Heavy:
                        if (input.IsPressed)
                            heavyInput = input;
                        break;
                    case InputType.Unique:
                        if (input.IsPressed)
                            uniqueInput = input;
                        break;
                }
            }

            if (mediumInput != null && heavyInput != null && (Mathf.Abs(mediumInput.Time - heavyInput.Time) <= simultaneousThreshold))
            {
                simultPressInput = new BufferedInput(InputType.MH, true, Time.time, direction, heavyInput.Frame);
                if (lightButton != null && (Mathf.Abs(simultPressInput.Time - lightInput.Time) <= simultaneousThreshold))
                    simultPressInput.SetInputType(InputType.LMH);
                if (simultPressInput.InputType == InputType.LMH && uniqueInput != null && (Mathf.Abs(simultPressInput.Time - uniqueInput.Time) <= simultaneousThreshold))
                    simultPressInput.SetInputType(InputType.LMHU);
            }
            if (lightInput != null && uniqueInput != null && (Mathf.Abs(lightInput.Time - uniqueInput.Time) <= simultaneousThreshold))
                simultPressInput = new BufferedInput(InputType.LU, true, Time.time, direction, uniqueInput.Frame);
            if (simultPressInput != null)
            {
                if (gameManager.IsRecording)
                    InputRecordingList.AddRange(inputQueue);
                PerformInput(simultPressInput);
                inputQueue.Clear();
                return true;
            }

            return false;
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
            motionInputQueue.Enqueue(new BufferedInput(InputType.Movement, !context.canceled, Time.time, direction, gameManager.RecordingFrame));
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
            motionInputQueue.Enqueue(new BufferedInput(InputType.Movement, !context.canceled, Time.time, direction, gameManager.RecordingFrame));
        }

        public void MovementFunction(Vector2 direction)
        {
            this.direction = direction;
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

        public void MediumButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                mediumButton.InputPressed();
            if (context.canceled)
                mediumButton.InputReleased();
        }

        public void MediumFunction(bool IsPressed = true)
        {
            if (IsPressed)
                mediumButton.InputPressed();
            else
                mediumButton.InputReleased();
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

        public void UniqueButton(InputAction.CallbackContext context)
        {
            if (context.performed)
                uniqueButton.InputPressed();
            if (context.canceled)
                uniqueButton.InputReleased();
        }

        public void UniqueFunction(bool isPressed = true)
        {
            if (isPressed)
                uniqueButton.InputPressed();
            else
                uniqueButton.InputReleased();
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
            inputQueue.Enqueue(new BufferedInput(input, isPressed, Time.time, Vector2.zero, gameManager.RecordingFrame));
        }

        public void PerformInput(BufferedInput input)
        {
            if (!input.IsPressed)
            {
                return;
            }
            if (input.InputType != InputType.Movement)
            {
                character.PerformInput(input.InputType);
            }
      
            character.SetMotionInput(MotionInputs.NONE);
        }

        private void OnDestroy()
        {
            playerInput.onActionTriggered -= OnActionTriggered;
            if (character.isPlayer2)
                return;
            inputActions.Controls.LightButton.performed -= LightButton;
            inputActions.Controls.MediumButton.performed -= MediumButton;
            inputActions.Controls.HeavyButton.performed -= HeavyButton;
            inputActions.Controls.UniqueButton.performed -= UniqueButton;
            inputActions.Controls.Start.performed -= StartButton;
            inputActions.Controls.Select.performed -= SelectButton;
            inputActions.Controls.MovementX.performed -= MovementXDown;
            inputActions.Controls.MovementY.performed -= MovementYDown;
            inputActions.Controls.Disable();
        }

        public void OnLightButton(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnMediumButton(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnHeavyButton(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnUniqueButton(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnStart(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnMovementX(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnMovementY(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}
