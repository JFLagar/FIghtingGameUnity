using SkillIssue.CharacterSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
        LU,
        MH,
        LMH,
        LMHU,
        Up,
        Down,
        Left,
        Right,
        NONE
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
        //For recording
        public int Frame { get; private set; }
        public BufferedInput(InputType input, bool pressed, float time, int frame)
        {
            InputType = input;
            IsPressed = pressed;
            Time = time;
            Frame = frame;
        }

        public void SetInputType(InputType input)
        { InputType = input; }

        public bool IsMotion()
        {
            return InputType == InputType.Up || InputType == InputType.Down || InputType == InputType.Left || InputType == InputType.Right;
        }
    }


    public class InputHandler
    {
        Player player;
        public PlayerInput PlayerInput { get; private set; }
        [SerializeField]
        InputActions inputActions;
        private LightInput lightButton = new LightInput();
        private MediumInput mediumButton = new MediumInput();
        private HeavyInput heavyButton = new HeavyInput();
        private UniqueInput uniqueButton = new UniqueInput();

        private UpInput upButton = new UpInput();
        private DownInput downButton = new DownInput();
        private LeftInput leftButton = new LeftInput();
        private RightInput rightButton = new RightInput();


        public bool WasYReleased { get; set; }

        [Space]

        [SerializeField]
        MotionInputStruct[] motionInputs;

        [Space]

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
        private float simultaneousThreshold = 0.3f;

        private bool isReplaying = false;
        int replayFrame = 0;

        private GameManager gameManager => Managers.Instance.GameManager;

        private Vector2 inputDirection = new Vector2();

        public void Initialize(Player controllingPlayer)
        {
            player = controllingPlayer;

            lightButton.SetInputHandler(this);
            mediumButton.SetInputHandler(this);
            heavyButton.SetInputHandler(this);
            uniqueButton.SetInputHandler(this);

            upButton.SetInputHandler(this);
            downButton.SetInputHandler(this);
            leftButton.SetInputHandler(this);
            rightButton.SetInputHandler(this);

            PlayerInput = player.transform.GetComponent<PlayerInput>();
            motionInputs = gameManager.GetCombatValues().GetMotionInputs();
            MapActions(true);
        }

        void MapActions(bool player)
        {
            inputActions = new InputActions();
            inputActions.Disable();
            inputActions.bindingMask = new InputBinding()
            {
                groups = PlayerInput.defaultControlScheme
            };
            inputActions.Enable();
            inputActions.Controls.Enable();
            inputActions.Controls.LightButton.performed += LightButton;
            inputActions.Controls.LightButton.canceled += LightButton;
            inputActions.Controls.MediumButton.performed += MediumButton;
            inputActions.Controls.MediumButton.canceled += MediumButton;
            inputActions.Controls.HeavyButton.performed += HeavyButton;
            inputActions.Controls.HeavyButton.canceled += HeavyButton;
            inputActions.Controls.UniqueButton.performed += UniqueButton;
            inputActions.Controls.UniqueButton.canceled += UniqueButton;

            inputActions.Controls.LU.performed += LUButton;
            inputActions.Controls.LU.canceled += LUButton;
            inputActions.Controls.LM.performed += LMButton;
            inputActions.Controls.LM.canceled += LMButton;
            inputActions.Controls.MH.performed += MHButton;
            inputActions.Controls.MH.canceled += MHButton;
            inputActions.Controls.LMH.performed += LMHButton;
            inputActions.Controls.LMH.canceled += LMHButton;
            inputActions.Controls.LMHU.performed += LMHUButton;
            inputActions.Controls.LMHU.canceled += LMHUButton;

            inputActions.Controls.Start.performed += StartButton;
            inputActions.Controls.Select.performed += SelectButton;

            inputActions.Controls.Up.performed += UpButton;
            inputActions.Controls.Up.canceled += UpButton;
            inputActions.Controls.Down.performed += DownButton;
            inputActions.Controls.Down.canceled += DownButton;

            inputActions.Controls.Left.performed += LeftButton;
            inputActions.Controls.Left.canceled += LeftButton;
            inputActions.Controls.Right.performed += RightButton;
            inputActions.Controls.Right.canceled += RightButton;

            inputActions.Menu.Enable();
            inputActions.Menu.UIConfirm.performed += UIConfirm;
            inputActions.Menu.UICancel.performed += UICancel;
        }

        public void UnmapActions()
        {
            inputActions.Controls.LightButton.performed -= LightButton;
            inputActions.Controls.LightButton.canceled -= LightButton;
            inputActions.Controls.MediumButton.performed -= MediumButton;
            inputActions.Controls.MediumButton.canceled -= MediumButton;
            inputActions.Controls.HeavyButton.performed -= HeavyButton;
            inputActions.Controls.HeavyButton.canceled -= HeavyButton;
            inputActions.Controls.UniqueButton.performed -= UniqueButton;
            inputActions.Controls.UniqueButton.canceled -= UniqueButton;

            inputActions.Controls.LU.performed -= LUButton;
            inputActions.Controls.LU.canceled -= LUButton;
            inputActions.Controls.LM.performed -= LMButton;
            inputActions.Controls.LM.canceled -= LMButton;
            inputActions.Controls.MH.performed -= MHButton;
            inputActions.Controls.MH.canceled -= MHButton;
            inputActions.Controls.LMH.performed -= LMHButton;
            inputActions.Controls.LMH.canceled -= LMHButton;
            inputActions.Controls.LMHU.performed -= LMHUButton;
            inputActions.Controls.LMHU.canceled -= LMHUButton;


            inputActions.Controls.Start.performed -= StartButton;
            inputActions.Controls.Select.performed -= SelectButton;

            inputActions.Controls.Up.performed -= UpButton;
            inputActions.Controls.Up.canceled -= UpButton;
            inputActions.Controls.Down.performed -= DownButton;
            inputActions.Controls.Down.canceled -= DownButton;

            inputActions.Controls.Left.performed -= LeftButton;
            inputActions.Controls.Left.canceled -= LeftButton;
            inputActions.Controls.Right.performed -= RightButton;
            inputActions.Controls.Right.canceled -= RightButton;

            inputActions.Menu.UIConfirm.performed -= UIConfirm;
            inputActions.Menu.UICancel.performed -= UICancel;

            inputActions.Controls.Disable();
        }

        public void EnableInput()
        {
            inputActions.Enable();
        }

        public void DisableInput()
        {
            inputActions.Disable();
        }

        public Vector2 GetInputDirection()
        {
            return inputDirection;
        }

        //DEBUG
        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            Debug.Log(context.action.name + PlayerInput.name);
        }

        public void Update()
        {
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
                EnableInput();
            }
            BufferedInput[] recordedInputs = InputReplayingList.FindAll(c => c.Frame == replayFrame).ToArray();
            foreach (var input in recordedInputs)
            {
                inputQueue.Enqueue(new BufferedInput(input.InputType, input.IsPressed, Time.time, input.Frame));
                InputReplayingList.Remove(input);
            }
        }

        public void StartPlayback()
        {
            DisableInput();
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
                    player.SetMotionInput(motion.Input);
                    return;
                }
            }
        }

        bool IsSequencePartialMatch(List<BufferedInput> inputs, Vector2[] motions)
       {
            BufferedInput previousInput = null;
            int seqIndex = 0;
            for (int i = 0; i < inputs.Count; i++)
            {
                // Adjust the input direction based on facing direction (only flip X-axis)
                Vector2 adjustedInputDirection = new Vector2(
                    GetInputDirection().x * player.FaceDir,
                    GetInputDirection().y
                );

                if (adjustedInputDirection == motions[seqIndex] && CheckForReleasedInput(inputs[i], previousInput))
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
            if (CalculateInputDirection(input) != CalculateInputDirection(previousInput)) return true;
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

            if (lightInput != null && uniqueInput != null && (Mathf.Abs(lightInput.Time - uniqueInput.Time) <= simultaneousThreshold))
                simultPressInput = new BufferedInput(InputType.LU, true, Time.time, uniqueInput.Frame);

            if (mediumInput != null && heavyInput != null && (Mathf.Abs(mediumInput.Time - heavyInput.Time) <= simultaneousThreshold))
            {
                simultPressInput = new BufferedInput(InputType.MH, true, Time.time, heavyInput.Frame);
                if (lightInput != null && (Mathf.Abs(simultPressInput.Time - lightInput.Time) <= simultaneousThreshold))
                    simultPressInput.SetInputType(InputType.LMH);
                if (simultPressInput.InputType == InputType.LMH && uniqueInput != null && (Mathf.Abs(simultPressInput.Time - uniqueInput.Time) <= simultaneousThreshold))
                    simultPressInput.SetInputType(InputType.LMHU);
            }

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
                player.SetMotionInput(MotionInputs.NONE);
            }
        }

        private void NavigateUI(InputAction.CallbackContext obj)
        {
            throw new NotImplementedException();
        }

        private void UICancel(InputAction.CallbackContext obj)
        {

        }

        private void UIConfirm(InputAction.CallbackContext obj)
        {

        }

        public void LightButton(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
                lightButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
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
            if (context.action.WasPressedThisFrame())
                mediumButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
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
            if (context.action.WasPressedThisFrame())
                heavyButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
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
            if (context.action.WasPressedThisFrame())
                uniqueButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
                uniqueButton.InputReleased();
        }

        public void UniqueFunction(bool isPressed = true)
        {
            if (isPressed)
                uniqueButton.InputPressed();
            else
                uniqueButton.InputReleased();
        }

        public void LUButton(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
            {
                lightButton.InputPressed();
                uniqueButton.InputPressed();
            }
            if (context.action.WasReleasedThisFrame())
            {
                lightButton.InputReleased();
                uniqueButton.InputReleased();
            }
        }

        public void LUFunction(bool isPressed)
        {
            if (isPressed)
            {
                lightButton.InputPressed();
                uniqueButton.InputPressed();
            }
            else
            {
                lightButton.InputReleased();
                uniqueButton.InputReleased();
            }
        }

        public void LMButton(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
            {
                lightButton.InputPressed();
                mediumButton.InputPressed();
            }
            if (context.action.WasReleasedThisFrame())
            {
                lightButton.InputReleased();
                mediumButton.InputReleased();
            }
        }

        public void LMFunction(bool isPressed)
        {
            if (isPressed)
            {
                lightButton.InputPressed();
                mediumButton.InputPressed();
            }
            else
            {
                lightButton.InputReleased();
                mediumButton.InputReleased();
            }
        }

        public void MHButton(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
            {
                mediumButton.InputPressed();
                heavyButton.InputPressed();
            }
            if (context.action.WasReleasedThisFrame())
            {
                mediumButton.InputReleased();
                heavyButton.InputReleased();
            }
        }

        public void MHFunction(bool isPressed)
        {
            if (isPressed)
            {
                mediumButton.InputPressed();
                heavyButton.InputPressed();
            }
            else
            {
                mediumButton.InputReleased();
                heavyButton.InputReleased();
            }
        }

        public void LMHButton(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
            {
                lightButton.InputPressed();
                mediumButton.InputPressed();
                heavyButton.InputPressed();
            }
            if (context.action.WasReleasedThisFrame())
            {
                lightButton.InputReleased();
                mediumButton.InputReleased();
                heavyButton.InputReleased() ;
            }
        }

        public void LMHFunction(bool isPressed)
        {
            if (isPressed)
            {
                lightButton.InputPressed();
                mediumButton.InputPressed();
                heavyButton.InputPressed();
            }
            else
            {
                lightButton.InputReleased();
                mediumButton.InputReleased();
                heavyButton.InputReleased();
            }
        }

        public void LMHUButton(InputAction.CallbackContext context)
        {
            if (context.action.WasPressedThisFrame())
            {
                lightButton.InputPressed();
                mediumButton.InputPressed();
                heavyButton.InputPressed();
                uniqueButton.InputPressed();
            }
            if (context.action.WasReleasedThisFrame())
            {
                lightButton.InputReleased();
                mediumButton.InputReleased();
                heavyButton.InputReleased();
                uniqueButton.InputReleased();
            }
        }

        public void LMHUFunction(bool isPressed)
        {
            if (isPressed)
            {
                lightButton.InputPressed();
                mediumButton.InputPressed();
                heavyButton.InputPressed();
                uniqueButton.InputPressed();
            }
            else
            {
                lightButton.InputReleased();
                mediumButton.InputReleased();
                heavyButton.InputReleased();
                uniqueButton.InputReleased();
            }
        }

        public void UpButton(InputAction.CallbackContext context)
        {
                upButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
                upButton.InputReleased();
        }

        public void UpFunction(bool isPressed = true)
        {
            if (isPressed)
                upButton.InputPressed();
            else
                upButton.InputReleased();
        }
        public void DownButton(InputAction.CallbackContext context)
        {
                downButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
                downButton.InputReleased();
        }

        public void DownFunction(bool isPressed = true)
        {
            if (isPressed)
                downButton.InputPressed();
            else
                downButton.InputReleased();
        }

        public void LeftButton(InputAction.CallbackContext context)
        {
                leftButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
                leftButton.InputReleased();
        }

        public void LeftFunction(bool isPressed = true)
        {
            if (isPressed)
                leftButton.InputPressed();
            else
                leftButton.InputReleased();
        }

        public void RightButton(InputAction.CallbackContext context)
        {
                rightButton.InputPressed();
            if (context.action.WasReleasedThisFrame())
                rightButton.InputReleased();
        }

        public void RightFunction(bool isPressed = true)
        {
            if (isPressed)
                rightButton.InputPressed();
            else
                rightButton.InputReleased();
        }


        public void StartButton(InputAction.CallbackContext context)
        {
            Managers.Instance.GameManager.PauseGame();
        }

        public void SelectButton(InputAction.CallbackContext context)
        {
            if (Managers.Instance.GameManager.IsTrainingModeOn)
                Managers.Instance.GameManager.ResetPosition();
        }

        public void AddInput(InputType input, bool isPressed)
        {
            if (CheckforRepeatedInputs(input, Time.time))
                inputQueue.Enqueue(new BufferedInput(input, isPressed, Time.time, gameManager.RecordingFrame));
            Debug.Log(input + " " + isPressed);
        }

        public bool CheckforRepeatedInputs(InputType input, float time)
        {
            BufferedInput bufferedInput = inputQueue.Where(c => c.InputType == input && c.IsPressed && c.Time <= time - bufferTime).FirstOrDefault();
            if (bufferedInput != null)
            {
                Debug.Log("Mashing: " + input);
                return false;
            }
            return true;
        }

        public void PerformInput(BufferedInput input)
        {
            if (!input.IsMotion())
            {
                if (!input.IsPressed)
                {
                    BufferedInput emptyInput = new BufferedInput(InputType.NONE, false, 0, 0);
                    input = emptyInput;
                }
                player.PerformInput(input.InputType);
            }
            else
                ProcessInputDirection(input);
        }

        void ProcessInputDirection(BufferedInput input)
        {
            switch(input.InputType)
            {
                case InputType.Up:
                case InputType.Down:
                    if (input.IsPressed)
                    {
                        if (inputDirection.y == 0)
                            inputDirection.y = input.InputType == InputType.Up ? 1 : -1;
                        else
                            inputDirection.y = 0;
                        
                    }
                    else
                    {
                        inputDirection.y = 0;
                        if (upButton.IsPressed)
                            inputDirection.y = 1;
                        if (downButton.IsPressed)
                            inputDirection.y = -1;
                    }
                        break;
                case InputType.Right:
                case InputType.Left:
                    if (input.IsPressed)
                    {
                        if (inputDirection.x == 0)
                            inputDirection.x = input.InputType == InputType.Right ? 1 : -1;
                        else
                            inputDirection.x = 0;
                    }
                    else
                    {
                        inputDirection.x = 0;
                        if (rightButton.IsPressed)
                            inputDirection.x = 1;
                        if (leftButton.IsPressed)
                            inputDirection.x = -1;
                    }
                    break;
            }
            if (!motionInputQueue.Any(c => c.Time == input.Time))
            {
                motionInputQueue.Enqueue(input);
            }
        }

        Vector2 CalculateInputDirection(BufferedInput input)
        {
            Vector2 inputDir = Vector2.zero;
            switch (input.InputType)
            {
                case InputType.Up:
                case InputType.Down:
                    if (input.IsPressed)
                    {
                        inputDir.y = input.InputType == InputType.Up ? 1 : -1;
                    }
                    else
                        inputDir.y = 0;
                    break;
                case InputType.Right:
                case InputType.Left:
                    if (input.IsPressed)
                    {
                        inputDir.x = input.InputType == InputType.Right ? 1 : -1;
                    }
                    else
                        inputDir.x = 0;
                    break;
            }
            return inputDir;
        }

        public void RemapButtonClicked(InputAction actionToRebind)
        {
            var rebindOperation = actionToRebind.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f).Start();
        }

    }
}
