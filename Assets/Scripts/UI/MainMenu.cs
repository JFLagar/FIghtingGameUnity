using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Samples.RebindUI;

public class MainMenu : MonoBehaviour
{
    public RectTransform[] uiElements;
    public Selectable[] selectables;

    private void Start()
    {
        InputManager.Instance.SwitchToMap("Menu");
        InputManager.Instance.GetMainPlayerController().SetPlayerUI(gameObject, selectables[0]);
    }

    public void OpenUIElement(int id)
    {
        foreach (RectTransform transform in uiElements)
        {
            transform.gameObject.SetActive(false);
        }
        uiElements[id].gameObject.SetActive(true);
        InputManager.Instance.GetMainPlayerController().SelectUIElement(selectables[id]);
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
        SceneManager.LoadScene(2);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ResetInputMapping()
    {
        RebindActionUI[] rebindActionUIs = FindObjectsByType<RebindActionUI>(FindObjectsSortMode.None);
        foreach(var action in rebindActionUIs)
        {
            action.ResetToDefault();
        }
    }

}
