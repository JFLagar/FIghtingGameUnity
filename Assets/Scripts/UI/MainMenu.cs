using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public RectTransform[] uiElements;
    //public Button[] buttons;

    public void OpenUIElement(int id)
    {
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
        uiElements[id].gameObject.SetActive(true);
        //buttons[id].Select();
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
        SceneManager.LoadScene(1);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

}
