using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SkillIssue
{
    public enum ColliderState
    {
        Closed,
        Open,
        Colliding
    }
    public class Hitbox : MonoBehaviour
    {
        public LayerMask mask;
        public bool useSphere = false;
        public Vector3 hitboxSize;
        public Color inactiveColor;
        public Color collisionOpenColor;
        public Color collidingColor;

        public ColliderState state;
        private IHitboxResponder responder = null;

        void FixedUpdate()
        {
            CheckCollision();
        }
        void CheckCollision()
        {
            if (state == ColliderState.Closed) { return; }

            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, hitboxSize, 0, mask);
            if (colliders.Length!= 0)
            {
                if (state == ColliderState.Colliding)
                    return;

                for (int i = 0; i < colliders.Length; i++)
                {
                    Collider2D aCollider = colliders[i];
                    Hurtbox collidedbox = aCollider.GetComponent<Hurtbox>();
                    if (collidedbox?.state == ColliderState.Open)
                    {
                        if (collidedbox?.blockCheck == false)
                            state = ColliderState.Colliding;
                        responder.CollisionedWith(aCollider);
                        return;
                    }

                }                  
               
            }
            else
            {
                state = ColliderState.Open;
            }

            

        }
        void OnDrawGizmosSelected()
        {
            CheckGizmoColor();
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2)); // Because size is halfExtents
        }
        void CheckGizmoColor()
        {
            switch (state)
            {
                case ColliderState.Closed:
                    Gizmos.color = inactiveColor;
                    break;
                case ColliderState.Open:
                    Gizmos.color = collisionOpenColor;
                    break;
                case ColliderState.Colliding:
                    Gizmos.color = collidingColor;
                    break;
            }
        }
        public void StartCheckingCollision()
        {
            state = ColliderState.Open;
        }

        public void StopCheckingCollision()
        {
            state = ColliderState.Closed;
        }
        public void setResponder(IHitboxResponder hitboxResponder)
        {
            responder = hitboxResponder;
        }
    }
}
