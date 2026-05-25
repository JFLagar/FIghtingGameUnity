using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MenuUI
{
    private void Start()
    {
        InputManager.Instance.SwitchToMap("Menu");
        OpenUIElement(0);
    }

    public void StartGame()
    {
        if (InputManager.Instance.AnyActivePlayers())
        SceneManager.LoadScene(2);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

