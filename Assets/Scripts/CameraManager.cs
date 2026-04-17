using SkillIssue.CharacterSpace;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using Unity.Cinemachine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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
    [SerializeField]
    CinemachineCamera gameCamera;
    [SerializeField]
    CinemachineCamera cinematicCamera;
    private float GetDistanceX => Mathf.Abs(Players[0].transform.position.x - Players[1].transform.position.x);
    private float GetCameraMiddleX => (Players[0].transform.position.x + Players[1].transform.position.x) / 2;
    private float GetDistanceY => Mathf.Abs(Players[0].transform.position.y - Players[1].transform.position.y);
    private float GetCameraMiddleY => (Players[0].transform.position.y + Players[1].transform.position.y) / 2;
    [SerializeField]
    private float cameraOriginY = 0;

    bool isZoomfixed = false;
    CinemachineConfiner2D confiner;
    private void Start()
    {
        Players = Managers.Instance.GameManager.GetPlayers();
        gameCamera = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None).First(x => x.Priority >= 20);
        confiner = gameCamera.GetComponent<CinemachineConfiner2D>();
        cinematicCamera = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None).First(x => x.Priority < 20);
    }

    void LateUpdate()
    {
        if (gameCamera == null) return;
        float middle = GetCameraMiddleX;
        float distance = GetDistanceX;
        HandleCameraZoom(distance, middle);
        HandleCameraPosition(middle);

        if (!isZoomfixed && gameCamera != null && confiner != null)
        {
                confiner.InvalidateBoundingShapeCache();
                isZoomfixed = true;
        }
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

        if (gameCamera.transform.position != pos)
            gameCamera.transform.DOMove(pos, cameraMovementDuration);
    }

    private void HandleCameraZoom(float distance, float middle)
    {
        float targetSize = Mathf.Clamp(distance / 2, minZoom, maxZoom);
        gameCamera.Lens.OrthographicSize = targetSize;
    }

    public void SwitchCamera(bool isCinematic, Player trackingPlayer)
    {
        CameraTarget target = new CameraTarget();
        target.TrackingTarget = trackingPlayer.gameObject.transform;
        cinematicCamera.Target = target;
        Debug.Log(isCinematic, target.TrackingTarget);
        gameCamera.Priority = isCinematic ? 0 : 20; 
    }

    [Button]
    public void SwitchCamera()
    {
        CameraTarget target = new CameraTarget();
        target.TrackingTarget = Players[0].gameObject.transform;
        cinematicCamera.Target = target;

        gameCamera.Priority = gameCamera.Priority == 20 ? 0 : 20;
    }
}
