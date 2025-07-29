using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue;
using SkillIssue.CharacterSpace;

public class Projectile : MonoBehaviour , IHitboxResponder
{
    public Character parent;
    public int hitPoints;
    public SpriteRenderer m_renderer;
    public AttackData data;
    public Vector2 origin;
    public Vector2 trajectory;
    public Hitbox hitbox;
    public Hurtbox m_hurtbox;
    public float speed;
    public Animator animator;
    private AttackData currentAttack;
    public Transform collisions;
    public bool ending;
    public int endingTime;
    public Projectile followUpProjectile;
    // Start is called before the first frame update
    void Start()
    {
        if (trajectory.x == -1)
        {
            m_renderer.flipX = true;
            collisions.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            m_renderer.flipX = false;
            collisions.eulerAngles = new Vector3(0, 0, 0);
        }
        if (ending)
        StartCoroutine(EndingCoroutine());
        hitbox.SetResponder(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(trajectory * speed * Time.deltaTime);
        if (!m_renderer.isVisible)
        {
            trajectory = Vector2.zero;
            Destroy(this.gameObject,0.5f);
        }
    }
    public void BoxCollisionedWith(Collider2D collider)
    {
        if(currentAttack != data)
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox?.GetHitBy(data);
            if (hurtbox.blockCheck)
                return;
            trajectory = Vector2.zero;
            animator.SetTrigger("Collided");
            if (followUpProjectile != null)
                SpawnProjectile();
            if (parent != null)
            parent.HitConnect(data);
            Destroy(this.gameObject, 0.5f);
            currentAttack = data;
        }
      
    }
    public void SpawnProjectile()
    {
        Projectile m_projectile = Instantiate(followUpProjectile, transform);
        m_projectile.trajectory.x = 0;
        m_projectile.origin.x = 0;
        m_projectile.transform.position = new Vector2(transform.position.x , transform.position.y);
        m_projectile.transform.parent = transform.parent;
        m_projectile.hitbox.targetMask = gameObject.layer;
        m_projectile.m_hurtbox.gameObject.layer = m_hurtbox.gameObject.layer;
    }
    IEnumerator EndingCoroutine()
    {
        int i = 0;
        while (i != endingTime)
        {
            i++;
            yield return null;
        }
        Destroy(this.gameObject, 0f);
    }

}
