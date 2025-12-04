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
        Player collisionPlayer = collision.GetComponent<Player>();
        if (collisionPlayer == null)
            return;
        if (collisionPlayer.MovementDirectionX != 0 && Managers.Instance.GameManager.CornerPlayer == null)
            return;
        if (Managers.Instance.GameManager.CornerPlayer == null && collisionPlayer.GetCurrentActionState() == SkillIssue.StateMachineSpace.ActionStates.None)
            Managers.Instance.GameManager.SetCornerChar(collisionPlayer);
        collisionPlayer.SetIsAgainstTheWall(true, screenEdgeFaceDir);        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (useRenderer)
        //    return;
        Player collisionPlayer = collision.GetComponent<Player>();
        if (collisionPlayer == null) return;
        if (Managers.Instance.GameManager.CornerPlayer != collisionPlayer)
            collisionPlayer.SetIsAgainstTheWall(true, screenEdgeFaceDir);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player collisionPlayer = collision.GetComponent<Player>();
        if (collisionPlayer == null)
            return;

        if (Managers.Instance.GameManager.CornerPlayer == collisionPlayer)
            Managers.Instance.GameManager.SetCornerChar(null);
        collisionPlayer.SetIsAgainstTheWall(false, screenEdgeFaceDir);
    }

}
