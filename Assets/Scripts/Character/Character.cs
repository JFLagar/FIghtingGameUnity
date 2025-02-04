using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SkillIssue.CharacterSpace
{
    public enum Element
    {
        Fire = 0,
        Water = 1,
        Wind = 2,
        Earth = 3
    }
    public class Character : MonoBehaviour, IPhysics, IHitboxResponder
    {
        public bool isPlayer2;
        public Character oponent;
        public float faceDir;
        public float xDiff;
        public SpriteRenderer render;
        public SpriteRenderer vfx;
        public SpriteRenderer screenCheck;
        public StateMachine stateMachine;
        public InputHandler inputHandler;

        [Space]

        public Element element;
        public AttackData grab;
        public AttackData[] standingAttacks;
        public AttackData[] crouchingAttacks;
        public AttackData[] jumpAttacks;
        public AttackData[] fireAttacks, waterAttacks, windAttacks, earthAttacks;
        private AttackData[] specialAttacks;
        public AttackClass attack;

        [Space]

        public Animator animator;
        public CharacterAnimationManagar characterAnimation;
        public Pushbox pushbox;
        public bool applyGravity = false;
        public bool wall;
        public bool cameraWall = false;
        public float wallx;
        public bool isGrounded;
        public float x;
        public float y;
        public ActionStates currentAction;
        public States currentState;
        [Space]

        public int maxHealth;
        public int currentHealth;
        public int comboHit;

        [Space]

        public float movementspeed;
        public float jumpPower;
        public float forceSpeed;
        public float forceLeftOver;
        bool landed;
        bool landingRecovery;
        public bool isJumping;
        [Space]

        public Transform collisions;
        private Coroutine currentHitCoroutine;
        private Coroutine currentMovementCoroutine;
        public AttackData storedAttack = null;
        public List<AttackData> currentCombo = new List<AttackData>();
        public bool visualState;
        public Color32[] stateColors;
        public Hitbox m_hitbox;
        public Hurtbox m_hurtbox;
        private Transform origin;
        private Projectile currentProjectile;
        private int playerId = 0;

        private void Awake()
        {
            inputHandler.character = this;
            stateMachine.character = this;

        }

        // Start is called before the first frame update
        void Start()
        {
            pushbox.SetResponder(this);
            pushbox.character = this;
            origin = transform;
            if (isPlayer2)
                playerId = 1;

        }

        // Update is called once per frame
        void Update()
        {
            switch (element)
            {
                case Element.Fire:
                    specialAttacks = fireAttacks;
                    break;
                case Element.Water:
                    specialAttacks = waterAttacks;
                    break;
                case Element.Wind:
                    specialAttacks = windAttacks;
                    break;
                case Element.Earth:
                    specialAttacks = earthAttacks;
                    break;
            }
            currentAction = stateMachine.currentAction;
            stateMachine.StateMachineUpdate();
            cameraWall = !screenCheck.isVisible;
            if (!screenCheck.isVisible)
            {
                x = 0;
                wallx = -faceDir;
            }
            if (oponent == null)
                return;
            xDiff = transform.position.x - oponent.transform.position.x;
            if (currentState != States.Jumping)
            {
                if (xDiff < 0)
                {
                    faceDir = 1;
                    if (render != null)
                        render.flipX = false;
                    collisions.eulerAngles = new Vector3(0, 0, 0);

                }
                else
                {
                    faceDir = -1;
                    if (render != null) render.flipX = true;
                    collisions.eulerAngles = new Vector3(0, 180, 0);
                }
                vfx.flipX = render.flipX;
            }
            if (currentAction == ActionStates.Hit)
            {
                if (currentProjectile != null)
                {
                    DestroyImmediate(currentProjectile.gameObject);
                }
            }
            //Safety messure against stunlock
            if (currentHitCoroutine == null && currentAction == ActionStates.Hit)
            {
                currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(5));
            }
            if (visualState)
            {
                switch (currentAction)
                {
                    case ActionStates.None:
                        render.color = stateColors[0];
                        break;
                    case ActionStates.Hit:
                        render.color = stateColors[1];
                        break;
                }
            }

        }

        public void PerformAttack(AttackType type)
        {
            if (type == AttackType.Grab)
            {
                attack.Attack(grab);
            }
            //here comes the canceable attack
            if ((int)type != 2)
            {
                switch (stateMachine.currentState)
                {
                    case StandingState:
                        //if (inputHandler.movementInput.direction.x > 0)
                        //{
                        //    Debug.Log(standingAttacks[((int)type + (int)inputHandler.movementInput.direction.x) + 1].ToString());
                        //    return;
                        //}
                        //else
                        {
                            switch (inputHandler.direction.y)
                            {
                                case 0f:
                                    attack.Attack(standingAttacks[((int)type)]);
                                    break;
                                case 1f:
                                    ApplyForce(inputHandler.direction, jumpPower);
                                    attack.Attack(jumpAttacks[((int)type)]);
                                    break;
                                case -1f:
                                    attack.Attack(crouchingAttacks[((int)type)]);
                                    break;
                            }

                        }
                        break;

                    case CrouchState:
                        switch (inputHandler.direction.y)
                        {
                            case 0f:
                                attack.Attack(standingAttacks[((int)type)]);
                                break;
                            case -1f:
                                attack.Attack(crouchingAttacks[((int)type)]);
                                break;
                        }
                        break;
                    case JumpState:
                        attack.Attack(jumpAttacks[((int)type)]);
                        break;
                }
            }
            else
            {
                int id = ((int)(inputHandler.direction.x * faceDir));
                switch (stateMachine.currentState)
                {

                    case StandingState:
                        if (id == 0 && currentProjectile != null)
                            return;
                        attack.Attack(specialAttacks[id + 1]);

                        break;

                    case CrouchState:
                        attack.Attack(specialAttacks[id + 1]);
                        break;
                    case JumpState:
                        break;
                }
            }

        }

        public void SpawnProjectile(Projectile projectile)
        {
            if (currentProjectile != null)
            {
                return;
            }
            Projectile m_projectile = Instantiate(projectile, transform);
            animator.speed = 0;
            m_projectile.trajectory.x = m_projectile.trajectory.x * faceDir;
            m_projectile.transform.position = new Vector2(transform.position.x + (projectile.origin.x * faceDir), transform.position.y + projectile.origin.y);
            m_projectile.transform.parent = transform.parent;
            m_projectile.hitbox.mask = m_hitbox.mask;
            m_projectile.m_hurtbox.gameObject.layer = m_hurtbox.gameObject.layer;
            m_projectile.parent = this;
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(projectile.data.hitstun));
            currentProjectile = m_projectile;
        }

        public void GetHit(AttackData data, bool blockCheck = false)
        {
            if (currentAction == ActionStates.Attack)
            {
                DamageDealt(data);
                return;
            }
            if (data.grab)
            {
                if (currentState == States.Jumping)
                    return;
                DamageDealt(data);
                return;
            }

            if (inputHandler.direction.x == -faceDir && currentAction != ActionStates.Hit && IsBlocking(data.attackAttribute))
            {
                BlockDone(data, blockCheck);
                return;
            }
            //block
            else if (!blockCheck)
            {
                DamageDealt(data);
                return;
            }
        }

        private void DamageDealt(AttackData data)
        {
            Vector2 dir = new Vector2(data.push.x * -faceDir, 0);
            PlaySound(data.collideSound);
            stateMachine.currentAction = ActionStates.Hit;
            if (data.launcher)
            {
                characterAnimation.AddAnimation(AnimType.Hit, "JumpingHit");
                stateMachine.currentState = stateMachine.jumpState;
                dir.y = data.push.y;
            }
            else
            {
                if (data.grab)
                {
                    characterAnimation.AddAnimation(AnimType.Hit, "StandingHit");
                    characterAnimation.animator.SetBool("Crouching", false);
                }
                else
                {
                    characterAnimation.AddAnimation(AnimType.Hit, currentState.ToString() + "Hit");
                }

                if (stateMachine.currentState == stateMachine.jumpState)
                {
                    dir.y = data.push.y;
                }
            }
            if (currentHitCoroutine != null)
                StopCoroutine(currentHitCoroutine);
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.hitstun, data.launcher));
            currentHealth = currentHealth - data.damage;
            Managers.Instance.GameManager.UpdateHealth(playerId, currentHealth);
            if (wall && faceDir != wallx)
            {
                ApplyCounterPush(-dir, 3f);
            }
            else
                ApplyForce(dir, 3f);
        }

        private void BlockDone(AttackData data, bool blockCheck = false)
        {
            PlaySound(data.collideSound);
            Vector2 dir = new Vector2(data.push.x * -faceDir, 0);
            Vector2 blockDir = new Vector2(dir.x, 0);
            stateMachine.currentAction = ActionStates.Block;
            characterAnimation.AddAnimation(AnimType.Hit, currentState.ToString() + "Block");
            if (currentHitCoroutine != null)
                StopCoroutine(currentHitCoroutine);
            if (blockCheck)
                currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.blockstun / 2));
            else
            {
                currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.blockstun));
                if (wall && faceDir != wallx)
                {
                    ApplyCounterPush(-blockDir, 3f);
                }
                else
                    ApplyForce(blockDir, 3f);
            }
        }

        public void ResetPos()
        {
            transform.position = origin.position;
        }

        public void FixPosition()
        {
            if (!landed)
            {
                transform.position = new Vector3(transform.position.x, 0f, 0);
                landed = true;
            }
        }

        public void CharacterMove(Vector2 direction)
        {
            float speed = movementspeed;
            x = direction.x;
            if (stateMachine.currentAction != ActionStates.None || currentState != States.Standing)
                return;

            if (x != 0)
            {
                if (x != faceDir)
                {
                    animator.SetInteger("X", -1);
                    speed = movementspeed / 1.3f;
                }

                else
                {
                    animator.SetInteger("X", 1);
                    speed = movementspeed;
                }
            }
            else
                animator.SetInteger("X", 0);


            if (wall || cameraWall)
            {
                if (direction.x == 0 || direction.x == wallx)

                {
                    transform.Translate(new Vector2(0, 0) * speed * Time.deltaTime);
                }
                else
                {
                    wall = false;
                    transform.Translate(new Vector2(direction.x, 0) * speed * Time.deltaTime);
                }
            }
            else
                transform.Translate(new Vector2(direction.x, 0) * speed * Time.deltaTime);

        }

        public void CharacterPush(float x)
        {
            if (wall && x == wallx || x == 0)
                return;
            transform.Translate(new Vector2(x, 0) * Time.deltaTime);
            wall = false;
        }

        public void ApplyCounterPush(Vector2 direction, float duration)
        {
            oponent.wall = false;
            Vector2 dir = new Vector2(direction.x, 0f);
            oponent.ApplyForce(dir, duration, true);
        }

        public void ApplyForce(Vector2 direction, float duration, bool counterforce = false)
        {
            bool m_bool = false;
            if (counterforce)
            {
                y = 0;
                m_bool = counterforce;
            }
            else
            {
                if (wall && direction.x == wallx || cameraWall && direction.x == wallx)
                    direction.x = 0;
                else
                    wall = false;
                y = direction.y;
                if (y > 0)
                    isGrounded = false;
                applyGravity = false;
            }


            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(ForceCoroutine(direction, duration, m_bool));

        }

        public void ApplyAttackForce(AttackData data)
        {
            Vector2 direction = data.movement;
            float duration = data.movementDuration;
            {
                if (wall && direction.x == wallx || cameraWall && direction.x == wallx)
                    direction.x = 0;
                else
                    wall = false;
                y = direction.y;
                if (y > 0)
                    isGrounded = false;
                applyGravity = false;
                wall = false;
            }

            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(ForceAttackCoroutine(new Vector2(direction.x * faceDir, direction.y), duration, false));

        }

        public void ApllyGravity()
        {
            if (!applyGravity)
                return;
            if (stateMachine.currentAction == ActionStates.None && isJumping)
            {
                characterAnimation.AddAnimation(AnimType.Movement, "JumpFall");
            }
            if (!isGrounded)
                isJumping = false;
            landed = false;

            if ((wall && x == wallx) || (cameraWall && x == wallx))
                transform.Translate(new Vector2(0, -1) * (forceSpeed) * Time.deltaTime);
            else
            {
                transform.Translate(new Vector2(x, -1) * (forceSpeed) * Time.deltaTime);
            }

        }

        public void IsGrounded(bool check)
        {
            isGrounded = check;
        }

        public void CollisionedWith(Collider2D collider)
        {
            if (collider == GetComponent<Collider2D>())
                return;
            Pushbox collidedbox = collider.GetComponent<Pushbox>();
            collidedbox?.HandleCollision(pushbox);

        }

        public void SetWall(bool isWall, int x)
        {
            wall = isWall;
            wallx = x;
        }

        public void CheckState()
        {
            Debug.Log(animator.GetCurrentAnimatorClipInfo(0));
        }

        public void AnimEnd()
        {
            oponent.ResetAttackInfo();
            characterAnimation.ClearAnimations();
            if (stateMachine.currentAction == ActionStates.None)
                return;
            stateMachine.currentAction = ActionStates.None;

        }

        public void OpenHitboxes(int number)
        {
            for (int i = 0; i < number; i++)
            {
                attack.hitboxes[i].state = ColliderState.Open;
            }
        }

        public void HitRecover()
        {
            animator.speed = 1;
            animator.SetTrigger("Recovery");
        }

        public IEnumerator ForceAttackCoroutine(Vector2 direction, float duration, bool counterForce)
        {
            float i = 0f;
            while (i != duration)
            {
                if (!counterForce)
                {
                    if (direction.x != 0 && ((wall && direction.x == wallx) || (cameraWall && direction.x == wallx)))
                        direction.x = 0;
                }
                x = direction.x;
                transform.Translate(direction * forceSpeed * Time.deltaTime);
                yield return null;
                i++;
                forceLeftOver = duration - i;
            }
            currentMovementCoroutine = null;
        }
        public IEnumerator ForceCoroutine(Vector2 direction, float duration, bool counterForce)
        {
            float i = 0f;
            while (i != duration)
            {
                if (!counterForce)
                {
                    if (direction.x != 0 && ((wall && direction.x == wallx) || (cameraWall && direction.x == wallx)))
                        direction.x = 0;
                }
                x = direction.x;
                transform.Translate(direction * forceSpeed * Time.deltaTime);
                yield return null;
                i++;
                forceLeftOver = duration - i;
            }
            currentMovementCoroutine = null;
        }

        public IEnumerator RecoveryFramesCoroutines(int frames, bool knockdown = false)
        {
            int i = 0;
            while (i != frames)
            {
                if (i == frames / 2 && !knockdown)
                    animator.speed = 0;
                yield return null;
                i++;
            }
            HitRecover();
        }

        public bool IsMoving()
        {
            if (currentMovementCoroutine != null)
                return true;
            return false;
        }

        public bool GetGravity()
        {
            return applyGravity;
        }

        public void TestAction(string name)
        {
            stateMachine.currentAction = ActionStates.Landing;
        }

        public void HitboxesEnabled()
        {
            visualState = !visualState;
        }

        public void HitConnect(AttackData data)
        {
            animator.speed = 0;
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.hitstun / 2));
            if (oponent.currentAction == ActionStates.Block)
            {
                currentCombo.Clear();
            }
            else
            {
                currentCombo.Add(data);
                comboHit = currentCombo.Count;
                Managers.Instance.GameManager.UpdateComboCounter(playerId);
            }
        }

        public void ResetAttackInfo()
        {
            currentCombo.Clear();
            storedAttack = null;
            comboHit = currentCombo.Count;
            Managers.Instance.GameManager.UpdateComboCounter(playerId);
        }

        bool IsBlocking(AttackAttribute attack)
        {
            switch (currentState)
            {
                case States.Standing:
                    if (attack == AttackAttribute.Low)
                        return false;
                    else
                        return true;
                case States.Crouching:
                    if (attack == AttackAttribute.High)
                        return false;
                    else
                        return true;
                case States.Jumping:
                    return false;
            }
            return false;
        }

        public void PlaySound(AudioClip clip = null)
        {
            if (clip != null)
                AudioManager.instance.PlayAnimationEffect(clip, playerId);
        }
    }

}
