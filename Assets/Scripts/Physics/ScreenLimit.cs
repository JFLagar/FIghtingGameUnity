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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character collisionChar = collision.GetComponent<Character>();
        if (collisionChar == null)
            return;
        if (collisionChar.MovementDirectionX != 0 && Managers.Instance.GameManager.CornerCharacter == null)
            return;
        if (Managers.Instance.GameManager.CornerCharacter == null && collisionChar.GetCurrentActionState() == SkillIssue.StateMachineSpace.ActionStates.None)
            Managers.Instance.GameManager.SetCornerChar(collisionChar);
        collisionChar.SetIsAgainstTheWall(true, screenEdgeFaceDir);        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (useRenderer)
        //    return;
        Character collisionChar = collision.GetComponent<Character>();
        if (collisionChar == null) return;
        if (Managers.Instance.GameManager.CornerCharacter != collisionChar)
            collisionChar.SetIsAgainstTheWall(true, screenEdgeFaceDir);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Character collisionChar = collision.GetComponent<Character>();
        if (collisionChar == null)
            return;

        if (Managers.Instance.GameManager.CornerCharacter == collisionChar)
            Managers.Instance.GameManager.SetCornerChar(null);
        collisionChar.SetIsAgainstTheWall(false, screenEdgeFaceDir);
    }

}
