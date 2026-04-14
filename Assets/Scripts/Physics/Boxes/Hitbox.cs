using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace SkillIssue
{
    public enum ColliderState
    {
        Open,
        Closed,
        Colliding
    }
    public class Hitbox : MonoBehaviour
    {
        public LayerMask targetMask;
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
            //Ignore contactOffset
            Vector3 adjustedSize = hitboxSize - (Vector3.one * Physics2D.defaultContactOffset * 2);
            adjustedSize = Vector3.Max(adjustedSize, Vector3.zero);

            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, adjustedSize, 0, targetMask);
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
                        {
                            SetState(ColliderState.Colliding);
                        }
                        responder?.BoxCollisionedWith(aCollider);
                        return;
                    }
                }                               
            }
            else
            {
                SetState(ColliderState.Open);
            }
        }

        void OnDrawGizmosSelected()
        {
            CheckGizmoColor();

            Gizmos.DrawWireCube(transform.position, hitboxSize); // Because size is halfExtents
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

        public void SetState(ColliderState state)
        {
            this.state = state;
        }

        public void SetResponder(IHitboxResponder hitboxResponder)
        {
            responder = hitboxResponder;
        }


        public void SetSize(Vector3 size)
        {
            hitboxSize = size;
        }

        public void SetPosition(Vector3 position)
        {
            this.transform.localPosition = position;
        }
    }
}
