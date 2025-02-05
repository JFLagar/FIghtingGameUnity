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
            character.SetIsAgainstTheWall(false, 0);
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
        if (pushbox.character.GetIsAgainstTheWall() && character.GetMovementDirectionX() == pushbox.character.GetWallDirectionX())
        {
            return;
        }
        // Pushcharacter away from wall
        if (character.GetIsAgainstTheWall())
        {
            pushbox.character.SetIsAgainstTheWall(true, character.GetWallDirectionX());
            // If the other char is on the air
            if (!pushbox.character.GetIsGrounded())
                pushbox.character.transform.position = new Vector2(pushbox.character.transform.position.x + 0.08f * -character.GetWallDirectionX(), pushbox.character.transform.position.y);
        }
        if (character.GetMovementDirectionX() != 0 && character.GetMovementDirectionX() == character.GetFaceDir())
        {
            if (character.GetApplyGravity())
            {
                pushbox.character.CharacterPush(-pushbox.character.GetFaceDir() / 2 * push * Time.deltaTime, (int)character.GetFaceDir());
            }
            else
            {
                pushbox.character.CharacterPush(character.GetFaceDir() / 2 * push * Time.deltaTime, (int)character.GetFaceDir());
            }
        }
        else
        {
            pushbox.character.CharacterPush(0, (int)character.GetFaceDir());
            if (pushbox.character.GetMovementDirectionX() == 0 && !pushbox.character.GetIsAgainstTheWall())
                pushbox.character.transform.position = new Vector2(pushbox.character.transform.position.x + 0.08f * -pushbox.character.GetFaceDir(), pushbox.character.transform.position.y);
        }
    }

    void HandleCollision(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            character.SetIsGrounded(true);
        else
        {
            character.SetIsAgainstTheWall(true, collision.GetComponent<ScreenLimit>().GetScreenDir());
        }
    }

}
