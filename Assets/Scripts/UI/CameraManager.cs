using SkillIssue.CharacterSpace;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    Character[] characters;
    [SerializeField] Vector3 pos = new Vector3(0, 0, -10);
    [SerializeField] float minZoom = 1.3f;
    [SerializeField] float maxZoom = 1.5f;
    [SerializeField] float edgePadding = 0.5f;
    [SerializeField] bool isOnScreenEdge = false;
    [SerializeField] int screenEdgeFaceDir = 0;
    private float GetDistanceX =>  Mathf.Abs(characters[0].transform.position.x - characters[1].transform.position.x);
    private float visibleRightEdgeLimit;
    private float visibleLeftEdgeLimit;
    Vector3 newPos;

    private void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    void LateUpdate()
    {
        float middle = GetCameraMiddleX();
        float distance = GetCharacterDistanceX();
        HandleCameraPosition(middle);
        HandleCameraZoom(distance, middle);
    }

    private void HandleCameraPosition(float middle)
    {
        if (IsMovingAwayFromScreenEdge(middle) || !isOnScreenEdge)
        {
            pos.x = middle;
        }
        if (GetCameraMiddleY() > 0.5)
        {           
            pos.y = GetCameraMiddleY();
        }
        else
            pos.y = 0;

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
            cam.transform.DOMove(pos, 0.5f);
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

    // Helper methods
    private float GetCharacterDistanceX()
    {
        return Mathf.Abs(characters[0].transform.position.x - characters[1].transform.position.x);
    }

    private float GetCameraMiddleX()
    {
        return (characters[0].transform.position.x + characters[1].transform.position.x) / 2;
    }

    private float GetCharacterDistanceY()
    {
        return Mathf.Abs(characters[0].transform.position.y - characters[1].transform.position.y);
    }

    private float GetCameraMiddleY()
    {
        return (characters[0].transform.position.y + characters[1].transform.position.y) / 2;
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
