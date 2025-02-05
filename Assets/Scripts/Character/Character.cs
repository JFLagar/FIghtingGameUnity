using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
        [SerializeField]
        bool isPlayer2;
        [SerializeField]
        Character opponent;
        float faceDir;
        float xDiff;
        [SerializeField]
        SpriteRenderer render;
        [SerializeField]
        SpriteRenderer vfx;

        [Space]

        [SerializeField]
        StateMachine stateMachine;
        [SerializeField]
        InputHandler inputHandler;

        [Space]
        //Turn this into into scriteable object
        [SerializeField]
        CharacterData characterData;

        [Space]

        [SerializeField]
        AttackClass attack;

        [Space]

        [SerializeField]
        Animator animator;
        [SerializeField]
        CharacterAnimationManager characterAnimation;
        [SerializeField]
        Pushbox pushbox;

        bool applyGravity = false;
        bool isAgainstTheWall;
        bool canBePushed = true;
        float wallFaceDirection;
        private bool isGrounded;
        float movementDirectionX;
        float posY;

        [Space]

        int currentHealth;

        [Space]

        [SerializeField]
        float jumpPower;
        [SerializeField]
        float forceSpeed;
        float forceLeftOver;

        bool landed;
        bool isJumping;

        [Space]

        [SerializeField]
        Transform collisions;
        private Coroutine currentHitCoroutine;
        private Coroutine currentMovementCoroutine;
        AttackData storedAttack = null;
        List<AttackData> currentCombo = new();
        [SerializeField]
        bool visualState;
        [SerializeField]
        Color32[] stateColors;
        [SerializeField]
        Hitbox m_hitbox;
        [SerializeField]
        Hurtbox m_hurtbox;
        [SerializeField]
        Transform origin;
        [SerializeField]
        Projectile currentProjectile;
        int playerId = 0;

        private void Awake()
        {
            inputHandler.Initialize(this);
            stateMachine.Initialize(this);

        }

        // Start is called before the first frame update
        void Start()
        {
            pushbox.SetResponder(this);
            pushbox.character = this;
            origin = transform;
            currentHealth = GetMaxHealth();
            if (isPlayer2)
                playerId = 1;
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.StateMachineUpdate();
            if (opponent == null)
                return;
            xDiff = transform.position.x - opponent.transform.position.x;
            if (GetCurrentState() != States.Jumping)
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
            if (GetCurrentActionState() == ActionStates.Hit)
            {
                if (currentProjectile != null)
                {
                    DestroyImmediate(currentProjectile.gameObject);
                }
            }
            //Safety messure against stunlock
            if (currentHitCoroutine == null && GetCurrentActionState() == ActionStates.Hit)
            {
                currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(5));
            }
            if (visualState)
            {
                switch (GetCurrentActionState())
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

        #region Getters and Setters
        public ActionStates GetCurrentActionState()
        {
            return stateMachine.GetActionState();
        }

        public Vector2 GetInputDirection()
        {
            return inputHandler.GetDirection();
        }

        public void SetActionState(ActionStates action)
        {
            stateMachine.SetCurrentActionState(action);
        }

        public States GetCurrentState()
        {
            return stateMachine.GetState();
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public CharacterAnimationManager GetCharacterAnimation()
        {
            return characterAnimation;
        }

        public AttackData GetStoredAttack()
        {
            return storedAttack;
        }

        public void SetStoredAttack(AttackData attack)
        {
            storedAttack = attack;
        }

        public float GetJumpPower()
        {
            return jumpPower;
        }

        public float GetXDiff()
        {
            return xDiff;
        }

        public int GetComboCount()
        {
            return currentCombo.Count;
        }

        public List<AttackData> GetCombo()
        {
            return currentCombo;
        }

        public bool GetApplyGravity()
        {
            return applyGravity;
        }

        public void SetApplyGravity(bool value)
        {
            applyGravity = value;
        }

        public bool GetIsJumping()
        {
            return isJumping;
        }

        public void SetIsJumping(bool value)
        {
            isJumping = value;
        }

        public bool IsStillInMovement()
        {
            if (currentMovementCoroutine != null)
                return true;
            return false;
        }

        public bool GetIsGrounded()
        {
            return isGrounded;
        }

        public void SetIsGrounded(bool value)
        {
            isGrounded = value;
        }

        public bool GetCanBePushed()
        {
            return canBePushed;
        }

        public void SetCanBePushed(bool value)
        {
            canBePushed = value;
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public int GetMaxHealth()
        {
            return characterData.GetMaxHP();
        }

        public Element GetElement()
        {
            return characterData.GetElement();
        }

        public float GetMovementDirectionX()
        {
            return movementDirectionX;
        }

        public float GetWallDirectionX()
        {
            return wallFaceDirection;
        }

        public void SetWallDirectionX(float direction)
        {
            wallFaceDirection = direction;
        }

        public float GetFaceDir()
        {
            return faceDir;
        }

        public bool GetIsAgainstTheWall()
        {
            return isAgainstTheWall;
        }

        public void SetIsAgainstTheWall(bool isAgainstTheWall, float faceDirection)
        {
            this.isAgainstTheWall = isAgainstTheWall;
            wallFaceDirection = faceDirection;
        }

        #endregion
        public void PerformAttack(AttackType type)
        {
            if (type == AttackType.Grab)
            {
                attack.Attack(characterData.GetGrabData());
            }
            //here comes the canceable attack
            if ((int)type != 2)
            {
                switch (GetCurrentState())
                {
                    case States.Standing:
                        //if (inputHandler.movementInput.direction.x > 0)
                        //{
                        //    Debug.Log(standingAttacks[((int)type + (int)inputHandler.movementInput.direction.x) + 1].ToString());
                        //    return;
                        //}
                        //else
                        {
                            switch (inputHandler.GetDirection().y)
                            {
                                case 0f:
                                    attack.Attack(characterData.GetStandingAttacks()[((int)type)]);
                                    break;
                                case 1f:
                                    attack.Attack(characterData.GetJumpAttacks()[((int)type)]);
                                    break;
                                case -1f:
                                    attack.Attack(characterData.GetCrouchingAttacks()[((int)type)]);
                                    break;
                            }

                        }
                        break;

                    case States.Crouching:
                        switch (inputHandler.GetDirection().y)
                        {
                            case 0f:
                                attack.Attack(characterData.GetStandingAttacks()[((int)type)]);
                                break;
                            case -1f:
                                attack.Attack(characterData.GetCrouchingAttacks()[((int)type)]);
                                break;
                        }
                        break;
                    case States.Jumping:
                        attack.Attack(characterData.GetJumpAttacks()[((int)type)]);
                        break;
                }
            }
            else
            {
                int id = ((int)(inputHandler.GetDirection().x * faceDir));
                switch (GetCurrentState())
                {

                    case States.Standing:
                        if (id == 0 && currentProjectile != null)
                            return;
                        attack.Attack(characterData.GetSpecialAttacks()[id + 1]);

                        break;

                    case States.Crouching:
                        attack.Attack(characterData.GetSpecialAttacks()[id + 1]);
                        break;
                    case States.Jumping:
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
            m_projectile.trajectory.x *= faceDir;
            m_projectile.transform.position = new Vector2(transform.position.x + (projectile.origin.x * faceDir), transform.position.y + projectile.origin.y);
            m_projectile.transform.parent = transform.parent;
            m_projectile.hitbox.mask = m_hitbox.mask;
            m_projectile.m_hurtbox.gameObject.layer = m_hurtbox.gameObject.layer;
            m_projectile.parent = this;
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(projectile.data.hitstun));
            currentProjectile = m_projectile;
        }

        public void CharacterGetHit(AttackData data, bool blockCheck = false)
        {
            if (GetCurrentActionState() == ActionStates.Attack)
            {
                DamageDealt(data);
                return;
            }
            if (data.grab)
            {
                if (GetCurrentState() == States.Jumping || GetCurrentActionState() == ActionStates.Hit)
                    return;
                DamageDealt(data);
                return;
            }

            if (inputHandler.GetDirection().x == -faceDir && GetCurrentActionState() != ActionStates.Hit && IsBlocking(data.attackAttribute))
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
            Vector2 dir = new(data.push.x * -faceDir, 0);
            PlaySound(data.collideSound);
            stateMachine.SetCurrentActionState(ActionStates.Hit);
            if (data.launcher)
            {
                characterAnimation.AddAnimation(AnimType.Hit, "JumpingHit");
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
                    characterAnimation.AddAnimation(AnimType.Hit, GetCurrentState().ToString() + "Hit");
                }

                if (GetCurrentState() == States.Jumping)
                {
                    dir.y = data.push.y;
                }
            }
            if (currentHitCoroutine != null)
                StopCoroutine(currentHitCoroutine);
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.hitstun, data.launcher));
            currentHealth -= data.damage;
            Managers.Instance.GameManager.UpdateHealth(playerId, GetCurrentHealth());
            if (GetIsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
            {
                ApplyCounterPush(-dir, 3f);
            }
            else
                ApplyForce(dir, 3f);
        }

        private void BlockDone(AttackData data, bool blockCheck = false)
        {
            PlaySound(data.collideSound);
            Vector2 dir = new(data.push.x * -GetFaceDir(), 0);
            Vector2 blockDir = new(dir.x, 0);
            stateMachine.SetCurrentActionState(ActionStates.Block);
            characterAnimation.AddAnimation(AnimType.Hit, GetCurrentState().ToString() + "Block");
            if (currentHitCoroutine != null)
                StopCoroutine(currentHitCoroutine);
            if (blockCheck)
                currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.blockstun / 2));
            else
            {
                currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.blockstun));
                if (GetIsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
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

        public void CharacterMove()
        {
            Vector2 direction = GetInputDirection();
            float speed = characterData.GetMovementSpeed();
            movementDirectionX = direction.x;
            if (GetCurrentActionState() != ActionStates.None || GetCurrentState() != States.Standing)
                return;

            if (direction.x != 0)
            {
                if (direction.x != faceDir)
                {
                    animator.SetInteger("X", -1);
                    speed = characterData.GetMovementSpeed() / 1.3f;
                }

                else
                {
                    animator.SetInteger("X", 1);
                    speed = characterData.GetMovementSpeed();
                }
            }
            else
                animator.SetInteger("X", 0);


            if (GetIsAgainstTheWall())
            {
                if (direction.x == 0 || direction.x == GetWallDirectionX())
                {
                    transform.Translate(speed * Time.deltaTime * new Vector2(0, 0));
                }
                else
                {
                    SetIsAgainstTheWall(false,0);
                    transform.Translate(speed * Time.deltaTime * new Vector2(direction.x, 0));
                }
            }
            else
                transform.Translate(speed * Time.deltaTime * new Vector2(direction.x, 0));

        }

        public void CharacterPush(float x, int faceDir)
        {
            if (GetIsJumping())
            {
                return;
            }
            if (!GetIsGrounded() && opponent.GetIsAgainstTheWall() && Mathf.Sign(GetMovementDirectionX()) == opponent.GetWallDirectionX())
            {
                movementDirectionX = faceDir;
            }
            if (opponent.GetIsAgainstTheWall() && GetFaceDir() == opponent.GetWallDirectionX())
            {
                SetIsAgainstTheWall(true, GetFaceDir());
                return;
            }
            if (GetIsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
            {
                return;
            }
            if (GetMovementDirectionX() != 0 && opponent.GetMovementDirectionX() == 0 && !opponent.GetIsAgainstTheWall())
            {
                return;
            }
            float pushStrenght = ((opponent.GetIsAgainstTheWall() && opponent.GetFaceDir() != opponent.GetWallDirectionX())
                || !opponent.GetIsGrounded()) ? 3f : 2f;
            transform.Translate(new Vector2(faceDir * pushStrenght, 0) * Time.deltaTime);
        }

        public void ApplyCounterPush(Vector2 direction, float duration)
        {
            opponent.SetIsAgainstTheWall(false, 0);
            Vector2 dir = new(direction.x, 0f);
            opponent.ApplyForce(dir, duration, true);
        }

        public void ApplyForce(Vector2 direction, float duration, bool counterforce = false)
        {
            bool m_bool = false;
            if (counterforce)
            {
                posY = 0;
                m_bool = counterforce;
            }
            else
            {
                if (GetIsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())
                    direction.x = 0;
                else
                    SetIsAgainstTheWall(false, 0);
                posY = direction.y;
                if (posY > 0)
                {
                    SetIsGrounded(false);
                    direction.x *= 0.5f;
                }

                SetApplyGravity(false);
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
                if (GetIsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())
                    direction.x = 0;
                else
                    SetIsAgainstTheWall(false, 0);
                posY = direction.y;
                if (posY > 0)
                    SetIsGrounded(false);
                SetApplyGravity(false);
                SetIsAgainstTheWall(false, 0);
            }

            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(ForceAttackCoroutine(new Vector2(direction.x * faceDir, direction.y), duration, false));

        }

        public void ApplyGravity()
        {
            if (!applyGravity)
                return;
            if (GetCurrentActionState() == ActionStates.None && isJumping)
            {
                characterAnimation.AddAnimation(AnimType.Movement, "JumpFall");
            }
            if (!GetIsGrounded())
                SetIsJumping(false);
            landed = false;

            if ((GetIsAgainstTheWall() && Mathf.Sign(GetWallDirectionX()) == GetWallDirectionX()))
                transform.Translate((forceSpeed) * Time.deltaTime * new Vector2(0, -1));
            else
            {
                transform.Translate((forceSpeed) * Time.deltaTime * new Vector2(GetMovementDirectionX(), -1));
            }

        }


        public void CollisionedWith(Collider2D collider)
        {
            if (collider == GetComponent<Collider2D>())
                return;
            if (collider.TryGetComponent<Pushbox>(out var collidedbox))
                collidedbox.HandleCollision(pushbox);
        }

        public void CheckState()
        {
            Debug.Log(animator.GetCurrentAnimatorClipInfo(0));
        }

        public void AnimEnd()
        {
            opponent.ResetAttackInfo();
            characterAnimation.ClearAnimations();
            if (GetCurrentActionState() == ActionStates.None)
                return;
            stateMachine.SetCurrentActionState(ActionStates.None);

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
                    if (direction.x != 0 && ((GetIsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())))
                        direction.x = 0;
                }
                movementDirectionX = direction.x;
                transform.Translate(forceSpeed * Time.deltaTime * direction);
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
                    if (GetIsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())
                        direction.x = 0;
                }
                movementDirectionX = direction.x;
                if (direction.x != 0)
                    movementDirectionX = direction.x;

                transform.Translate(forceSpeed * Time.deltaTime * direction);
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

        public void HitboxesEnabled()
        {
            visualState = !visualState;
        }

        public void HitConnect(AttackData data)
        {
            animator.speed = 0;
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.hitstun / 2));
            if (opponent.GetCurrentActionState() == ActionStates.Block)
            {
                currentCombo.Clear();
            }
            else
            {
                currentCombo.Add(data);
                Managers.Instance.GameManager.UpdateComboCounter(playerId);
            }
        }

        public void ResetAttackInfo()
        {
            currentCombo.Clear();
            storedAttack = null;
            Managers.Instance.GameManager.UpdateComboCounter(playerId);
        }

        bool IsBlocking(AttackAttribute attack)
        {
            switch (GetCurrentState())
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
