using UnityEngine;
public class DisplaySettingsUI : SettingsPanelUI
{
    [SerializeField]
    private SettingSelector windowModeSelector;
    [SerializeField]
    private SettingSelector resolutionSelector;
    private DisplaySettings displaySettings;

    private void OnEnable()
    {
        displaySettings = SaveDataManager.Instance.ActiveSaveData.GameSettings.m_DisplaySettings;
        //Initialize Selectors
        windowModeSelector.InitializeValues(displaySettings.WindowModeId, 0, displaySettings.WindowModes.Length - 1, this);
        resolutionSelector.InitializeValues(displaySettings.ResolutionId, 0 , displaySettings.Resolutions.Length - 1, this);
        OnWindowValueChanged(displaySettings.WindowModeId);
        OnResolutionValueChanged(displaySettings.ResolutionId);
        //Subscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation += MoveSelector;
        windowModeSelector._selectorAction += OnWindowValueChanged;
        resolutionSelector._selectorAction += OnResolutionValueChanged;
    }

    private void OnWindowValueChanged(int value)
    {
        windowModeSelector.SetSelectionText(displaySettings.WindowModes[value].ToString());
        displaySettings.WindowModeId = value;
        Screen.fullScreenMode = displaySettings.WindowModes[value];
        SaveValues();
    }

    private void OnResolutionValueChanged(int value)
    {
        resolutionSelector.SetSelectionText(displaySettings.Resolutions[value].ToString());
        displaySettings.ResolutionId = value;
        if (displaySettings.WindowModes[displaySettings.WindowModeId] != FullScreenMode.FullScreenWindow)
        {
            Vector2 resolution = displaySettings.Resolutions[value];
            Screen.SetResolution((int)resolution.x, (int)resolution.y, Screen.fullScreenMode);
        }
        SaveValues();
    }

    public override void SaveValues()
    {
        SaveDataManager.Instance.ActiveSaveData.GameSettings.m_DisplaySettings = displaySettings;
        base.SaveValues();
    }

    private void OnDisable()
    {
        //Unsubscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation -= MoveSelector;
        windowModeSelector._selectorAction -= OnWindowValueChanged;
        resolutionSelector._selectorAction -= OnResolutionValueChanged;
    }
}


