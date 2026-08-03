using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerDisplayUI : MonoBehaviour
{
    [SerializeField]
    ControllerContainerUI[] controllers;
    ControllerContainerUI p1Controller;
    ControllerContainerUI p2Controller;
    [SerializeField]
    MainMenuUI mainMenuUI;

    private void OnEnable()
    {
        for (int i = 0; i <= InputManager.Instance.GetPlayerControllers().Length - 1; i++)
        {
            controllers[i].SubscribeToEvent(InputManager.Instance.GetPlayerControllers()[i], this);
        }
    }

    public ControllerContainerUI GetP1Controller()
    {
        p1Controller = controllers.FirstOrDefault(c => c.GetPosition() == -1);
        return p1Controller;
    }

    public ControllerContainerUI GetP2Controller()
    {
        p2Controller = controllers.FirstOrDefault(c => c.GetPosition() == 1);
        return p2Controller;
    }

    public void StartGame()
    {
        GetP1Controller();
        if (p1Controller != null)
            InputManager.Instance.SetPlayerController(p1Controller.GetPlayerController(), true);
        GetP2Controller();
        if (p2Controller != null)
            InputManager.Instance.SetPlayerController(p2Controller.GetPlayerController(), false);

        if (InputManager.Instance.AnyActivePlayers())
            SceneManager.LoadScene(2);
    }

    public void CancelControllerDisplay()
    {
        mainMenuUI.OpenUIElement(0);
    }

    private void OnDisable()
    {
        for (int i = 0; i <= InputManager.Instance.GetPlayerControllers().Length - 1; i++)
        {
            controllers[i].UnsubscribeToEvent(InputManager.Instance.GetPlayerControllers()[i]);
        }
    }
}
