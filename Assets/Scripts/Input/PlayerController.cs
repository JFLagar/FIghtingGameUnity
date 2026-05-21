using SkillIssue.CharacterSpace;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int Id;
    [SerializeField]
    private Player assignedPlayer;
    public Action<PlayerController> _startAction;
    public Action<int> _UINavigation;
    [SerializeField]
    private MultiplayerEventSystem eventSystem;
    private InputDevice controllingDevice; 

    public void Initialize(Player player, int playerId)
    {
        assignedPlayer = player;
        player.Initialize(this);
    }

    public PlayerInput GetPlayerInput()
    {
        return GetComponent<PlayerInput>();
    }

    public void SetInputDevice(InputDevice device)
    {
        controllingDevice = device;
    }

    public InputDevice GetControllingDevice()
    {
        return controllingDevice;
    }

    public void SetMainController()
    {
        eventSystem.playerRoot = FindAnyObjectByType<Canvas>().gameObject;
    }

    public void SetPlayerUI(GameObject uiGameObject, Selectable selectable)
    {
        eventSystem.playerRoot = uiGameObject;
        SelectUIElement(selectable);
    }

    public void SelectUIElement(Selectable selectable)
    {
        eventSystem.SetSelectedGameObject(selectable.gameObject);
    }

    // UNITY EVENTS
    public void UpButton(InputAction.CallbackContext cxt)
    {
        if (cxt.phase == InputActionPhase.Started)
            return;
        assignedPlayer.GetInputHandler().UpButton(cxt);
    }

    public void DownButton(InputAction.CallbackContext cxt)
    {
        if (cxt.phase == InputActionPhase.Started)
            return;
        assignedPlayer.GetInputHandler().DownButton(cxt);
    }

    public void LeftButton(InputAction.CallbackContext cxt)
    {
        if (cxt.phase == InputActionPhase.Started)
            return;
        assignedPlayer.GetInputHandler().LeftButton(cxt);
    }

    public void RightButton(InputAction.CallbackContext cxt)
    {
        if (cxt.phase == InputActionPhase.Started)
            return;
        assignedPlayer.GetInputHandler().RightButton(cxt);
    }

    public void LightButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().LightButton(cxt);
    }

    public void MediumButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().MediumButton(cxt);
    }

    public void HeavyButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().HeavyButton(cxt);
    }

    public void UniqueButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().UniqueButton(cxt);
    }
    public void LMButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().LMButton(cxt);
    }

    public void LMHButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().LMHButton(cxt);
    }

    public void LMHUButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().LMHUButton(cxt);
    }

    public void LUButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().LUButton(cxt);
    }

    public void MHButton(InputAction.CallbackContext cxt)
    {
        assignedPlayer.GetInputHandler().MHButton(cxt);
    }

    public void StatButton(InputAction.CallbackContext cxt)
    {
        if (_startAction == null) 
            return;
        if (cxt.phase == InputActionPhase.Started)
            _startAction.Invoke(this);
    }

    public void UINavigation(InputAction.CallbackContext cxt)
    {
        if (_UINavigation == null)
            return;
        if (cxt.ReadValue<Vector2>().y != 0)
            return;
        _UINavigation.Invoke((int)cxt.ReadValue<Vector2>().x);
    }
}