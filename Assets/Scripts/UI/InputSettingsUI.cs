using UnityEngine;
using UnityEngine.UI;

public class InputSettingsUI : MenuUI
{
    [SerializeField]
    private GameObject canvasUI;
    public InputMappingUI[] inputMappingUIs;
    private void OnEnable()
    {
        //Initialize Selectors
    }

    public override void CloseUIElements(Selectable selectable)
    {
        if (UIClosed())
        {
            InputManager.Instance.GetMainPlayerController().SetPlayerUI(canvasUI, selectable);
            base.CloseUIElements(selectable);
        }

    }

    private bool UIClosed()
    {
        foreach (var inputMappingUI in inputMappingUIs)
        {
            if ( !inputMappingUI.IsActive())
            {
                return false;
            }
        }
        return true;
    }
}
