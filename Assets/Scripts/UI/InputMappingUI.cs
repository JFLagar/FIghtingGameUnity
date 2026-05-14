using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputMappingUI : MonoBehaviour
{
    public RectTransform[] inputMappings;
    public RectTransform confirmPanel;
    private PlayerController[] activePlayers;
    public Selectable[] selectables;

    private void OnEnable()
    {
        activePlayers = InputManager.Instance.GetPlayerControllers();
        foreach (PlayerController playerController in activePlayers)
        {
            playerController._startAction += OpenInputMapping;
        }
    }

    public void ToggleConfirmPanel(bool toggle)
    {
        confirmPanel.gameObject.SetActive(toggle);
    }

    public void OpenInputMapping(InputAction.CallbackContext ctx, PlayerController controller)
    {
        ToggleConfirmPanel(false);
        foreach (var rect in inputMappings)
        {
            rect.gameObject.SetActive(false);
        }

        if (ctx.control.device is Keyboard)
        {
            controller.SetPlayerUI(inputMappings[0].gameObject, selectables[0]);
            inputMappings[0].gameObject.SetActive(true);
        }
        else if (ctx.control.device is Gamepad)
        {
            controller.SetPlayerUI(inputMappings[1].gameObject, selectables[1]);
            inputMappings[1].gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        foreach (PlayerController playerController in activePlayers)
        {
            playerController._startAction -= OpenInputMapping;
        }
    }
}

