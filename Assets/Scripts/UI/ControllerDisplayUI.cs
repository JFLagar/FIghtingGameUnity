using UnityEngine;

public class ControllerDisplayUI : MonoBehaviour
{
    [SerializeField]
    ControllerContainerUI[] controllers;
    ControllerContainerUI p1Controller;
    ControllerContainerUI p2Controller;

    private void OnEnable()
    {
        for (int i = 0; i <= InputManager.Instance.GetPlayerControllers().Length - 1; i++)
        {
            controllers[i].SubscribeToEvent(InputManager.Instance.GetPlayerControllers()[i], this);
        }
    }

    public ControllerContainerUI GetP1Controller()
    {
        return p1Controller;
    }

    public ControllerContainerUI GetP2Controller()
    {
        return p2Controller;
    }

    public void SetController(ControllerContainerUI controller, bool isP1)
    {
        if (isP1)
            p1Controller = controller;
        else
            p2Controller = controller;
        InputManager.Instance.SetPlayerController(controller.GetPlayerController(), isP1);
    }

    private void OnDisable()
    {
        for (int i = 0; i <= InputManager.Instance.GetPlayerControllers().Length - 1; i++)
        {
            controllers[i].UnsubscribeToEvent(InputManager.Instance.GetPlayerControllers()[i]);
        }
    }
}
