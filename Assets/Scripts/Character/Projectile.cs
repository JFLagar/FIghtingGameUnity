using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue;
using SkillIssue.CharacterSpace;
using NUnit.Framework.Constraints;
using System.Net.NetworkInformation;

public class Projectile : MonoBehaviour , IHitboxResponder
{
    Character parent;
    [SerializeField]
    SpriteRenderer m_renderer;
    [SerializeField]
    Transform collisions;
    ProjectileData projectileData;
    [SerializeField]
    Hitbox hitbox;
    private AttackData currentAttack;
    private Animator animator;
    Vector2 trajectory;
    // Start is called before the first frame update
    public void Initialize(Character character, ProjectileData data)
    {
        parent = character;
        float xOrigin = transform.position.x + (parent.FaceDir/2);
        transform.position = new Vector2(xOrigin, 0);
        animator = GetComponent<Animator>();
        projectileData = data;

        trajectory = data.GetTrajectory();
        trajectory.x = trajectory.x * parent.FaceDir;

        m_renderer = GetComponent<SpriteRenderer>();
        m_renderer.sprite = data.GetSprite();

        hitbox.targetMask = parent.GetHitboxTargetMask();
        hitbox.gameObject.layer = parent.GetHitboxLayerMask();

        if (trajectory.x == -1)
        {
            m_renderer.flipX = true;
            //collisions.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            m_renderer.flipX = false;
            //collisions.eulerAngles = new Vector3(0, 0, 0);
        }
        if (projectileData.GetDuration() > 0)
            StartCoroutine(EndingCoroutine());
        hitbox.SetResponder(this);
        gameObject.transform.parent = null; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(trajectory * projectileData.GetSpeed() * Time.fixedDeltaTime);
        if (!m_renderer.isVisible)
        {
            trajectory = Vector2.zero;
            Destroy(this.gameObject,0.5f);
        }
    }

    public void BoxCollisionedWith(Collider2D collider)
    {
        if (currentAttack != projectileData.GetAttackData())
        {
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox?.GetHitBy(projectileData.GetAttackData());
            if (hurtbox.blockCheck)
                return;
            trajectory = Vector2.zero;
            if (parent != null)
                parent.HitConnect(projectileData.GetAttackData());
            Destroy(this.gameObject, 0.5f);
            currentAttack = projectileData.GetAttackData();
        }
    }

    IEnumerator EndingCoroutine()
    {
        yield return null;
        int i = 0;
        while (i != projectileData.GetDuration())
        {
            i++;
            yield return null;
        }
        Destroy(this.gameObject, 0f);
    }

}
