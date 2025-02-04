using UnityEngine;

public class ScreenLimit : MonoBehaviour
{
    [SerializeField]
    private int screenEdgeFaceDir;
    [SerializeField] 
    private bool useRenderer;
    [SerializeField]
    private SpriteRenderer screenEdgeRenderer;


    private void Update()
    {
        if (!useRenderer)
            return;
        if (screenEdgeRenderer.isVisible)
        {
            Managers.Instance.CameraManager.SetWallDirection(screenEdgeFaceDir);
        }
    }

    public int GetScreenDir()
    {
        return screenEdgeFaceDir;
    }

}
