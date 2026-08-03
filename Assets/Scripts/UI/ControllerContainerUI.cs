using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControllerContainerUI : MonoBehaviour
{
    [SerializeField]
    Image controllerImage;
    [ReadOnly]
    [SerializeField]
    int position = 0;
    ControllerDisplayUI controllerDisplayUI;
    PlayerController playerController;

    public void SubscribeToEvent(PlayerController controller, ControllerDisplayUI parent)
    {
        position = 0;
        playerController = controller;
        controllerImage.gameObject.SetActive(true);
        controller._UINavigation += SetController;
        controller._UICancel += CancelScreen;
        controllerDisplayUI = parent;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public int GetPosition()
    {
        return position;
    }

    public void UnsubscribeToEvent(PlayerController controller)
    {
        controller._UINavigation -= SetController;
        controller._UICancel -= CancelScreen;
    }

    public void CancelScreen()
    {
        controllerDisplayUI.CancelControllerDisplay();
    }

    public void SetController(int direction)
    {
         int calcPosition = CalculatePosition(direction);
        switch (calcPosition)
        { case -1:
                if (controllerDisplayUI.GetP1Controller() != null && position != calcPosition)
                {
                    calcPosition = 0;
                }
                break;
            case 1:
                if (controllerDisplayUI.GetP2Controller() != null && position != calcPosition)
                {
                    calcPosition = 0;

                }
                break;
            case 0:
                break;
        }
        
        controllerImage.transform.localPosition = new Vector3(calcPosition * 100, 0,0);
        position = calcPosition;
    }

    private int CalculatePosition(int direction)
    {
        int result = position;
        switch (direction)
        {
            case -1:
                {
                    if (position == -1)
                        result = -1;
                    else
                        result--;
                }
                break;
            case 1:
                {
                    if (position == 1)
                        result = 1;
                    else
                        result++;
                }
                break;
        }
        
        return result;
    }
}
