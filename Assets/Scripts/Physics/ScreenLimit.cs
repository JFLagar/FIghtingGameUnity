using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;

public class ScreenLimit : MonoBehaviour
{
    public bool isWall;
    public int x;
    public Collider2D m_collider;
    public LayerMask mask;
    public Vector3 hitboxSize;
    public Color color;
    public bool cameraWall = false;
    private Pushbox previousCollision;
    public CameraManager cam;
    public Collider2D[] colliders;
    public Collider2D[] previousColliders;
    // Update is called once per frame
    void FixedUpdate()
    {
        CheckCollisions();
    }
    private void Update()
    {
        if (!isWall)
            return;
      if(GetComponent<Renderer>().isVisible)
        {
            if(x < 0)
            {
                cam.check = true;

            }
            else
            {
                cam.check2 = true;
            }
        }
      else
        {
            if (x < 0)
            {
                cam.check = false;
            }
            else
            {
                cam.check2 = false;
            }
        }
    }

    private void CheckCollisions()
    {
        //if (previousColliders.Length != 0)
        //{
        //    for (int i = 0; i < previousColliders.Length; i++)
        //    {
        //        previousColliders[i].GetComponent<Pushbox>()?.character.IsGrounded(false);
        //    }
        //}
        colliders = Physics2D.OverlapBoxAll(m_collider.bounds.center, m_collider.bounds.size, 0, (mask));
       
        if(colliders.Length > 1)
        previousColliders = colliders;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D aCollider = colliders[i];
            Pushbox collidedbox = colliders[i].GetComponent<Pushbox>();
            if (!cameraWall)
            {
                if (!isWall)
                {
                    bool applyGravity = (bool)(collidedbox?.character.GetGravity());
                    if(applyGravity)
                    collidedbox?.character.IsGrounded(true); 
                }
                else
                {
                    collidedbox?.character.SetWall(true, x);
                }
            }
        }
  
      
    }
    void OnDrawGizmoSelected()
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2)); // Because size is halfExtents
    }

}
