using NaughtyAttributes;
using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        CharacterAttackManager attackManager;

        [Space]

        [SerializeField]
        Animator animator;
        [SerializeField]
        CharacterAnimationManager characterAnimation;

        bool applyGravity = false;
        bool isAgainstTheWall;
        float wallFaceDirection;
        private bool isGrounded = true;
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
        Vector3 origin;
        [SerializeField]
        Projectile currentProjectile;
        int playerId = 0;
        bool isKnockedDown = false;
        bool isHardKnockDown = false;

        private void Awake()
        {
            characterAnimation.Initialize(this, animator);
            inputHandler.Initialize(this);
            stateMachine.Initialize(this);
            attackManager.Initialize(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            origin = transform.position;
            currentHealth = GetMaxHealth();
            if (isPlayer2)
                playerId = 1;
        }

        void FixedUpdate()
        {
            stateMachine.StateMachineUpdate();
            CharacterMove();
        }

        // Update is called once per frame
        void Update()
        {

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
            //if (currentHitCoroutine == null && GetCurrentActionState() == ActionStates.Hit || GetCurrentActionState() == ActionStates.Block)
            //{
            //    currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(5, false));
            //}
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
        public CharacterAnimationsData GetCharacterAnimationsData()
        {
            return characterData.GetCharacterAnimationsData();
        }

        public List<AnimationClip> GetCharacterMovementClips()
        {
            CharacterAnimationsData animationsData = characterData.GetCharacterAnimationsData();
            List<AnimationClip> animationClips = new();
            foreach (var anim in animationsData.standingClips)
            {
                animationClips.Add(anim);
            }
            foreach (var anim in animationsData.jumpingClips)
            {
                animationClips.Add(anim);
            }
            animationClips.Add(animationsData.crouchingClip);
            return animationClips;
        }

        public List<AnimationClip> GetCharacterActionClips()
        {
            CharacterAnimationsData animationsData = characterData.GetCharacterAnimationsData();
            List<AnimationClip> animationClips = new();

            foreach (var anim in animationsData.blockingClips)
            {
                animationClips.Add(anim);
            }
            foreach (var anim in animationsData.hitClips)
            {
                animationClips.Add(anim);
            }
            foreach (var anim in animationsData.stateTransitionClips)
            {
                animationClips.Add(anim);
            }

            foreach (var attack in characterData.GetStandingAttacks())
            {
                animationClips.Add(attack.animation);
                if (attack.followUpAttack != null)
                    animationClips.Add(attack.followUpAttack.animation);
            }
            foreach (var attack in characterData.GetCrouchingAttacks())
            {
                animationClips.Add(attack.animation);
                if (attack.followUpAttack != null)
                    animationClips.Add(attack.followUpAttack.animation);
            }
            foreach (var attack in characterData.GetJumpAttacks())
            {
                animationClips.Add(attack.animation);
                if (attack.followUpAttack != null)
                    animationClips.Add(attack.followUpAttack.animation);
            }
            foreach (var attack in characterData.GetSpecialAttacks())
            {
                animationClips.Add(attack.animation);
                if (attack.followUpAttack != null)
                    animationClips.Add(attack.followUpAttack.animation);
            }
            animationClips.Add(characterData.GetGrabData().animation);
            if (characterData.GetGrabData().followUpAttack != null)
                    animationClips.Add(characterData.GetGrabData().followUpAttack.animation);

            return animationClips;
        }

        public ActionStates GetCurrentActionState()
        {
            return stateMachine.GetActionState();
        }

        public bool IsHardKnockedDown()
        {
            return isHardKnockDown;
        }

        public bool IsKnockedDown()
        {
            return isKnockedDown;
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

        public bool IsJumping()
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

        public bool IsGrounded()
        {
            return isGrounded;
        }

        public void SetIsGrounded(bool value)
        {
            isGrounded = value;
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

        public bool IsAgainstTheWall()
        {
            return isAgainstTheWall;
        }

        public void SetIsAgainstTheWall(bool isAgainstTheWall, float faceDirection)
        {
            this.isAgainstTheWall = isAgainstTheWall;
            wallFaceDirection = faceDirection;
        }

        #endregion
        #region Character Commands
        public void PerformAttack(AttackType type)
        {
            if (type == AttackType.Grab)
            {
                attackManager.Attack(characterData.GetGrabData());
                return;
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
                                    attackManager.Attack(characterData.GetStandingAttacks()[((int)type)]);
                                    break;
                                case 1f:
                                    attackManager.Attack(characterData.GetJumpAttacks()[((int)type)]);
                                    break;
                                case -1f:
                                    attackManager.Attack(characterData.GetCrouchingAttacks()[((int)type)]);
                                    break;
                            }

                        }
                        break;

                    case States.Crouching:
                        switch (inputHandler.GetDirection().y)
                        {
                            case 0f:
                                attackManager.Attack(characterData.GetStandingAttacks()[((int)type)]);
                                break;
                            case -1f:
                                attackManager.Attack(characterData.GetCrouchingAttacks()[((int)type)]);
                                break;
                        }
                        break;
                    case States.Jumping:
                        attackManager.Attack(characterData.GetJumpAttacks()[((int)type)]);
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
                        attackManager.Attack(characterData.GetSpecialAttacks()[id + 1]);

                        break;

                    case States.Crouching:
                        attackManager.Attack(characterData.GetSpecialAttacks()[id + 1]);
                        break;
                    case States.Jumping:
                        break;
                }
            }

        }

        public void HurtboxOnCollision(AttackData data, bool blockCheck = false)
        {
            if (GetCurrentActionState() == ActionStates.Attack)
            {
                PerformGettingHit(data);
                return;
            }
            if (data.grab)
            {
                if (GetCurrentState() == States.Jumping || GetCurrentActionState() == ActionStates.Hit)
                    return;
                PerformGettingHit(data);
                return;
            }

            if (IsBlocking(data.attackAttribute))
            {
                PerformBlock(data, blockCheck);
                return;
            }
            //block
            else if (!blockCheck)
            {
                PerformGettingHit(data);
                return;
            }
        }

        private void PerformGettingHit(AttackData data)
        {
            Vector2 dir = new(data.push.x * -faceDir, 0);
            PlaySound(data.collideSound);
            stateMachine.SetCurrentActionState(ActionStates.Hit);
            if (data.launcher || isKnockedDown)
            {
                characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[2]);
                dir.y = data.push.y;
                isKnockedDown = true;
                if (data.hardKnockdown)
                {
                    isHardKnockDown = true;
                }
            }
            else
            {
                if (data.grab)
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[0]);
                }
                else
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[(int)GetCurrentState()]);

                }
                if (GetCurrentState() == States.Jumping)
                {
                    dir.y = data.push.y;
                    isKnockedDown = true;
                    if (data.hardKnockdown)
                    {
                        isHardKnockDown = true;
                    }
                }
            }
       
            currentHealth -= data.damage;
            Managers.Instance.GameManager.UpdateHealth(playerId, GetCurrentHealth());

            StopHitRecover();
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.hitstun, false, data.launcher));
            if (IsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
            {
                ApplyCounterPush(-dir, 3f);
            }
                ApplyForce(dir, 3f);
        }

        private void PerformBlock(AttackData data, bool blockCheck = false)
        {
            PlaySound(data.collideSound);
            Vector2 dir = new(data.push.x * -GetFaceDir(), 0);
            Vector2 blockDir = new(dir.x, 0);
            stateMachine.SetCurrentActionState(ActionStates.Block);
            StopHitRecover();

            if (!blockCheck)
                characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().blockingClips[(int)GetCurrentState()]);
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(data.blockstun, false));
                if (IsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
                {
                    ApplyCounterPush(-blockDir, 3f);
                }
                else
                    ApplyForce(blockDir, 3f);

        }

        public void CharacterMove()
        {
            if (GetCurrentActionState() != ActionStates.None || GetCurrentState() != States.Standing)
                return;
            float speed = characterData.GetMovementSpeed();
            movementDirectionX = GetInputDirection().x;
            Vector3 destination = new(GetInputDirection().x, 0);

            if (GetInputDirection().x != 0)
            {
                if (IsAgainstTheWall() && Mathf.Sign(destination.x) == GetWallDirectionX())
                    destination.x = 0;
                if (GetInputDirection().x != faceDir)
                {
                    characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips.LastOrDefault());
                    speed = characterData.GetMovementSpeed() / 1.3f;
                }

                else
                {
                    characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips[1]);
                    speed = characterData.GetMovementSpeed();
                }
            }
            else
            characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips.FirstOrDefault());

            transform.position += (speed * Time.deltaTime * destination);

        }

        public void OnAnimationEnd()
        {
                opponent.ResetAttackInfo();
                if (GetCurrentActionState() == ActionStates.None)
                    return;
                stateMachine.SetCurrentActionState(ActionStates.None);
                isKnockedDown = false;
                isHardKnockDown = false;
        }

        public void ResetCharacter()
        {
            ResetPos();
            currentHealth = GetMaxHealth();
        }
        #endregion
        #region AnimEvents
        //Anim Event
        public void SpawnProjectile(Projectile projectile)
        {
            if (currentProjectile != null)
            {
                return;
            }
            Projectile m_projectile = Instantiate(projectile, transform);
            m_projectile.trajectory.x *= faceDir;
            m_projectile.transform.position = new Vector2(transform.position.x + (projectile.origin.x * faceDir), transform.position.y + projectile.origin.y);
            m_projectile.transform.parent = transform.parent;
            m_projectile.hitbox.mask = m_hitbox.mask;
            m_projectile.m_hurtbox.gameObject.layer = m_hurtbox.gameObject.layer;
            m_projectile.parent = this;
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(projectile.data.hitstun, true));
            currentProjectile = m_projectile;
        }

        public void CheckState()
        {
            Debug.Log(animator.GetCurrentAnimatorClipInfo(0));
        }

        //Anim Event
        public void ApplyAttackForce(AttackData data)
        {
            Vector2 direction = data.movement;
            float duration = data.movementDuration;
            {
                if (IsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())
                    direction.x = 0;

                posY = direction.y;
                if (posY > 0)
                {
                    SetIsGrounded(false);
                    SetApplyGravity(false);
                }
            }

            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(ForceAttackCoroutine(new Vector2(direction.x * faceDir, direction.y), duration, false));
        }

        public void OpenHitboxes(int number)
        {
            for (int i = 0; i < number; i++)
            {
                attackManager.hitboxes[i].state = ColliderState.Open;
            }
        }
        #endregion
        #region Movement Physics

        public void ResetPos()
        {
            transform.position = origin;
        }

        public void FixPosition()
        {
            if (!landed)
            {
                transform.position = new Vector3(transform.position.x, 0f, 0);
                landed = true;
            }
        }

        public void ApplyCounterPush(Vector2 direction, float duration)
        {
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
                if (IsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())
                    direction.x = 0;

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

        public void ApplyGravity()
        {
            if (!applyGravity)
                return;
                characterAnimation.ChangeMovementState(stateMachine.GetCharacter().GetCharacterAnimationsData().jumpingClips.LastOrDefault());
            if (!IsGrounded())
                SetIsJumping(false);
            landed = false;

            if ((IsAgainstTheWall() && Mathf.Sign(GetWallDirectionX()) == GetWallDirectionX()))
                transform.Translate((forceSpeed) * Time.deltaTime * new Vector2(0, -1));
            else
            {
                transform.Translate((forceSpeed) * Time.deltaTime * new Vector2(GetMovementDirectionX(), -1));
            }

        }

        public IEnumerator ForceAttackCoroutine(Vector2 direction, float duration, bool counterForce)
        {
            float i = 0f;
            while (i != duration)
            {
                if (!counterForce)
                {
                    if (direction.x != 0 && ((IsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())))
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
                    if (IsAgainstTheWall() && Mathf.Sign(direction.x) == GetWallDirectionX())
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
        #endregion

        public void BoxCollisionedWith(Collider2D collider)
        {
            if (collider == GetComponent<Collider2D>())
                return;
        }

        public IEnumerator RecoveryFramesCoroutines(int frames, bool isAttack, bool knockdown = false)
        {
            characterAnimation.PauseActionPlayabe(frames);
            if (!isAttack)
            {
                int i = 0;
                //add histop
                while (i != frames + 4)
                {
                    yield return null;
                    i++;
                    if (IsGrounded() && knockdown || GetApplyGravity())
                        characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips.Last());
                }
            }
            else
            {
                int i = 0;
                while (i != frames)
                {
                    yield return null;
                    i++;
                }
            }
            HitRecover();

        }

        public void HitRecover()
        {
            characterAnimation.ResumeActionPlayable();
        }

        public void HitboxesEnabled()
        {
            visualState = !visualState;
        }

        public void HitConnect(AttackData data)
        {
            StopHitRecover();
            //hitstop
            currentHitCoroutine = StartCoroutine(RecoveryFramesCoroutines(4, true));
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
            if (GetCurrentActionState() == ActionStates.Hit)
                return false;
            switch (GetCurrentState())
            {
                case States.Standing:
                    if (attack == AttackAttribute.Low)
                        return false;
                    break;
                case States.Crouching:
                    if (attack == AttackAttribute.High)
                        return false;
                    break;
                case States.Jumping:
                    return false;
            }
            if (inputHandler.GetDirection().x == -faceDir || GetCurrentActionState() == ActionStates.Block)
                return true;
            return false;
        }

        public void PlaySound(AudioClip clip = null)
        {
            if (clip != null)
                AudioManager.instance.PlayAnimationEffect(clip, playerId);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            switch (LayerMask.LayerToName(collision.gameObject.layer))
            {
                case ("Pushbox"):
                    if (IsAgainstTheWall() && !IsGrounded())
                    {
                        opponent.transform.Translate(new Vector2(GetFaceDir(),0) * 3 * Time.deltaTime);
                    }
                    break;
                case ("Ground"):
                    SetIsGrounded(true);
                    break;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (LayerMask.LayerToName (collision.gameObject.layer) == "Pushbox")
            {
                if (Managers.Instance.GameManager.GetCornerChar() == opponent)
                    transform.Translate(new Vector2(-GetFaceDir(), 0) * 2 * Time.deltaTime);
            }
        }

        [Button]
        public void EnableInput()
        {
            inputHandler.GetPlayerInput().ActivateInput();
        }

        [Button]
        public void DisableInput()
        {
            inputHandler.GetPlayerInput().DeactivateInput();
        }

        public void StopHitRecover()
        {
            if (currentHitCoroutine != null)
                StopCoroutine(currentHitCoroutine);
        }
    }

}
