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
        public bool isPlayer2;
        [SerializeField]
        Character opponent;
        float faceDir;
        float xDiff;
        [SerializeField]
        SpriteRenderer render;
        [SerializeField]
        GameObject model3D;
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
        [SerializeField]
        bool isAgainstTheWall;
        float wallFaceDirection;
        private bool isGrounded = true;
        float movementDirectionX;
        float posY;

        [Space]

        int currentHealth;

        [Space]
        [SerializeField]
        int wakingUpFrames = 6;
        [SerializeField]
        int jumpStartup = 4;
        [SerializeField]
        float gravity;
        float forceLeftOver;

        bool isJumping;
        bool isRunning = false;

        [Space]

        [SerializeField]
        Transform collisions;
        private Coroutine currentMovementCoroutine;
        private Coroutine currentHitstopCoroutine;
        List<AttackData> currentCombo = new();
        [SerializeField]
        bool visualState;
        [SerializeField]
        Color32[] stateColors;
        //6//
        [SerializeField]
        int hitboxLayerMask = 6;
        //P1 = 7, P2= 8//
        [SerializeField]
        string hurtboxLayerMask;
        [SerializeField]
        LayerMask hitboxTargetLayerMask;
        [SerializeField]
        Vector3 origin;
        [SerializeField]
        Projectile currentProjectile;
        int playerId = 0;
        bool isKnockedDown = false;
        bool isHardKnockDown = false;
        bool isWakingUp = false;
        private int hitstop;

        private MotionInputs storedMotionInput = MotionInputs.NONE;
        private int storedDashDir = 0;
        public bool SameAttackSequence {  get; private set; }
        AttackData onGoingAttack;

        [Space]

        [SerializeField]
        private int superMeter;
        [SerializeField]
        private bool hasBurst;
        [SerializeField]
        private int burstCD = 100;
        [SerializeField]
        private int currentBurstCD = 0;
        [SerializeField]
        private int airActions = 0;
        [SerializeField]
        bool canDoubleJump = false;

        private void Awake()
        {
            CharacterModel characterModel = Instantiate(characterData.CharacterModel, model3D.transform);
            characterModel.Initialize(this);
            animator = characterModel.GetComponent<Animator>();
            collisions = characterModel.GetCollisions();
            characterAnimation.Initialize(this, animator);
            inputHandler.Initialize(this);
            stateMachine.Initialize(this);
            attackManager.Initialize(this, characterModel.GetHitboxes());
            gravity = characterData.GetGravity();
        }

        // Start is called before the first frame update
        void Start()
        {
            origin = transform.position;
            currentHealth = GetMaxHealth();
            if (isPlayer2)
            {
                playerId = 1;
            }
        }

        void FixedUpdate()
        {
            inputHandler.Update();
            stateMachine.StateMachineUpdate();
            CharacterMove();
            if (!hasBurst)
                currentBurstCD++;
            if (currentBurstCD == burstCD)
            {
                hasBurst = true;
                currentBurstCD = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {
            characterAnimation.AnimUpdate();
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

                }
                else
                {
                    faceDir = -1;
                    if (render != null)
                        render.flipX = true;
                }
                vfx.flipX = render.flipX;
                model3D.transform.localScale = new Vector3(Mathf.Abs(model3D.transform.localScale.x) * faceDir, model3D.transform.localScale.y, model3D.transform.localScale.z);
                model3D.transform.localRotation = new Quaternion(model3D.transform.localRotation.x, Mathf.Abs(model3D.transform.localRotation.y) * faceDir, model3D.transform.localRotation.z, model3D.transform.localRotation.w);
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
                    case ActionStates.Attack:
                        render.color = stateColors[2];
                        break;
                }
            }

        }

        #region Getters and Setters


        public InputHandler GetInputHandler()
        {
           return inputHandler;
        }

        public LayerMask GetHitboxLayerMask()
        {
            return hitboxLayerMask;
        }

        public LayerMask GetHitboxTargetMask()
        {
            return hitboxTargetLayerMask;
        }

        public string GetHurtboxLayerMask()
        {
            return hurtboxLayerMask;
        }

        public void SetMotionInput(MotionInputs motion)
        {
            if (storedMotionInput == motion)
                return;
            switch (motion)
            {
                case MotionInputs.ff:
                case MotionInputs.bb:
                    PerformDash();
                    break;
                default:
                    storedMotionInput = motion;
                    break;
            }
        }

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
            foreach (var anim in animationsData.wakeupClips)
            {
                animationClips.Add(anim);
            }
            foreach (var anim in animationsData.recoveryClips)
            {
                animationClips.Add(anim);
            }
            foreach (var anim in animationsData.cancelClips)
            {
                animationClips.Add(anim);
            }

            // Attack Animations
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
            foreach (var attack in characterData.GetForwardAttacks())
            {
                animationClips.Add(attack.animation);
                if (attack.followUpAttack != null)
                    animationClips.Add(attack.followUpAttack.animation);
            }
            foreach (var attack in characterData.GetGrabData())
            {
                animationClips.Add(attack.animation);
                if (attack.followUpAttack != null)
                    animationClips.Add(attack.followUpAttack.animation);
            }

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

        public bool WasYReleased()
        {
            return inputHandler.GetWasYReleased();
        }

        public bool CanDoubleJump()
        {
            return canDoubleJump;
        }

        public void SetDoubleJump(bool value)
        {
            canDoubleJump = value;
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

        public float GetJumpPower()
        {
            return characterData.GetJumpPower();
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

        public Character GetOpponent()
        {
            return opponent;
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

        public int GetAirActions()
        {
            return airActions;
        }

        public void ResetAirActions()
        {
            airActions = characterData.GetAirActions();
        }

        public bool CanJump()
        {
            if (GetCurrentActionState() != ActionStates.None)
            {
                if (onGoingAttack != null && onGoingAttack.jumpCancel && opponent.GetCurrentActionState() == ActionStates.Hit)
                {
                    ResetAttackSequence();
                    return true;
                }
                return false;
            }
            return true;
        }

        public bool CanDash()
        {
            if (GetCurrentActionState() != ActionStates.None)
            {
                if (IsGrounded() && GetCurrentActionState() == ActionStates.Landing)
                    return true;
                if (onGoingAttack != null && onGoingAttack.dashCancel && opponent.GetCurrentActionState() == ActionStates.Hit && GetInputDirection().x == faceDir)
                {
                    ResetAttackSequence();
                    return true;
                }
                return false;
            }
            return true;
        }

        public bool CanPerformOffensiveAction()
        {
            if (GetCurrentActionState() != ActionStates.Attack)
                return false;
            if (opponent.GetCurrentActionState() == ActionStates.Block)
                return true;
            if (opponent.GetCurrentActionState() == ActionStates.Hit)
                return true;
            return false;
        }

        public void ResetAttackSequence()
        {
            SameAttackSequence = false;
        }

        #endregion
        #region Character Commands
        void PerformDash()
        {
            if (!CanDash())
            {
                return;
            }
            if (GetInputDirection().x == faceDir)
            {
                if (GetCurrentActionState() == ActionStates.Attack)
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().jumpingClips[2], 10);
                    ApplyForce(new Vector2(faceDir * 1.5f, 0f), 10);
                    return;
                }
                if (GetCurrentState() == States.Standing)
                {
                    isRunning = true;
                }
                else if ((GetCurrentState() == States.Jumping) && (airActions > 0))
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().jumpingClips[2], 22);
                    ApplyForce(new Vector2(faceDir * 1.5f, 0.1f), 10);
                    airActions--;
                }

            }
            else
            {

                if (GetCurrentState() == States.Standing)
                {
                    SetActionState(ActionStates.Landing);
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().standingClips.LastOrDefault(), 22);
                    ApplyForce(new Vector2(-faceDir * 2, 0.5f), 10);
                }
                else if ((GetCurrentState() == States.Jumping) && (airActions > 0))
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().standingClips.LastOrDefault(), 22);
                    ApplyForce(new Vector2(-faceDir, 0.1f), 10);
                    airActions--;
                }
            }
        }

        public void PerformJump()
        {
            //SetActionState(ActionStates.None);
            if (GetCurrentState() == States.Jumping)
            {
                if (airActions > 0)
                    airActions--;
                else
                    return;
            }
            SetIsJumping(true);
            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = StartCoroutine(JumpCoroutine());
        }

        public void PerformInput(InputType type)
        {
            switch (GetCurrentActionState())
            {
                case ActionStates.None:
                    PerformNeutralAction(type);
                    break;
                case ActionStates.Attack:
                    PerformOffensiveAction(type);
                    //Rapid and OD
                    break;
                case ActionStates.Block:
                    //GC and OD
                    break;
                case ActionStates.Hit:
                    //OD
                    break;
            }
        }

        private void PerformNeutralAction(InputType type)
        {
            if (superMeter > 0 && type == InputType.MH && GetInputDirection().x != faceDir)
            {
                Debug.Log("Barrier");
                superMeter --;
                return;
            }

            if (superMeter >= 50 && type == InputType.MH)
            {
                Debug.Log("GuardBreak");
                superMeter -= 50;
                return;
            }

            if (hasBurst && type == InputType.LMHU)
            {
                Debug.Log("Burst");
                hasBurst = false;
                return;
            }
            PerformAttack(type);
        }

        private void PerformOffensiveAction(InputType type)
        {
            if (!CanPerformOffensiveAction())
                return;

            //Check for special actions like rapid or OD activation
            if (superMeter >= 50 && type == InputType.LMH)
            {
                Debug.Log("Rapid");
                superMeter -= 50;
                return;
            }
            if (superMeter>= 50 && type == InputType.MH)
            {
                Debug.Log("GuardBreak");
                superMeter -= 50;
                return;
            }

            if (hasBurst && type == InputType.LMHU)
            {
                Debug.Log("Burst");
                hasBurst = false;
                return;
            }
            PerformAttack(type);
        }

        private void PerformDefensiveAction(InputType type)
        {
            //Check for guardbreak or burst activation
            if (superMeter > 0 && type == InputType.MH)
            {
                Debug.Log("Barrier");
                superMeter --;
                return;
            }

            if (superMeter >= 50 && type == InputType.MH && GetInputDirection().x == faceDir)
            {
                Debug.Log("AttackBreak");
                superMeter -= 50;
                return;
            }

            if (hasBurst && type == InputType.LMHU)
            {
                Debug.Log("Burst");
                hasBurst = false;
                return;
            }

        }

        public void PerformAttack(InputType type)
        {
            if (type == InputType.Heavy || type == InputType.Unique)
                return;
            if (storedMotionInput != MotionInputs.NONE)
            {
                if (characterData.FindSpecialAttack(storedMotionInput, type) != null)
                {
                    attackManager.Attack(characterData.FindSpecialAttack(storedMotionInput, type));
                    return;
                }                
            }

            if (type == InputType.LU)
            {
                if (GetCurrentState() != States.Jumping)
                    attackManager.Attack(characterData.GetGrabData()[0]);
                else
                    attackManager.Attack(characterData.GetGrabData()[1]);
                return;
            }
            if ((int)type > characterData.GetStandingAttacks().Length)
                return;
 
                AttackData attackData = new AttackData();
                switch (GetCurrentState())
                {
                    case States.Standing:

                        {
                            switch (inputHandler.GetDirection().y)
                            {
                                case 0f:
                                attackData = inputHandler.GetDirection().x == faceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetStandingAttacks()[((int)type)];
                                    break;
                                case 1f:
                                    attackData = characterData.GetJumpAttacks()[((int)type)];
                                    break;
                                case -1f:
                                attackData = inputHandler.GetDirection().x == faceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetCrouchingAttacks()[((int)type)];
                                    break;
                            }

                        }
                        break;

                    case States.Crouching:
                        switch (inputHandler.GetDirection().y)
                        {
                            case 0f:
                            attackData = inputHandler.GetDirection().x == faceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetStandingAttacks()[((int)type)];
                                break;
                            case -1f:
                            attackData = inputHandler.GetDirection().x == faceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetCrouchingAttacks()[((int)type)];
                                break;
                        }
                        break;
                    case States.Jumping:
                        attackData = characterData.GetJumpAttacks()[((int)type)];
                        break;
                }
                if (attackData != null)
                {
                    attackManager.Attack(attackData);
                }

            //Special Attack Button
            //else
            //{
            //    int id = ((int)(inputHandler.GetDirection().x * faceDir));
            //    switch (GetCurrentState())
            //    {

            //        case States.Standing:
            //            if (id == 0 && currentProjectile != null)
            //                return;
            //            attackManager.Attack(characterData.GetSpecialAttacks()[id + 1]);

            //            break;

            //        case States.Crouching:
            //            attackManager.Attack(characterData.GetSpecialAttacks()[id + 1]);
            //            break;
            //        case States.Jumping:
            //            break;
            //    }
            //}

        }

        public void Attack(AttackData attackData)
        {
            onGoingAttack = attackData;
            SetActionState(ActionStates.Attack);
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
            Vector2 dir = CalculateHitPush(data);
            PlaySound(data.collideSound);;
            if (data.launcher || isKnockedDown)
            {
                characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[2], CalculateHitstun(data));
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
                    if (GetCurrentState() == States.Jumping)
                    {
                        characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[(int)GetCurrentState()], CalculateHitstun(data));
                   }
                    else
                        characterAnimation.PlayHitAnimation(GetCharacterAnimationsData().hitClips[(int)GetCurrentState()], CalculateHitstun(data));
                }
            }
            if (currentHitstopCoroutine != null)
                StopCoroutine(currentHitstopCoroutine);

            hitstop = data.attackLevel + 8;
            currentHitstopCoroutine = StartCoroutine(WaitForHitStopCoroutine());
            if (currentHealth > 0)
            {
                currentHealth -= data.damage;
                Managers.Instance.GameManager.UpdateHealth(playerId, GetCurrentHealth());
            }
            if (IsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
            {
                ApplyCounterPush(-dir, 3f);
            }
                ApplyForce(dir, 3f);
            SetActionState(ActionStates.Hit);
        }

        private void PerformBlock(AttackData data, bool blockCheck = false)
        {
            PlaySound(data.collideSound);
            Vector2 dir = CalculateHitPush(data);
            Vector2 blockDir = new(dir.x, 0);
            SetActionState(ActionStates.Block);

            if (!blockCheck)
                characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().blockingClips[(int)GetCurrentState()]);
                if (IsAgainstTheWall() && GetFaceDir() != GetWallDirectionX())
                {
                    ApplyCounterPush(-blockDir, 3f);
                }
                else
                    ApplyForce(blockDir, 3f);

        }

        private int CalculateHitstun(AttackData data)
        {
            if (data.attackLevel == 0)
                data.attackLevel = 1;
            int result = (data.attackLevel * 2) + 10 + data.extraHitstun; //attacklevel + hitstunbase(10) + extra
            return result;
        }

        private Vector2 CalculateHitPush(AttackData data)
        {
            Vector2 result = new Vector2();
            result.x = ((data.attackLevel) + data.extraPush.x) * -GetFaceDir();
            if (data.launcher || isKnockedDown || GetCurrentState() == States.Jumping)
            {
                result.y = (data.attackLevel) + data.extraPush.y;
                if (result.y == 0)
                {
                    result.y = 1;
                }
            }

            return result;
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
                    characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips[2]);
                    speed = characterData.GetMovementSpeed() / 1.5f;
                    isRunning = false;
                }

                else
                {
                    int moveId = isRunning ? 3 : 1;
                    characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips[moveId]);
                    speed = isRunning ? characterData.GetRunSpeed() : characterData.GetMovementSpeed();
                }
            }
            else
            {
                characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips.FirstOrDefault());
                isRunning = false;
            }


            transform.position += (speed * Time.fixedDeltaTime * destination);

        }

        public void OnAnimationEnd()
        {
            if (GetCurrentActionState() == ActionStates.Hit)
                opponent.ResetAttackInfo();
            characterAnimation.OnActionAnimationEnd();
            SetActionState(ActionStates.None);
            isKnockedDown = false;
            isHardKnockDown = false;
            isWakingUp = false;
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
            m_projectile.hitbox.targetMask = hitboxLayerMask;
            m_projectile.m_hurtbox.gameObject.layer = LayerMask.NameToLayer(hurtboxLayerMask);
            m_projectile.parent = this;
            currentProjectile = m_projectile;
        }

        public void AnimationMovement(Vector2 direction)
        {
            if (direction == null)
                return;
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

            currentMovementCoroutine = StartCoroutine(ForceCoroutine(new Vector2(direction.x * faceDir, direction.y), 100, false));
        }

        public void AnimationMovementEnd()
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }

        public void ClearPreviousAttack()
        {
            attackManager.ClearPreviousAttack();
        }
        
        public void AnimEvent()
        {
            characterAnimation.GetAnimationTime();
        }

        public void PlaySound(AudioClip clip = null)
        {
            if (clip != null)
                AudioManager.instance.PlayAnimationEffect(clip, playerId);
        }

        #endregion
        #region Movement Physics

        public void ResetPos()
        {
            transform.position = origin;
        }

        public void FixPosition()
        {
            transform.position = new Vector3(transform.position.x, 0f, 0);
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
                characterAnimation.ChangeMovementState(stateMachine.GetCharacter().GetCharacterAnimationsData().jumpingClips.FirstOrDefault());
            if (!IsGrounded() && currentMovementCoroutine == null)
                SetIsJumping(false);
            if ((IsAgainstTheWall() && Mathf.Sign(GetWallDirectionX()) == GetWallDirectionX()))
                transform.Translate((gravity) * Time.fixedDeltaTime * new Vector2(0, -1));
            else
            {
                transform.Translate((gravity) * Time.fixedDeltaTime * new Vector2(GetMovementDirectionX(), -1));
            }

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

                transform.Translate(gravity * Time.fixedDeltaTime * direction);
                yield return new FrameWait(1);
                i++;
                forceLeftOver = duration - i;
            }
            currentMovementCoroutine = null;
        }

        public IEnumerator WaitForHitStopCoroutine()
        {
            int target = hitstop;
            for (int hitstopframe = 0; hitstopframe < target; hitstopframe++)
            {
                if (hitstopframe == 1)
                    characterAnimation.PauseActionPlayabe();
                yield return new FrameWait(1);
                hitstop--;
            }
            characterAnimation.ResumeActionPlayable();
            //Followup of grabs or other attacks
            if (currentCombo.Count != 0 && currentCombo.Last().followUpAttack != null)
            {
                attackManager.Attack(currentCombo.Last().followUpAttack, true);
            }
            currentHitstopCoroutine = null;
        }

        public IEnumerator JumpCoroutine()
        {
            SetActionState(ActionStates.Landing);
            stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
            stateMachine.GetCharacter().GetCharacterAnimation().PlayActionAnimation(stateMachine.GetCharacter().GetCharacterAnimationsData().jumpingClips.FirstOrDefault());
            float jumpPower = GetJumpPower();
            // SuperJump
            if (storedMotionInput == MotionInputs.du)
            {
                Debug.Log("SuperJump");
                jumpPower = jumpPower * 2;
            }

            yield return new FrameWait(jumpStartup);
            SetActionState(ActionStates.None);
            ApplyForce(new Vector2(GetInputDirection().x, 1f), jumpPower);
            storedMotionInput = MotionInputs.NONE;
        }
        #endregion

        public void BoxCollisionedWith(Collider2D collider)
        {
            if (collider == GetComponent<Collider2D>())
                return;
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
            if (currentHitstopCoroutine != null)
                StopCoroutine(currentHitstopCoroutine);

            hitstop = data.attackLevel + 8;
            currentHitstopCoroutine = StartCoroutine(WaitForHitStopCoroutine());

            if (opponent.GetCurrentActionState() == ActionStates.Block)
            {
                currentCombo.Clear();
            }
            else
            {
                currentCombo.Add(data);
                Managers.Instance.GameManager.UpdateComboCounter(playerId);
                
            }
            SameAttackSequence = true;
        }

        public void ResetAttackInfo()
        {
            currentCombo.Clear();
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
                    if (attack == AttackAttribute.Mid)
                        return false;
                    break;
            }
            if (inputHandler.GetDirection().x == -faceDir || GetCurrentActionState() == ActionStates.Block)
                return true;
            return false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            switch (LayerMask.LayerToName(collision.gameObject.layer))
            {
                case ("Pushbox"):
                    if (IsAgainstTheWall() && !IsGrounded())
                    {
                        if (!opponent.IsAgainstTheWall())
                        {
                            opponent.transform.Translate(new Vector2(GetFaceDir(), 0) * 3 * Time.fixedDeltaTime);
                        }
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
                if (Managers.Instance.GameManager.GetCornerChar() == opponent && !IsAgainstTheWall())
                {
                    transform.Translate(new Vector2(-GetFaceDir(), 0) * 2 * Time.fixedDeltaTime);
                }
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

        [Button]
        public void ToggleRecording()
        {
            if (!Managers.Instance.GameManager.IsRecording)
            inputHandler.InputRecordingList.Clear();
            Managers.Instance.GameManager.ToggleRecording();
            Debug.Log(Managers.Instance.GameManager.IsRecording);
        }

        [Button]
        public void StartPlayback()
        {
            if (Managers.Instance.GameManager.IsRecording)
                Managers.Instance.GameManager.ToggleRecording();
            inputHandler.StartPlayback();
        }
    }

}

public class FrameWait : CustomYieldInstruction
{
    private int framesToWait;
    private int currentFrame;

    // Constructor to set the number of frames to wait
    public FrameWait(int frames)
    {
        framesToWait = frames;
        currentFrame = 0;
    }

    // Custom logic to check if the wait is over
    public override bool keepWaiting
    {
        get
        {
            currentFrame++;
            return currentFrame < framesToWait;  // Wait until the number of frames has passed
        }
    }
}
