public class SystemSettingsUI : MenuUI
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
