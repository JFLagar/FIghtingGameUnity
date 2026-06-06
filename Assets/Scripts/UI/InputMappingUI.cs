using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class InputMappingUI : MonoBehaviour
{
    public RectTransform[] inputMappings;
    public RectTransform confirmPanel;
    private PlayerController mappingPlayer;
    public RebindButton[] selectables;
    [SerializeField]
    private int id;
    [SerializeField]
    private RebindButton[] rebindButtonsKeyboard;
    [SerializeField]
    private RebindButton[] rebindButtonsGamepad;

    private void OnEnable()
    {
        mappingPlayer = InputManager.Instance.GetPlayerControllers()[id];
        if (mappingPlayer != null )
        {
            mappingPlayer._startAction += OpenInputMapping;
        }
        if (id == InputManager.Instance.GetMainPlayerController().Id)
            OpenInputMapping(mappingPlayer);
        for (int i = 0; i <= rebindButtonsKeyboard.Length -1; i++)
        {
            rebindButtonsKeyboard[i].Initialize(mappingPlayer, i, 0);
        }
        for (int i = 0; i <= rebindButtonsGamepad.Length - 1; i++)
        {
            rebindButtonsGamepad[i].Initialize(mappingPlayer, i, 1);
        }
    }

    public PlayerController GetPlayerController()
    {
        return mappingPlayer;
    }

    public bool IsActive()
    {
        return confirmPanel.gameObject.activeSelf;
    }

    public void ResetInputMapping()
    {
        RebindActionUI[] rebindActionUIs = FindObjectsByType<RebindActionUI>(FindObjectsSortMode.None);
        foreach (var action in rebindActionUIs)
        {
            action.ResetToDefault();
        }
    }

    public void ToggleConfirmPanel(bool toggle)
    {
        confirmPanel.gameObject.SetActive(toggle);
        if (toggle)
        {
            foreach (var rect in inputMappings)
            {
                rect.gameObject.SetActive(false);
            }
        }
    }

    public void OpenInputMapping(PlayerController controller)
    {
        if (!confirmPanel.gameObject.activeInHierarchy)
        {
            return;
        }

        ToggleConfirmPanel(false);
        foreach (var rect in inputMappings)
        {
            rect.gameObject.SetActive(false);
        }

        if (controller.GetControllingDevice() is Keyboard)
        {
            controller.SetPlayerUI(inputMappings[0].gameObject, selectables[0]);
            inputMappings[0].gameObject.SetActive(true);
            selectables[0].SubscribeToEvent(controller);
        }
        else if (controller.GetControllingDevice() is Gamepad)
        {
            controller.SetPlayerUI(inputMappings[1].gameObject, selectables[1]);
            inputMappings[1].gameObject.SetActive(true);
            selectables[1].SubscribeToEvent(controller);
        }
    }

    private void OnDisable()
    {
        if (mappingPlayer != null)
        {
            mappingPlayer._startAction -= OpenInputMapping;
        }
    }

}

