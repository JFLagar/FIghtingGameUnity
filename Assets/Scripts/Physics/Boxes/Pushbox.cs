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
    public bool pushcheck;
    void FixedUpdate()
    {
        CheckCollision();
    }
    private void Update()
    {
        if (state == ColliderState.Colliding)
        {
            character.CharacterPush(1, -(int)character.GetFaceDir());
        }
    }

    public bool IsColliding()
    {
        return state == ColliderState.Colliding;
    }

    void CheckCollision()
    {

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, hitboxSize / 2, 0, (mask));
        if (colliders.Length <= 1)
        {
            if (colliders[0] == null || colliders[0].gameObject.layer != LayerMask.NameToLayer("Ground"))
                character.SetIsAgainstTheWall(false, 0);

            state = ColliderState.Open;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer != LayerMask.NameToLayer("Pushbox"))
            {
                HandleScreenCollisioon(colliders[i]);
            }
            if (colliders[i] != m_collider)
            {
                Collider2D aCollider = colliders[i];
                responder?.CollisionedWith(aCollider);
            }
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

    }

    void HandleScreenCollisioon(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            character.SetIsGrounded(true);
        else
        {
            character.SetIsAgainstTheWall(true, collision.GetComponent<ScreenLimit>().GetScreenDir());
        }
    }

}
