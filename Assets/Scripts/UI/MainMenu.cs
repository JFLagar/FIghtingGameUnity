using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SkillIssue.CharacterSpace;
using System;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RectTransform[] uiElements;

    public InputMapping inputMapping;
    public Button[] buttons;

    public Slider[] elementSliders;
    public Image[] elementIcon;
    public Sprite[] elementSprites;

    private void Start()
    {

    }

    private void Update()
    {
     
    }

    public void OpenUIElement(int id)
    {
        AudioManager.instance.PlaySoundEffect(1);
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
        uiElements[id].gameObject.SetActive(true);
        buttons[id].Select();
    }

    public void StartButton(bool training)
    {
        AudioManager.instance.PlaySoundEffect(1);
        OpenUIElement(3);
    }

    public void StartButtonVSCPU()
    {
        AudioManager.instance.PlaySoundEffect(1);
        OpenUIElement(3);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void OnSliderChange(bool isP2Slider)
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

}
