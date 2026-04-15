using SkillIssue.CharacterSpace;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    Player[] Players;
    [SerializeField]
    Vector3 pos = new Vector3(0, 0, -10);
    [SerializeField]
    float minZoom = 1.3f;
    [SerializeField]
    float maxZoom = 1.5f;

    [SerializeField]
    float verticalMinimumHeight = 0.5f;
    [SerializeField]
    float cameraMovementDuration = 0.5f;
    CinemachineCamera activeCamera;
    private float GetDistanceX => Mathf.Abs(Players[0].transform.position.x - Players[1].transform.position.x);
    private float GetCameraMiddleX => (Players[0].transform.position.x + Players[1].transform.position.x) / 2;
    private float GetDistanceY => Mathf.Abs(Players[0].transform.position.y - Players[1].transform.position.y);
    private float GetCameraMiddleY => (Players[0].transform.position.y + Players[1].transform.position.y) / 2;
    [SerializeField]
    private float cameraOriginY = 0;

    private void Start()
    {
        Players = Managers.Instance.GameManager.GetPlayers();
        activeCamera = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera as CinemachineCamera;
        activeCamera.GetComponent<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
    }

    void LateUpdate()
    {
        if (activeCamera == null) return;
        float middle = GetCameraMiddleX;
        float distance = GetDistanceX;
        HandleCameraZoom(distance, middle);
        HandleCameraPosition(middle);
    }

    private void HandleCameraPosition(float middle)
    {

        pos.x = middle;
        if (GetCameraMiddleY > verticalMinimumHeight)
        {
            pos.y = GetCameraMiddleY;
        }
        else
            pos.y = cameraOriginY;

        if (activeCamera.transform.position != pos)
            activeCamera.transform.DOMove(pos, cameraMovementDuration);
    }

    private void HandleCameraZoom(float distance, float middle)
    {
        float targetSize = Mathf.Clamp(distance / 2, minZoom, maxZoom);
        activeCamera.Lens.OrthographicSize = targetSize;
    }

}
