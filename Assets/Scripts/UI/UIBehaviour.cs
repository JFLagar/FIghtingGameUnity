using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillIssue.CharacterSpace;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Linq;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField]
    Character[] characters;
    [SerializeField]
    Slider[] sliders;
    [SerializeField]
    Slider[] elementSliders;
    [SerializeField]
    Image[] elementIcon;
    [SerializeField]
    Sprite[] elementSprites;
    [SerializeField]
    TextMeshProUGUI[] comboDisplays;
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    float timer = 99;
    [SerializeField]
    TextMeshProUGUI debug;
    [SerializeField]
    Image[] p1RoundsIcons, p2RoundsIcons;
    [SerializeField]
    RectTransform pauseUI;
    [SerializeField]
    Button characterSelect;
    [SerializeField]
    RectTransform characterSelectUI;

    [SerializeField]
    Image fadePanel;

     int player1WonRounds = 0;
     int player2WonRounds = 0;

    bool roundActive = false;


    // Start is called before the first frame update
    public void Initialize()
    {
    
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maxValue = characters[i].GetMaxHealth();
            sliders[i].value = characters[i].GetCurrentHealth();
        }

        if (Managers.Instance.GameManager.IsTraining())
        {
            characterSelect.gameObject.SetActive(true);
        }
        else
        {
            characterSelect.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!roundActive || Managers.Instance.GameManager.IsTraining())
            return;
        timer -= Time.deltaTime;
        timerText.text = Mathf.FloorToInt(timer).ToString();
        if (timer <= 0)
        {
            if(sliders[0].value > sliders[1].value)
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
        foreach (Slider slider in sliders)
        {
            slider.value = slider.maxValue;
        }
        foreach (Character character in characters)
        {
            character.ResetCharacter();
        }
        if (player1WonRounds == p1RoundsIcons.Length || player2WonRounds == p2RoundsIcons.Length)
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
        foreach(Character character in characters)
        {
            // character.GetInputHandler().GetPlayerInput().SwitchCurrentActionMap("Menu");
        }
        pauseUI.gameObject.SetActive(true);
    }

    private void ClosePauseUI()
    {
        pauseUI.gameObject.SetActive(false);
        foreach (Character character in characters)
        {
            // character.GetInputHandler().GetPlayerInput().SwitchCurrentActionMap("Controls");
        }
    }

    public void MainMenu()
    {
        AudioManager.instance.PlaySoundEffect(0);
        Managers.Instance.GameManager.BackToMenu();
    }

    public void Quit()
    {
        AudioManager.instance.PlaySoundEffect(0);
        Managers.Instance.GameManager.EndGame();
    }

    public void OpenCharacterSelect()
    {
        AudioManager.instance.PlaySoundEffect(0);
        pauseUI.gameObject.SetActive(false);
        characterSelectUI.gameObject.SetActive(true);
        elementSliders[0].Select();

    }

    public void CloseCharacterSelect()
    {
        AudioManager.instance.PlaySoundEffect(0);
        pauseUI.gameObject.SetActive(true);
        characterSelectUI.gameObject.SetActive(false);
        characterSelect.Select();
    }

    public void OnElementSliderChange(bool isP2Slider)
    {
        AudioManager.instance.PlaySoundEffect(0);
        int playerId = isP2Slider ? 1 : 0;
        switch (elementSliders[playerId].value)
        {
            //Here add to the local persistence manager which character 
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    public void UpdateHealth(int playerId, float value)
    {
        sliders[playerId].value = value;
        if (sliders[playerId].value <= 0)
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
        if (characters[playerId].GetComboCount() <= 1)
        {
            comboDisplays[playerId].text = "";
        }
        else
        {
            comboDisplays[playerId].text = characters[playerId].GetComboCount() + " HIT";
        }
    }

    private void AddScore(int PlayerId)
    {
        foreach (var player in characters)
        {
            player.DisableInput();
        }
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
        for (int rounds = 0; rounds < player1WonRounds; rounds++)
        {
            if (player1WonRounds > p1RoundsIcons.Length)
            {
                return;
            }
            p1RoundsIcons[rounds].enabled = true;
        }
        for (int rounds = 0; rounds < player2WonRounds; rounds++)
        {
            if (player2WonRounds > p2RoundsIcons.Length)
            {
                return;
            }
            p2RoundsIcons[rounds].enabled = true;
        }
    }

    private void ResetScores()
    {
        player1WonRounds = 0;
        player2WonRounds = 0;
        foreach (var icon in p1RoundsIcons)
        {
            icon.enabled = false;
        }
        foreach (var icon in p2RoundsIcons)
        {
            icon.enabled = false;
        }
    }

    [Button]
    public void FadeIn()
    {
        fadePanel.DOFade(1, 1).OnComplete(()=>
        {
            ResetAll(); FadeOut();
        });
    }

    [Button]
    public void FadeOut()
    {
        fadePanel.DOFade(0, 3).OnComplete(() => 
        {
            foreach(var player in characters)
            {
                player.EnableInput();
            }
            roundActive = true; 
        }) ;
    }
}
