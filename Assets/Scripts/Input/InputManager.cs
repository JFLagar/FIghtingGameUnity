using UnityEngine;
using UnityEngine.InputSystem;
using SkillIssue.CharacterSpace;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    PlayerInputManager playerInputManager;
    [SerializeField]
    PlayerController playerControllerPrefab;
    List<PlayerController> playerControllers = new List<PlayerController>();
    PlayerController mainPlayerController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.playerPrefab = playerControllerPrefab.gameObject;
        InitializeDevices();
    }

    public PlayerController GetMainPlayerController()
    {
        return mainPlayerController;
    }

    public PlayerController[] GetPlayerControllers()
    {
        return playerControllers.ToArray();
    }

    public void SetMainPlayerController(PlayerController playerController)
    {
        playerController = mainPlayerController;
    }

    void InitializeDevices()
    {
        int deviceId = 0;
        foreach (InputDevice device in InputSystem.devices)
        {
            if (deviceId >= playerInputManager.maxPlayerCount)
                return;
            if (device is Keyboard)
            {
                JoinPlayer(device, deviceId);
                deviceId++;
            }
        }
        foreach (Gamepad device in Gamepad.all)
        {
            if (deviceId >= playerInputManager.maxPlayerCount)
                return;
            JoinPlayer(device, deviceId);
            deviceId++;
        }
        foreach (Joystick device in Joystick.all)
        {
            if (deviceId >= playerInputManager.maxPlayerCount)
                return;
            JoinPlayer(device, deviceId);
            deviceId++;
        }

    }

    void JoinPlayer(InputDevice device, int id)
    {
        PlayerInput player = playerInputManager.JoinPlayer(pairWithDevice: device);
        player.SwitchCurrentActionMap("Controls");
        player.gameObject.transform.parent = gameObject.transform;
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.Id = id;
        playerControllers.Add(controller);
    }

    public void SetupController(int id)
    {
        Player[] activePlayers = Managers.Instance.GameManager.GetPlayers();
        if (activePlayers.Length < id - 1)
            return;
        playerControllers[id].Initialize(activePlayers[id], id);
    }

    public void DisableInput()
    {
        foreach (PlayerController controller in playerControllers)
        {
            controller.GetPlayerInput().DeactivateInput();
        }
    }

    public void EnableInput()
    {
        foreach (PlayerController controller in playerControllers)
        {
            controller.GetPlayerInput().ActivateInput();
        }
    }

    public void SwitchToMap(string map)
    {
        foreach (PlayerController controller in playerControllers)
        {
            controller.GetPlayerInput().SwitchCurrentActionMap(map);
        }
    }
}
