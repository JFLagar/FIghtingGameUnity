public class SystemSettingsUI : SettingsPanelUI
{ 
    private void OnEnable()
    {
        //Initialize Selectors
    }

    public void DeleteSaveData()
    {
        SaveDataManager.Instance.SaveData(new UserData());
    }
}
