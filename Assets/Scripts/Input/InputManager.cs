using UnityEngine;
using UnityEngine.InputSystem;
using SkillIssue.CharacterSpace;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    PlayerInputManager playerInputManager;
    [SerializeField]
    PlayerController playerControllerPrefab;
    [ReadOnly]
    [SerializeField]
    List<PlayerController> playerControllers = new List<PlayerController>();
    [ReadOnly]
    [SerializeField]
    PlayerController mainPlayerController;
    [ReadOnly]
    [SerializeField]
    PlayerController player1Controller;
    [ReadOnly]
    [SerializeField]
    PlayerController player2Controller;
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

    public void SetPlayerController(PlayerController playerController, bool isP1)
    {
        if (isP1)
            player1Controller = playerController;
        else
            player2Controller = playerController;
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
        if (playerController == null)
            return;
        //Swap the IDs between controllers
        if (playerController.Id != 0)
        {
            playerControllers.FirstOrDefault(c => c.Id == 0).Id = playerController.Id;
            playerController.Id = 0;
        }
        mainPlayerController = playerController;
        playerControllers = playerControllers.OrderBy(c => c.Id).ToList();
        playerController.SetMainController();
    }

    void InitializeDevices()
    {
        int deviceId = 0;

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
    }

    void JoinPlayer(InputDevice device, int id)
    {
        PlayerInput player = playerInputManager.JoinPlayer(pairWithDevice: device);
        player.SwitchCurrentActionMap("Controls");
        player.gameObject.transform.parent = gameObject.transform;
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.Id = id;
        controller.SetInputDevice(device);
        if (controller.Id == 0)
            SetMainPlayerController(controller);
        playerControllers.Add(controller);
    }

    public void SetupController(int id)
    {
        Player[] activePlayers = Managers.Instance.GameManager.GetPlayers();
        if (activePlayers.Length < id - 1)
            return;
        switch (id)
        {
            case 0:
                if (player1Controller != null)
                    player1Controller.Initialize(activePlayers[id], id);
                else
                    activePlayers[id].Initialize(null);
                break;
            case 1:
                if (player2Controller != null)
                    player2Controller.Initialize(activePlayers[id], id);
                else
                    activePlayers[id].Initialize(null);
                break;
        }
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

    public bool AnyActivePlayers()
    {
        return (player1Controller != null || player2Controller != null);
    }
}
