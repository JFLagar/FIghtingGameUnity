using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControllerContainerUI : MonoBehaviour
{
    [SerializeField]
    Image controllerImage;
    int position = 0;
    ControllerDisplayUI controllerDisplayUI;
    PlayerController playerController;

    public void SubscribeToEvent(PlayerController controller, ControllerDisplayUI parent)
    {
        playerController = controller;
        controllerImage.gameObject.SetActive(true);
        controller._UINavigation += SetController;
        controllerDisplayUI = parent;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public void UnsubscribeToEvent(PlayerController controller)
    {
        controller._UINavigation -= SetController;
    }

    public void SetController(int direction)
    {
        position = CalculatePosition(direction);
        switch (position)
        { case -1:
                if (controllerDisplayUI.GetP1Controller() != null)
                {
                    position = 0;
                    return;
                }
                controllerDisplayUI.SetController(this,true);
                break;
            case 1:
                if (controllerDisplayUI.GetP2Controller() != null)
                {
                    position = 0;
                    return;
                }
                controllerDisplayUI.SetController(this, false);
                break;
            case 0:
                if (controllerDisplayUI.GetP1Controller() == this)
                {
                    controllerDisplayUI.SetController(null, true);
                }
                if (controllerDisplayUI.GetP2Controller() == this)
                {
                    controllerDisplayUI.SetController(null, false);
                }
                break;
        }
        
        controllerImage.transform.DOLocalMove(new Vector3(position * 100, 0,0), 0.2f);
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
