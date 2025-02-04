using SkillIssue;
using UnityEngine;

public enum PushType
{
    Character,
    Ground,
    Wall
}
public class Pushbox : MonoBehaviour
{
    public Collider2D m_collider;
    public LayerMask mask;
    public bool useSphere = false;
    public Vector3 hitboxSize;
    public Color color;
    public ColliderState state;

    private IHitboxResponder responder = null;
    public SkillIssue.CharacterSpace.Character character = null;
    public float push = 60;

    void FixedUpdate()
    {
        CheckCollision();
        if (state == ColliderState.Colliding)
        {
            character.CharacterPush(0);
        }
    }

    void CheckCollision()
    {

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, hitboxSize / 2, 0, (mask));
        if (colliders.Length <= 1)
        {
            state = ColliderState.Open;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != m_collider)
            {
                Collider2D aCollider = colliders[i];
                responder?.CollisionedWith(aCollider);
            }
            if (colliders[i].gameObject.layer != LayerMask.NameToLayer("Pushbox"))
            {
                HandleCollision(colliders[i]);
            }
        }
        if (colliders.Length <= 1 && colliders[0].gameObject.layer != LayerMask.NameToLayer("Ground"))
        {
            character.isAgainstTheWall = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2)); // Because size is halfExtents
    }

    public void SetResponder(IHitboxResponder hitboxResponder)
    {
        responder = hitboxResponder;
    }

    public void HandleCollision(Pushbox pushbox)
    {

        state = ColliderState.Colliding;
        if (pushbox.character.isAgainstTheWall && character.x == pushbox.character.wallFaceDirection)
        {
            return;
        }
        if (character.isAgainstTheWall)
        {
            pushbox.character.isAgainstTheWall = true;
            pushbox.character.wallFaceDirection = character.wallFaceDirection;
            if (!pushbox.character.isGrounded && pushbox.character.faceDir == pushbox.character.wallFaceDirection)
                pushbox.character.transform.position = new Vector2(pushbox.character.transform.position.x + 0.08f * -pushbox.character.faceDir, pushbox.character.transform.position.y);
        }
        if (character.x != 0 && character.x == character.faceDir)
        {
            if (character.applyGravity)
            {
                pushbox.character.CharacterPush(-pushbox.character.faceDir / 2 * push * Time.deltaTime);
            }
            else
            {
                pushbox.character.CharacterPush(character.x / 2 * push * Time.deltaTime);
            }
        }
        else
        {
            pushbox.character.CharacterPush(0);
            if (pushbox.character.x == 0 && !pushbox.character.isAgainstTheWall)
                pushbox.character.transform.position = new Vector2(pushbox.character.transform.position.x + 0.08f * -pushbox.character.faceDir, pushbox.character.transform.position.y);
        }
    }

    void HandleCollision(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            character.SetIsGrounded(true);
        else
        {
            character.isAgainstTheWall = true;
            character.wallFaceDirection = collision.GetComponent<ScreenLimit>().GetScreenDir();
        }
    }

    private void OnCollision2DExit(Collision2D collision)
    {
        Debug.Log("CollisionExit");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("Wall left");
            character.isAgainstTheWall = false;
        }
    }
}
