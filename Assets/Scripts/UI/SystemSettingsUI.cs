public class SystemSettingsUI : SettingsPanelUI
{ 
    private void OnEnable()
    {
        //Initialize Selectors
    }

    public void DeleteSaveData()
    {
        SaveDataManager.Instance.CreateNewSave();
    }
}
