using SkillIssue.CharacterSpace;
using UnityEngine;

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

    private float lockedEdgePosition;
    Vector3 newPos;

    private void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    void LateUpdate()
    {
        float middle = GetCameraMiddle();
        float distance = GetCharacterDistance();

        HandleCameraPosition(middle);
        HandleCameraZoom(distance, middle);
    }

    private void HandleCameraPosition(float middle)
    {
        if (IsMovingAwayFromScreenEdge(middle) || !isOnScreenEdge)
        {
            pos.x = middle;
        }

        float halfWidth = cam.orthographicSize * cam.aspect;

        // If clamped near a wall
        if (isOnScreenEdge)
        {
            if (screenEdgeFaceDir == -1) // Wall on the left
            {
                // Prevent camera from going left beyond the lockedEdgePosition
                pos.x = Mathf.Max(pos.x, lockedEdgePosition + halfWidth - edgePadding);
            }
            else if (screenEdgeFaceDir == 1) // Wall on the right
            {
                // Prevent camera from going right beyond the lockedEdgePosition
                pos.x = Mathf.Min(pos.x, lockedEdgePosition - halfWidth + edgePadding);
            }
        }

        cam.transform.position = pos;
    }

    private void HandleCameraZoom(float distance, float middle)
    {
        float targetSize = Mathf.Clamp(distance / 2, minZoom, maxZoom);
        cam.orthographicSize = targetSize;

        // Optionally re-clamp camera again after zoom changed visible area
        //if (isOnScreenEdge)
        //{
        //    float halfWidth = cam.orthographicSize * cam.aspect;

        //    if (screenEdgeFaceDir == -1)
        //    {
        //        pos.x = Mathf.Max(pos.x, lockedEdgePosition + halfWidth - edgePadding);
        //    }
        //    else if (screenEdgeFaceDir == 1)
        //    {
        //        pos.x = Mathf.Min(pos.x, lockedEdgePosition - halfWidth + edgePadding);
        //    }

        //    cam.transform.position = pos;
        //}
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
    private float GetCharacterDistance()
    {
        return Mathf.Abs(characters[0].transform.position.x - characters[1].transform.position.x);
    }

    private float GetCameraMiddle()
    {
        return (characters[0].transform.position.x + characters[1].transform.position.x) / 2;
    }

    // External controls (keep your existing interface)
    public void SetWallDirection(int faceDirection)
    {
        isOnScreenEdge = true;
        screenEdgeFaceDir = faceDirection;

        lockedEdgePosition = GetCameraMiddle();
    }

}
