using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;

namespace SkillIssue
{
    public class Hurtbox : MonoBehaviour
    {
        public Character character;
        public ColliderState state = ColliderState.Open;
        public BoxCollider2D boxCollider;
        public Color inactiveColor;
        public Color collisionOpenColor;
        public Color collidingColor;
        public bool blockCheck = false;
        public bool projectile = false;

        public void GetHitBy(AttackData data)
        {
            if(projectile)
            {
                Destroy(this, 0.5f);
                return;
            }
            if (state == ColliderState.Closed)
            {
                return;
            }
            else
            {

                if (!blockCheck)
                {
                    character.HurtboxOnCollision(data);
                }     
                else
                {
                    character.HurtboxOnCollision(data, true);
                    Debug.Log("Blocking Zone");
                }
                
            }
        }

        void OnDrawGizmosSelected()
        {
            CheckGizmoColor();
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(boxCollider.size.x, boxCollider.size.y, 1)); // Because size is halfExtents
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

    }
}
