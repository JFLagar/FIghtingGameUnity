using SkillIssue.CharacterSpace;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int Id;
    [SerializeField]
    private Player assignedPlayer;

    public void Initialize(Player player, int playerId)
    {

        assignedPlayer = player;
        player.Initialize(this);
    }

    public PlayerInput GetPlayerInput()
    {
        return GetComponent<PlayerInput>();
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
}