using SkillIssue.CharacterSpace;
using UnityEngine;

public class ScreenLimit : MonoBehaviour
{
    [SerializeField]
    private int screenEdgeFaceDir;
    [SerializeField] 
    private bool useRenderer;
    [SerializeField]
    private SpriteRenderer screenEdgeRenderer;

    private void FixedUpdate()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character collisionChar = collision.GetComponent<Character>();
        if (collisionChar == null)
            return;
        if (collisionChar.GetMovementDirectionX() != 0 && Managers.Instance.GameManager.GetCornerChar() == null)
            return;
        if (Managers.Instance.GameManager.GetCornerChar() == null)
            Managers.Instance.GameManager.SetCornerChar(collisionChar);
        collisionChar.SetIsAgainstTheWall(true, GetScreenDir());        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (useRenderer)
        //    return;
        Character collisionChar = collision.GetComponent<Character>();
        if (collisionChar == null) return;
        if (Managers.Instance.GameManager.GetCornerChar() != collisionChar)
            collisionChar.SetIsAgainstTheWall(true, GetScreenDir());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Character collisionChar = collision.GetComponent<Character>();
        if (collisionChar == null)
            return;

        if (Managers.Instance.GameManager.GetCornerChar() == collisionChar)
            Managers.Instance.GameManager.SetCornerChar(null);
        collisionChar.SetIsAgainstTheWall(false, GetScreenDir());
    }

}
