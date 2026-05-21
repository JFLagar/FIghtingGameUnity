using UnityEngine;
public class DisplaySettingsUI : SettingsPanelUI
{
    [SerializeField]
    private SettingSelector windowModeSelector;
    [SerializeField]
    private FullScreenMode[] windowModes;
    [SerializeField]
    private int defaultWindowId;
    [SerializeField]
    private SettingSelector resolutionSelector;
    [SerializeField]
    private Vector2[] resolutions;
    [SerializeField]
    private int defaultResolutionId;

    private void OnEnable()
    {
        //Initialize Selectors
        windowModeSelector.InitializeValues(defaultWindowId, 0, windowModes.Length - 1, this);
        resolutionSelector.InitializeValues(defaultResolutionId, 0 , resolutions.Length - 1, this);
        OnWindowValueChanged(defaultResolutionId);
        OnResolutionValueChanged(defaultResolutionId);
        //Subscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation += MoveSelector;
        windowModeSelector._selectorAction += OnWindowValueChanged;
        resolutionSelector._selectorAction += OnResolutionValueChanged;
    }

    private void OnWindowValueChanged(int value)
    {
        windowModeSelector.SetSelectionText(windowModes[value].ToString());
    }

    private void OnResolutionValueChanged(int value)
    {
        resolutionSelector.SetSelectionText(resolutions[value].ToString());
    }

    private void OnDisable()
    {
        //Unsubscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation -= MoveSelector;
        windowModeSelector._selectorAction -= OnWindowValueChanged;
        resolutionSelector._selectorAction -= OnResolutionValueChanged;
    }
}


