using UnityEngine;
using UnityEngine.InputSystem;
using SkillIssue.CharacterSpace;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;
    [SerializeField]
    PlayerController playerControllerPrefab;
    List<PlayerController> playerControllers = new List<PlayerController>();

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.playerPrefab = playerControllerPrefab.gameObject;
        InitializeDevices();
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
        //foreach (InputDevice device in InputSystem.devices)
        //{
        //    if (deviceId >= playerInputManager.maxPlayerCount)
        //        return;
        //    if (device is Keyboard)
        //    {
        //        JoinPlayer(device, deviceId);
        //        deviceId++;
        //    }
        //}
    }

    void JoinPlayer(InputDevice device, int id)
    {
        //playerInputManager.JoinPlayer(pairWithDevice: device);
        PlayerInput player = playerInputManager.JoinPlayer(pairWithDevice: device);
        PlayerController controller = player.GetComponent<PlayerController>();
        SetupController(controller, id);
        Debug.Log(device);
    }

    void SetupController(PlayerController controller, int id)
    {
        Player[] activePlayers = Managers.Instance.GameManager.GetPlayers();
        if (activePlayers.Length < id - 1)
            return;
        controller.Initialize(activePlayers[id], id);
        playerControllers.Add(controller);
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
