using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class SplashScreenUI : MonoBehaviour
{
    private PlayerController[] controllers;
    [SerializeField]
    private TextMeshProUGUI splashArt;
    [SerializeField]
    private Image saveDataMessage;
    [SerializeField]
    private TextMeshProUGUI saveDataText;
    [SerializeField]
    float fadeIn;
    [SerializeField]
    float fadeOut;


    private void Start()
    {
        InputManager.Instance.SwitchToMap("Menu");
        controllers = InputManager.Instance.GetPlayerControllers();
        foreach (var controller in controllers)
        {
            controller._startAction += AssignMainPlayer;
        }
        splashArt.DOFade(1, fadeIn).OnComplete(() =>
        {
            splashArt.DOFade(0, fadeOut).OnComplete(() =>
            {
                saveDataMessage.gameObject.SetActive(true);
                CheckForSaveData();
            });
        });
    }

    void AssignMainPlayer(InputAction.CallbackContext ctx, PlayerController controller)
    {
        if (!saveDataMessage.gameObject.activeSelf)
            return;
        InputManager.Instance.SetMainPlayerController(controller);
        SceneManager.LoadScene(1);
    }

    private void CheckForSaveData()
    {

        if (SaveDataManager.Instance.CheckData())
        {
            saveDataText.text = "Loading Save Data";
        }
        else
            saveDataText.text = "Creating Save Data";
        SaveDataManager.Instance.LoadData();
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
        foreach (var controller in controllers)
        {
            controller._startAction -= AssignMainPlayer;
        }
    }
}
