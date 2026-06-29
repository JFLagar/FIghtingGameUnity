using UnityEngine;
using UnityEngine.UI;
using SkillIssue.CharacterSpace;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;
using System.ComponentModel;

public class BattleUI : MonoBehaviour
{
    Player[] players;
    [SerializeField]
    Slider[] HpSliders;
    [SerializeField]
    TextMeshProUGUI[] comboDisplays;
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    float timer = 99;
    [SerializeField]
    TextMeshProUGUI recoveryDebugText;
    [SerializeField]
    RoundsContainer[] roundsContainers;
    [SerializeField]
    RectTransform pauseUI;

    [SerializeField]
    Image fadePanel;

     int player1WonRounds = 0;
     int player2WonRounds = 0;

    bool roundActive = false;

    bool isInitialized = false;


    // Start is called before the first frame update
    public void Initialize()
    {
        players = Managers.Instance.GameManager.GetPlayers();
        UserData saveData = SaveDataManager.Instance.ActiveSaveData;
        int rounds = saveData.GameSettings.m_BattleSettings.ActiveRoundsNo;
        for (int i = 0; i < HpSliders.Length; i++)
        {
            HpSliders[i].maxValue = players[i].GetMaxHealth();
            HpSliders[i].value = players[i].CurrentHealth;
        }
        foreach (RoundsContainer container in roundsContainers)
        {
            container.CreateRounds(rounds);
        }
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized)
            Managers.Instance.GameManager.SetBattleUI(this);
        if (!roundActive || Managers.Instance.GameManager.IsTrainingModeOn)
            return;
        timer -= Time.deltaTime;
        timerText.text = Mathf.FloorToInt(timer).ToString();
        if (timer <= 0)
        {
            if(HpSliders[0].value > HpSliders[1].value)
            {
                AddScore(0);
            }
            else
            {
                AddScore(1);
            }
        }
    
    }

    public void ResetAll()
    {
        timer = 99;
        timerText.text = Mathf.FloorToInt(timer).ToString();
        foreach (Slider slider in HpSliders)
        {
            slider.value = slider.maxValue;
        }
        foreach (Player player in players)
        {
            player.ResetPlayer();
        }
            ResetScores();
    }

    public void ShowPauseUI(bool showPauseUI)
    {
        if (showPauseUI)
            OpenPauseUI();
        else
            ClosePauseUI();
    }
    private void OpenPauseUI()
    {
        Managers.Instance.InputManager.SwitchToMap("Menu");
        pauseUI.gameObject.SetActive(true);
    }

    private void ClosePauseUI()
    {
        Managers.Instance.InputManager.SwitchToMap("Controls");
        pauseUI.gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        AudioManager.instance.PlaySoundEffect(0);
        Managers.Instance.GameManager.BackToMenu();
    }

    public void Quit()
    {
        Managers.Instance.GameManager.EndGame();
    }

    public void UpdateHealth(int playerId, float value)
    {
        HpSliders[playerId].value = value;
        if (HpSliders[playerId].value <= 0)
        {
            if (playerId == 0)
            {
                AddScore(1);
            }
            else
            {
                AddScore(0);
            }
        }
    }

    public void UpdateComboCounter(int playerId)
    {
        if (players[playerId].GetComboCount() <= 1)
        {
            comboDisplays[playerId].text = "";
        }
        else
        {
            comboDisplays[playerId].text = players[playerId].GetComboCount() + " HIT";
        }
    }

    private void AddScore(int PlayerId)
    {
        Managers.Instance.InputManager.DisableInput();
        roundActive = false;
        if (PlayerId == 0)
            player1WonRounds++;
        else
            player2WonRounds++;
        UpdateScores();
        FadeIn();
        //Change scores
    }

    private void UpdateScores()
    {
        roundsContainers[0].UpdateRounds(player1WonRounds);
        roundsContainers[1].UpdateRounds(player2WonRounds);
    }

    private void ResetScores()
    {
        player1WonRounds = 0;
        player2WonRounds = 0;
        foreach(RoundsContainer container in roundsContainers)
        {
            container.ResetRounds();
        }
    }

    [Button]
    public void FadeIn()
    {
        Managers.Instance.InputManager.DisableInput();
        fadePanel.DOFade(1, 1).OnComplete(()=>
        {
            ResetAll(); FadeOut();
        });
    }

    [Button]
    public void FadeOut()
    {
        fadePanel.DOFade(0, 2).OnComplete(() => 
        {
            Managers.Instance.InputManager.EnableInput();
            roundActive = true; 
        }) ;
    }
}
