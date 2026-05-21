using UnityEngine;
using SkillIssue;

public class BattleSettingsUI : SettingsPanelUI
{
    [SerializeField]
    private SettingSelector roundsSelector;
    [SerializeField]
    private int[] rounds;
    [SerializeField]
    private int defaultRoundsId;
    [SerializeField]
    private SettingSelector timerSelector;
    [SerializeField]
    private int[] timers;
    [SerializeField]
    private int defaultTimersId;
    private void OnEnable()
    {
        //Initialize Selectors
        roundsSelector.InitializeValues(defaultTimersId, 0, rounds.Length - 1, this);
        timerSelector.InitializeValues(defaultRoundsId,0, timers.Length - 1, this);
        OnRoundsValueChanged(defaultTimersId);
        OnTimerValueChanged(defaultTimersId);
        //Subscribe to events
        InputManager.Instance.GetMainPlayerController()._UINavigation += MoveSelector;
        roundsSelector._selectorAction += OnRoundsValueChanged;
        timerSelector._selectorAction += OnTimerValueChanged;
    }
   
    private void OnRoundsValueChanged(int value)
    {
        roundsSelector.SetSelectionText(rounds[value].ToString());
    }

    private void OnTimerValueChanged(int value)
    {
        timerSelector.SetSelectionText(timers[value].ToString());
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
