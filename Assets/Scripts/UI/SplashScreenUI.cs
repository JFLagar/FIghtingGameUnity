using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SplashScreenUI : MonoBehaviour
{
    private PlayerController[] controllers;

    private void Start()
    {
        InputManager.Instance.SwitchToMap("Menu");
        controllers = InputManager.Instance.GetPlayerControllers();
        foreach (var controller in controllers)
        {
            controller._startAction += AssignMainPlayer;
        }
    }

    void AssignMainPlayer(InputAction.CallbackContext ctx, PlayerController controller)
    {
        InputManager.Instance.SetMainPlayerController(controller);
        SceneManager.LoadScene(1);
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
