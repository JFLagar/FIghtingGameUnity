using SkillIssue.CharacterSpace;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    Player[] Players;
    [SerializeField]
    Vector3 pos = new Vector3(0, 0, -10);
    [SerializeField]
    float minZoom = 1.3f;
    [SerializeField]
    float maxZoom = 1.5f;
    [SerializeField]
    float edgePadding = 0.5f;
    [SerializeField]
    float verticalMinimumHeight = 0.5f;
    [SerializeField]
    float cameraMovementDuration = 0.5f;
    [SerializeField]
    bool isOnScreenEdge = false;
    [SerializeField]
    int screenEdgeFaceDir = 0;

    private float visibleRightEdgeLimit;
    private float visibleLeftEdgeLimit;

    private float GetDistanceX => Mathf.Abs(Players[0].transform.position.x - Players[1].transform.position.x);
    private float GetCameraMiddleX => (Players[0].transform.position.x + Players[1].transform.position.x) / 2;
    private float GetDistanceY => Mathf.Abs(Players[0].transform.position.y - Players[1].transform.position.y);
    private float GetCameraMiddleY => (Players[0].transform.position.y + Players[1].transform.position.y) / 2;

    private float cameraOriginY = 0;

    private void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
        cameraOriginY = cam.gameObject.transform.position.y;
    }

    void LateUpdate()
    {
        float middle = GetCameraMiddleX;
        float distance = GetDistanceX;
        HandleCameraPosition(middle);
        HandleCameraZoom(distance, middle);
    }

    private void HandleCameraPosition(float middle)
    {
        if (IsMovingAwayFromScreenEdge(middle) || !isOnScreenEdge)
        {
            pos.x = middle;
        }
        if (GetCameraMiddleY > verticalMinimumHeight)
        {
            pos.y = GetCameraMiddleY;
        }
        else
            pos.y = cameraOriginY;

        if (isOnScreenEdge)
        {
            if (screenEdgeFaceDir == -1)
            {
                pos.x = visibleLeftEdgeLimit + ((cam.orthographicSize - minZoom) * (maxZoom + (maxZoom - minZoom)));
            }
            else if (screenEdgeFaceDir == 1)
            {
                pos.x = visibleRightEdgeLimit - ((cam.orthographicSize - minZoom) * (maxZoom + (maxZoom - minZoom)));
            }
        }
        if (cam.transform.position != pos)
            cam.transform.DOMove(pos, cameraMovementDuration);
    }

    private void HandleCameraZoom(float distance, float middle)
    {
        float targetSize = Mathf.Clamp(distance / 2, minZoom, maxZoom);
        cam.orthographicSize = targetSize;
    }

    private bool IsMovingAwayFromScreenEdge(float middle)
    {
        if (screenEdgeFaceDir == -1 && middle > pos.x)
        {
            isOnScreenEdge = false;
            return true;
        }
        if (screenEdgeFaceDir == 1 && middle < pos.x)
        {
            isOnScreenEdge = false;
            return true;
        }
        return false;
    }

    // External controls (keep your existing interface)
    public void SetWallDirection(int faceDirection)
    {
        isOnScreenEdge = true;
        screenEdgeFaceDir = faceDirection;
        float halfWidth = maxZoom * cam.aspect; // world space half-width at max zoom
        float currentX = cam.transform.position.x;
        if (faceDirection == -1 && visibleLeftEdgeLimit == 0) // wall on left
        {
            visibleLeftEdgeLimit = (currentX - halfWidth)/2;
        }
        else if (faceDirection == 1 && visibleRightEdgeLimit == 0) // wall on right
        {
            visibleRightEdgeLimit = (currentX + halfWidth)/2;
        }
    }


}
