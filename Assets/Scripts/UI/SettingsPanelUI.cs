using UnityEngine;

public class SettingsPanelUI : MonoBehaviour
{
    private SettingSelector activeSelector;
    public virtual void SetSelectedSelector(SettingSelector selector)
    {
        activeSelector = selector;
    }

    public virtual SettingSelector GetActiveSelector()
    {
        return activeSelector;
    }

    public virtual void MoveSelector(int direction)
    {
        if (direction > 0)
            activeSelector.MoveRight();
        else
            activeSelector.MoveLeft();
    }

    public virtual void SaveValues()
    {
        SaveDataManager.Instance.SaveData();
    }
}
