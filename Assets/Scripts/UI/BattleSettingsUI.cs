using UnityEngine;
using SkillIssue;
using System.Linq;

public class BattleSettingsUI : SettingsPanelUI
{
    [SerializeField]
    private SettingSelector roundsSelector;
    [SerializeField]
    private SettingSelector timerSelector;
    private BattleSettings battleSettings;

    private void OnEnable()
    {
        battleSettings = SaveDataManager.Instance.ActiveSaveData.GameSettings.m_BattleSettings;
        //Initialize Selectors
        roundsSelector.InitializeValues(battleSettings.RoundsId, 0, battleSettings.Rounds.Length - 1, this);
        timerSelector.InitializeValues(battleSettings.TimerId,0, battleSettings.Timers.Length - 1, this);
        OnRoundsValueChanged(battleSettings.RoundsId);
        OnTimerValueChanged(battleSettings.TimerId);
        //Subscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation += MoveSelector;
        roundsSelector._selectorAction += OnRoundsValueChanged;
        timerSelector._selectorAction += OnTimerValueChanged;
    }
   
    private void OnRoundsValueChanged(int value)
    {
        roundsSelector.SetSelectionText(battleSettings.Rounds[value].ToString());
        SaveDataManager.Instance.ActiveSaveData.GameSettings.m_BattleSettings.RoundsId = value;
        SaveDataManager.Instance.SaveData();
    }

    private void OnTimerValueChanged(int value)
    {
        timerSelector.SetSelectionText(battleSettings.Timers[value].ToString());
        if (battleSettings.Timers[value] == 0)
        {
            //PLACEHOLDER FOR INFINITE CHAR
            timerSelector.SetSelectionText("%");
        }
        SaveDataManager.Instance.ActiveSaveData.GameSettings.m_BattleSettings.TimerId = value;
        SaveDataManager.Instance.SaveData();
    }

    private void OnDisable()
    {
        //Unsubscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation -= MoveSelector;
        roundsSelector._selectorAction -= OnRoundsValueChanged;
        timerSelector._selectorAction -= OnTimerValueChanged;
    }

    public void OnTopUISliderChange()
    {
        //Change the position of the top UI
    }

    public void OnBottomUISliderChange()
    {
        //Change the position of the bottom UI
    }

    public void DeleteReplayData()
    {
        //Deletes all Replays
        return;
    }
}
