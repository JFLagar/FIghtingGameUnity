using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillIssue.CharacterSpace;
using TMPro;
using System;

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
    Image[] p1Rounds, p2Rounds;
    [SerializeField]
    RectTransform pauseUI;
    [SerializeField]
    Button characterSelect;
    [SerializeField]
    RectTransform characterSelectUI;

    private int player1WonRounds;
    private int player2WonRounds;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maxValue = characters[i].maxHealth;
            sliders[i].value = characters[i].currentHealth;
            elementSliders[i].value = (float)characters[i].element;
            elementIcon[i].sprite = elementSprites[(int)characters[i].element];
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

    internal void ResetAll()
    {
        foreach(Slider slider in sliders)
        {
            slider.value = slider.maxValue;
            timer = 99;
        }
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
        Time.timeScale = 0;
        foreach(Character character in characters)
        {
            character.inputHandler.playerInput.SwitchCurrentActionMap("Menu");
        }
        pauseUI.gameObject.SetActive(true);
    }

    private void ClosePauseUI()
    {
        pauseUI.gameObject.SetActive(false);
        foreach (Character character in characters)
        {
            character.inputHandler.playerInput.SwitchCurrentActionMap("Controls");
        }
        Time.timeScale = 1;
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
        int playerId = 0;
        playerId = isP2Slider ? 1 : 0;
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
        if (characters[playerId].comboHit <= 1)
        {
            comboDisplays[playerId].text = "";
        }
        else
        {
            comboDisplays[playerId].text = characters[playerId].comboHit + " HIT";
        }
    }

    private void AddScore(int PlayerId)
    {
        //Change scores
        Managers.Instance.GameManager.ResetPosition();
    }

    private void ResetScores()
    {
        //Reset Scores
        Managers.Instance.GameManager.ResetPosition();
    }
}
