using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class InputMappingUI : MonoBehaviour
{
    public RectTransform[] inputMappings;
    public RectTransform confirmPanel;
    private PlayerController mappingPlayer;
    public Selectable[] selectables;
    [SerializeField]
    private int id;

    private void OnEnable()
    {
        mappingPlayer = InputManager.Instance.GetPlayerControllers()[id];
        if (mappingPlayer != null )
        {
            mappingPlayer._startAction += OpenInputMapping;
        }
        if (id == InputManager.Instance.GetMainPlayerController().Id)
            OpenInputMapping(mappingPlayer);
    }

    public bool IsActive()
    {
        return confirmPanel.gameObject.activeSelf;
    }

    public void ResetInputMapping()
    {
        Debug.Log("Reseting");
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
        }
        else if (controller.GetControllingDevice() is Gamepad)
        {
            controller.SetPlayerUI(inputMappings[1].gameObject, selectables[1]);
            inputMappings[1].gameObject.SetActive(true);
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

