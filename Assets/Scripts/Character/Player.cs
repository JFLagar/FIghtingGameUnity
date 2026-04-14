using SkillIssue.Animations;
using SkillIssue.Inputs;
using SkillIssue.StateMachineSpace;
using System;
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
    public class Player : MonoBehaviour, IPhysics, IHitboxResponder
    {
        StateMachine stateMachine;
        InputHandler inputHandler;

        public bool isPlayer2;
        [SerializeField]
        Player opponent;
        public float FaceDir { get; private set; }
        float xDiff;
        [SerializeField]
        SpriteRenderer render;
        [SerializeField]
        GameObject model3D;
        [SerializeField]
        SpriteRenderer vfx;

        [Space]
        //Turn this into into scriteable object
        [SerializeField]
        CharacterData characterData;

        [Space]

        [SerializeField]
        PlayerAttackManager attackManager;

        [Space]

        [SerializeField]
        Animator animator;
        [SerializeField]
        CharacterAnimationManager characterAnimation;

        public bool IsApplyingGravity { get; private set; }
        public bool IsAgainstTheWall { get; private set; }
        public float WallFaceDirection { get; private set; }
        public bool IsGrounded { get; private set; }
        public float MovementDirectionX { get; private set; }
        public float PosY { get; private set; }
        public int CurrentHealth { get; private set; }

        public AttackData HitAttack { get; private set; }

        [Space]

        //Move to General combat
        [SerializeField]
        int jumpStartup = 4;
        float gravity;
        float forceLeftOver;

        public bool IsJumping { get; private set; }
        bool isRunning = false;

        [Space]

        [SerializeField]
        Transform collisions;
        private Coroutine currentMovementCoroutine;
        private Coroutine currentHitstopCoroutine;
        public List<AttackData> CurrentCombo { get; private set; }
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
        private int hitstop;

        public MotionInputs StoredMotionInput { get; private set; }
        public bool SameAttackSequence { get; private set; }
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
        public int AirActions { get; private set; }
        public bool CanDoubleJump { get; private set; }
        private CharacterModel characterModel;
        private bool isAnyHitboxOpen => characterModel != null && characterModel.GetHitboxes().FirstOrDefault(c => c.state == ColliderState.Open) != null;

        //EVENTS
        public event Action _hitAction, _blockAction, _attackAction, _dashAction, _jumpAction, _onAnimationEnd;
        public event Action _returnToStand, _returnToCrouch, _returnToJump;
        public event Action _overdriveAction, _quarterMeterAction, _halfMeterAction;
        public event Action<InputType> _inputAction;

        void AddTransition(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void AddAnyTransition(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        AnimationData currentAnimation = null;
        [SerializeField]
        int currentFrame = 0;

        public void Initialize()
        {
            characterModel = Instantiate(characterData.GetCharacterModel(), model3D.transform);
            characterModel.Initialize(this);
            animator = characterModel.GetComponent<Animator>();
            collisions = characterModel.GetCollisions();
            characterAnimation.Initialize(this, animator);
            inputHandler = new InputHandler();
            inputHandler.Initialize(this);
            InitializeStateMachine();
            attackManager.Initialize(this, characterModel.GetHitboxes());
            gravity = characterData.GetGravity();
            CurrentCombo = new List<AttackData>();
            IsGrounded = true;
            SetMotionInput(MotionInputs.NONE);
        }

        void InitializeStateMachine()
        {
            stateMachine = new StateMachine();

            StandingState standingState = new StandingState(this, stateMachine);
            CrouchingState crouchingState = new CrouchingState(this, stateMachine);
            JumpingState jumpingState = new JumpingState(this, stateMachine);

            AttackState attackState = new AttackState(this, stateMachine);
            BlockState blockState = new BlockState(this, stateMachine);
            HitState hitState = new HitState(this, stateMachine);

            AddAnyTransition(hitState, new ActionPredicate(ref _hitAction));
            AddAnyTransition(attackState, new ActionWithFuncPredicate(ref _attackAction, () => stateMachine.CanAttack()));
            AddAnyTransition(blockState, new ActionWithFuncPredicate(ref _blockAction, () => stateMachine.CanBlock()));
            AddAnyTransition(standingState, new ActionPredicate(ref _halfMeterAction));
            AddAnyTransition(standingState, new ActionPredicate(ref _overdriveAction));
            AddAnyTransition(standingState, new ActionPredicate(ref _returnToStand));
            AddAnyTransition(crouchingState, new ActionPredicate(ref _returnToCrouch));
            AddAnyTransition(jumpingState, new ActionPredicate(ref _returnToJump));

            //TO DO EventPredicate for Blocking Attacking and Getting Hit for any transition

            AddTransition(jumpingState, standingState, new FuncPredicate(() => IsGrounded));
            AddTransition(crouchingState, standingState, new FuncPredicate(() => GetInputDirection().y >= 0));
            AddTransition(standingState, crouchingState, new FuncPredicate(() => GetInputDirection().y < 0));
            AddTransition(standingState, jumpingState, new FuncPredicate(() => !IsGrounded));

            stateMachine.SetState(standingState);
        }

        // Start is called before the first frame update
        void Start()
        {
            origin = transform.position;
            CurrentHealth = GetMaxHealth();
            if (isPlayer2)
            {
                playerId = 1;
            }
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
            if (!hasBurst)
                currentBurstCD++;
            if (currentBurstCD == burstCD)
            {
                hasBurst = true;
                currentBurstCD = 0;
            }
            if (currentAnimation != null)
                CheckForFrameEvents();
        }

        // Update is called once per frame
        void Update()
        {
            characterAnimation.AnimUpdate();
            stateMachine.Update();
            inputHandler.Update();
            if (opponent == null)
                return;

            xDiff = transform.position.x - opponent.transform.position.x;

            if (GetCurrentState() is HitState)
            {
                if (currentProjectile != null)
                {
                    DestroyImmediate(currentProjectile.gameObject);
                }
            }

        }

        void CheckForFrameEvents()
        {
            currentFrame++;
            FrameEvent frame = currentAnimation.FrameEvents().FirstOrDefault(c => c.Frame == currentFrame);
            if (frame != null)
            {
                ProcessFrame(currentAnimation.FrameEvents().FirstOrDefault(c => c.Frame == currentFrame));
            }
        }

        void ProcessFrame(FrameEvent frame)
        {

            switch (frame.Type())
            {
                case AnimationData.EventType.CollisionBox:
                    {
                        foreach (var hurtbox in characterModel.GetHurtboxes())
                        {
                            hurtbox.SetSize(Vector3.zero);
                            hurtbox.transform.position = Vector3.zero;
                            hurtbox.SetState(ColliderState.Closed);
                        }

                        for (int i = 0; i < frame.Hitboxes().Count + 1; i++)
                        {
                            if (i == 0)
                                continue;
                            if (i == 1)
                            {
                                foreach (var box in characterModel.GetHitboxes())
                                {
                                    box.SetState(ColliderState.Closed);
                                }
                            }
                            Hitbox hitbox = characterModel.GetHitboxes()[i - 1];
                            hitbox.SetSize(frame.Hitboxes()[i - 1].Size());
                            hitbox.SetPosition(frame.Hitboxes()[i - 1].Position());
                            hitbox.SetState(frame.Hitboxes()[i - 1].State());
                        }
                        for (int i = 0; i < frame.Hurtboxes().Count + 1; i++)
                        {
                            if (i == 0)
                                continue;
                            if (i == 1)
                            {
                                foreach (var box in characterModel.GetHurtboxes())
                                {
                                    box.SetState(ColliderState.Closed);
                                }
                            }
                            Hurtbox hurtbox = characterModel.GetHurtboxes()[i - 1];
                            hurtbox.SetSize(frame.Hurtboxes()[i - 1].Size());
                            hurtbox.SetPosition(frame.Hurtboxes()[i - 1].Position());
                            hurtbox.SetState(frame.Hurtboxes()[i - 1].State());
                        }
                    }
                    break;
                case AnimationData.EventType.AnimationEnd:
                    {
                        Debug.Log(frame.Frame + "End");
                        currentAnimation = null;
                    }
                    break;
                case AnimationData.EventType.Projectile:
                    {
                        SpawnProjectile();
                    }
                    break;
                case AnimationData.EventType.Movement:
                    {
                        AnimationMovement();
                    }
                    break;
                case AnimationData.EventType.MovementEnd:
                    {
                        AnimationMovementEnd();
                    }
                    break;

            }
        }

        #region Getters and Setters
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

        public CharacterData GetCharacterData()
        {
            return characterData;
        }

        public void PerformRecovery()
        {
            opponent.ResetAttackInfo();
            MovementDirectionX = 0;
        }

        public void SetMotionInput(MotionInputs motion)
        {
            if (StoredMotionInput == motion)
                return;
            switch (motion)
            {
                case MotionInputs.ff:
                case MotionInputs.bb:
                    PerformDash();
                    break;
                default:
                    StoredMotionInput = motion;
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
                animationClips.Add(anim.AnimationClip());
            }
            foreach (var anim in animationsData.jumpingClips)
            {
                animationClips.Add(anim.AnimationClip());
            }
            animationClips.Add(animationsData.crouchingClip.AnimationClip());
            return animationClips;
        }

        public List<AnimationClip> GetCharacterActionClips()
        {
            CharacterAnimationsData animationsData = characterData.GetCharacterAnimationsData();
            List<AnimationClip> animationClips = new();

            foreach (var anim in animationsData.blockingClips)
            {
                animationClips.Add(anim.AnimationClip());
            }
            foreach (var anim in animationsData.hitClips)
            {
                animationClips.Add(anim.AnimationClip());
            }
            foreach (var anim in animationsData.stateTransitionClips)
            {
                animationClips.Add(anim.AnimationClip());
            }
            foreach (var anim in animationsData.wakeupClips)
            {
                animationClips.Add(anim.AnimationClip());
            }
            foreach (var anim in animationsData.recoveryClips)
            {
                animationClips.Add(anim.AnimationClip());
            }
            foreach (var anim in animationsData.cancelClips)
            {
                animationClips.Add(anim.AnimationClip());
            }

            // Attack Animations
            foreach (var attack in characterData.GetStandingAttacks())
            {
                animationClips.Add(attack.GetAnimationClip().AnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip().AnimationClip());
            }
            foreach (var attack in characterData.GetCrouchingAttacks())
            {
                animationClips.Add(attack.GetAnimationClip().AnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip().AnimationClip());
            }
            foreach (var attack in characterData.GetJumpAttacks())
            {
                animationClips.Add(attack.GetAnimationClip().AnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip().AnimationClip());
            }
            foreach (var attack in characterData.GetSpecialAttacks())
            {
                animationClips.Add(attack.GetAnimationClip().AnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip().AnimationClip());
            }
            foreach (var attack in characterData.GetForwardAttacks())
            {
                animationClips.Add(attack.GetAnimationClip().AnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip().AnimationClip());
            }
            foreach (var attack in characterData.GetGrabData())
            {
                animationClips.Add(attack.GetAnimationClip().AnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip().AnimationClip());
            }

            return animationClips;
        }

        public bool IsHardKnockedDown()
        {
            return isHardKnockDown;
        }

        public bool IsKnockedDown()
        {
            return isKnockedDown;
        }

        public void SetAirActions(int airActions)
        {
            AirActions = airActions;
        }

        public Vector2 GetInputDirection()
        {
            return inputHandler.GetDirection();
        }

        public bool WasYReleased()
        {
            return inputHandler.WasYReleased;
        }

        public void SetDoubleJump(bool value)
        {
            CanDoubleJump = value;
        }

        public IState GetCurrentState()
        {
            return stateMachine.GetState();
        }

        public CharacterAnimationManager GetCharacterAnimation()
        {
            return characterAnimation;
        }

        public float GetJumpPower()
        {
            return characterData.GetJumpPower();
        }

        public int GetComboCount()
        {
            return CurrentCombo.Count;
        }

        public void SetApplyGravity(bool value)
        {
            IsApplyingGravity = value;
        }

        public void SetIsJumping(bool value)
        {
            IsJumping = value;
        }

        public bool IsStillInMovement()
        {
            if (currentMovementCoroutine != null)
                return true;
            return false;
        }

        public void SetIsGrounded(bool value)
        {
            IsGrounded = value;
        }

        public int GetMaxHealth()
        {
            return characterData.GetMaxHP();
        }

        public Player GetOpponent()
        {
            return opponent;
        }

        public void SetWallDirectionX(float direction)
        {
            WallFaceDirection = direction;
        }


        public void SetIsAgainstTheWall(bool isAgainstTheWall, float faceDirection)
        {
            this.IsAgainstTheWall = isAgainstTheWall;
            WallFaceDirection = faceDirection;
        }

        public void ResetAirActions()
        {
            AirActions = characterData.GetAirActions();
        }

        public bool CanLandCancel()
        {
            if (onGoingAttack != null && onGoingAttack.GetAttackState() != States.Jumping)
                return false;
            return true;
        }

        public bool CanJump()
        {
            if (GetCurrentState() is AttackState)
            {
                if (onGoingAttack != null && onGoingAttack.GetCancelTypes().ToList().Contains(CancelTypes.Jump) && isAnyHitboxOpen && opponent.GetCurrentState() is HitState)
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
            if (GetCurrentState() is AttackState)
            {
                if (onGoingAttack != null && onGoingAttack.GetCancelTypes().ToList().Contains(CancelTypes.Dash) && opponent.GetCurrentState() is HitState && GetInputDirection().x == FaceDir)
                {
                    ResetAttackSequence();
                    return true;
                }
                return false;
            }
            return true;
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
            // Perform dash event in statemachines
            _dashAction.Invoke();
        }

        public void SetRunning(bool value)
        {
            isRunning = value;
        }

        public void PerformJump()
        {
            if (!CanJump())
            {
                return;
            }
            _jumpAction.Invoke();
        }

        public void StartJumping()
        {
            if (IsJumping && GetCurrentState() is StandingState || IsJumping && GetCurrentState() is AttackState)
                return;
            SetIsJumping(true);
            if (currentMovementCoroutine != null)
            {
                StopCoroutine(currentMovementCoroutine);
            }
            currentMovementCoroutine = StartCoroutine(JumpCoroutine());
        }

        public void PerformInput(InputType type)
        {
            _inputAction.Invoke(type);
        }

        public bool PerformOverdrive()
        {
            if (!hasBurst)
                return false;
            hasBurst = false;
            _overdriveAction.Invoke();
            return true;
        }

        public bool PerformHalfMeterAction()
        {
            if (superMeter < Managers.Instance.GameManager.GetCombatValues().GetHalfMeter())
                return false;
            superMeter -= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter();
            _halfMeterAction.Invoke();
            return true;
            // Rapid logic can go here since its the same
        }

        public bool PerformQuarterMeterAction()
        {
            if (superMeter < Managers.Instance.GameManager.GetCombatValues().GetHalfMeter() / 2)
                return false;
            superMeter -= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter() / 2;
            _quarterMeterAction.Invoke();
            return true;
            // Attack break logic can go here since its the same
        }

        public void ProcessAttack(AttackData attack)
        {
            // Perform attack event in statemachines
            if (attack == null)
                return;

            if (StoredMotionInput != MotionInputs.NONE)
            {
                if (characterData.FindSpecialAttack(StoredMotionInput, attack.GetInputType()) != null)
                {
                    if (GetCurrentState() is JumpingState || stateMachine.GetPreviousMovementState() is JumpingState)
                    {
                        attackManager.ProcessAttack(characterData.FindSpecialAttack(StoredMotionInput, attack.GetInputType(), true));
                        return;
                    }
                    else
                    {
                        attackManager.ProcessAttack(characterData.FindSpecialAttack(StoredMotionInput, attack.GetInputType()));
                        return;
                    }
                }
            }
            attackManager.ProcessAttack(attack);
        }

        public void PerformAttack(AttackData attackData)
        {
            onGoingAttack = attackData;
            _attackAction.Invoke();
        }

        public void HurtboxOnCollision(AttackData attack, bool blockCheck = false)
        {
            // COUNTERHIT
            if (GetCurrentState() is AttackState)
            {
                PerformGettingHit(attack);
                return;
            }

            if (attack.IsGrab())
            {
                if (GetCurrentState() is JumpingState || GetCurrentState() is HitState)
                    return;
                PerformGettingHit(attack);
                return;
            }

            if (IsBlocking(attack))
            {
                PerformBlock(attack, blockCheck);
                return;
            }
            //block
            else if (!blockCheck)
            {
                PerformGettingHit(attack);
                return;
            }
        }

        private void PerformGettingHit(AttackData attack)
        {
            HitAttack = attack;
            hitstop = HitAttack.GetAttackLevel() + Managers.Instance.GameManager.GetCombatValues().GetHitstopBase();
            _hitAction.Invoke();
            // On Enter of Hit State          
        }

        public void StartHitstopCoroutine()
        {
            if (currentHitstopCoroutine != null)
                StopCoroutine(currentHitstopCoroutine);

            currentHitstopCoroutine = StartCoroutine(WaitForHitStopCoroutine());
        }

        private void PerformBlock(AttackData attack, bool blockCheck = false)
        {
            // On Enter of block
            _blockAction.Invoke();
            HitAttack = attack;
        }

        public void ReturnToMovementState()
        {
            if (stateMachine.IsMoveState)
                return;
            IState state = stateMachine.GetPreviousMovementState();
            switch (state)
            {
                case StandingState:
                    _returnToStand.Invoke();
                    break;
                case CrouchingState:
                    _returnToCrouch.Invoke();
                    break;
                case JumpingState:
                    _returnToJump.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void CharacterMove()
        {
            float speed = characterData.GetMovementSpeed();
            MovementDirectionX = GetInputDirection().x;
            Vector3 destination = new(GetInputDirection().x, 0);

            if (GetInputDirection().x != 0)
            {
                if (IsAgainstTheWall && Mathf.Sign(destination.x) == WallFaceDirection)
                    destination.x = 0;
                if (GetInputDirection().x != FaceDir)
                {
                    ChangeMovementState(GetCharacterAnimationsData().standingClips[2]);
                    speed = characterData.GetMovementSpeed() / Managers.Instance.GameManager.GetCombatValues().GetBackWalkReduction();
                    isRunning = false;
                }

                else
                {
                    int moveId = isRunning ? 3 : 1;
                    ChangeMovementState(GetCharacterAnimationsData().standingClips[moveId]);
                    speed = isRunning ? characterData.GetRunSpeed() : characterData.GetMovementSpeed();
                }
            }
            else
            {
                ChangeMovementState(GetCharacterAnimationsData().standingClips.FirstOrDefault());
                isRunning = false;
            }


            transform.position += (speed * Time.fixedDeltaTime * destination);

        }

        public void ConsumeBarrier()
        {
            superMeter -= (int)MathF.Floor(1 * Time.deltaTime);
        }

        public void CheckAndFlipCharacterModel()
        {
            if (xDiff < 0)
            {
                FaceDir = 1;
                if (render != null)
                    render.flipX = false;

            }
            else
            {
                FaceDir = -1;
                if (render != null)
                    render.flipX = true;
            }
            vfx.flipX = render.flipX;
            model3D.transform.localScale = new Vector3(Mathf.Abs(model3D.transform.localScale.x) * FaceDir, model3D.transform.localScale.y, model3D.transform.localScale.z);
            model3D.transform.localRotation = new Quaternion(model3D.transform.localRotation.x, Mathf.Abs(model3D.transform.localRotation.y) * FaceDir, model3D.transform.localRotation.z, model3D.transform.localRotation.w);
        }

        public void OnAnimationEnd()
        {
            _onAnimationEnd.Invoke();
            ReturnToMovementState();
            characterAnimation.OnActionAnimationEnd();
            isKnockedDown = false;
            isHardKnockDown = false;
            HitAttack = null;
        }

        public void ResetPlayer()
        {
            ResetPos();
            CurrentHealth = GetMaxHealth();
        }
        #endregion
        #region AnimEvents
        //Anim Event
        public void SpawnProjectile()
        {
            if (onGoingAttack.GetProjectileData() == null || currentProjectile != null)
            {
                return;
            }
            currentProjectile = Instantiate(Managers.Instance.GameManager.GetCombatValues().GetProjectile(), this.transform);
            currentProjectile.Initialize(this, onGoingAttack.GetProjectileData());
        }

        public void ChangeMovementState(AnimationData animationData)
        {
            GetCharacterAnimation().ChangeMovementState(animationData.AnimationClip());
            PrepareAnimation(animationData);
        }

        public void QueueMovementState(AnimationData animationData)
        {
            GetCharacterAnimation().QueueMovementState(animationData.AnimationClip());
            PrepareAnimation(animationData);
        }

        public void PlayActionAnimation(AnimationData animationData, float duration = 0)
        {
            GetCharacterAnimation().PlayActionAnimation(animationData.AnimationClip(), duration);
            PrepareAnimation(animationData);
        }

        public void PlayHitAnimation(AnimationData animationData, float duration = 0)
        {
            GetCharacterAnimation().PlayHitAnimation(animationData.AnimationClip(), duration);
            PrepareAnimation(animationData);
        }

        void PrepareAnimation(AnimationData animationData)
        {
            if (currentAnimation = animationData)
            {
                return;
            }
            currentFrame = 0;
            currentAnimation = animationData;
        }

        public void AnimationMovement()
        {
            if (onGoingAttack == null)
            {
                return;
            }
            Vector2 direction = onGoingAttack.GetMovementDirection();
            if (direction == null)
                return;
            {
                if (IsAgainstTheWall && Mathf.Sign(direction.x) == WallFaceDirection)
                    direction.x = 0;

                PosY = direction.y;
                if (PosY > 0)
                {
                    SetIsGrounded(false);
                    SetApplyGravity(false);
                }
            }

            if (currentMovementCoroutine != null)
                StopCoroutine(currentMovementCoroutine);

            currentMovementCoroutine = StartCoroutine(ForceCoroutine(new Vector2(direction.x * FaceDir, direction.y), 100, false));
        }

        public void AnimationMovementEnd()
        {
            if (currentMovementCoroutine == null)
                return;
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
            if (opponent.GetCurrentState() is JumpingState)
                return;
            Vector2 dir = new(direction.x, 0f);
            opponent.ApplyForce(dir, duration, true);
        }

        public void ApplyForce(Vector2 direction, float duration, bool counterforce = false)
        {
            bool m_bool = false;
            if (counterforce)
            {
                PosY = 0;
                m_bool = counterforce;
            }
            else
            {
                if (IsAgainstTheWall && Mathf.Sign(direction.x) == WallFaceDirection)
                    direction.x = 0;

                PosY = direction.y;
                if (PosY > 0)
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
            if (!IsApplyingGravity)
                return;
            ChangeMovementState(GetCharacterAnimationsData().jumpingClips.FirstOrDefault());
            if (!IsGrounded && currentMovementCoroutine == null)
                SetIsJumping(false);
            if ((IsAgainstTheWall && Mathf.Sign(WallFaceDirection) == WallFaceDirection))
                transform.Translate((gravity) * Time.fixedDeltaTime * new Vector2(0, -1));
            else
            {
                transform.Translate((gravity) * Time.fixedDeltaTime * new Vector2(MovementDirectionX, -1));
            }
        }

        public IEnumerator ForceCoroutine(Vector2 direction, float duration, bool counterForce)
        {
            float i = 0f;
            while (i != duration)
            {
                while (currentHitstopCoroutine != null)
                {
                    yield return null;
                }
                MovementDirectionX = direction.x;
                if (!counterForce)
                {
                    if (IsAgainstTheWall && Mathf.Sign(direction.x) == WallFaceDirection)
                        direction.x = 0;
                }
                transform.Translate(gravity * Time.fixedDeltaTime * direction);
                yield return new FrameWait(1);
                i++;
                forceLeftOver = duration - i;
            }
            currentMovementCoroutine = null;
        }

        public IEnumerator WaitForHitStopCoroutine()
        {
            bool wasApplyingGravity = IsApplyingGravity;
            //int target = hitstop;
            int target = Managers.Instance.GameManager.GetCombatValues().GetHitstopBase();
            for (int hitstopframe = 0; hitstopframe < target; hitstopframe++)
            {
                if (hitstopframe == 1)
                {
                    characterAnimation.PauseActionPlayabe();
                    if (wasApplyingGravity)
                        SetApplyGravity(false);
                }
                yield return new FrameWait(1);
                hitstop--;
            }
            characterAnimation.ResumeActionPlayable();
            //Followup of grabs or other attacks
            if (CurrentCombo.Count != 0 && CurrentCombo.Last().GetFollowUpAttackData() != null)
            {
                attackManager.ProcessAttack(CurrentCombo.Last().GetFollowUpAttackData(), true);
            }
            currentHitstopCoroutine = null;
        }

        public IEnumerator JumpCoroutine()
        {
            PlayActionAnimation(GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
            PlayActionAnimation(GetCharacterAnimationsData().jumpingClips.FirstOrDefault());
            float jumpPower = GetJumpPower();

            yield return new FrameWait(jumpStartup);
            // SuperJump
            if (StoredMotionInput == MotionInputs.du)
            {
                jumpPower = jumpPower * Managers.Instance.GameManager.GetCombatValues().GetJumpMultiplier();
            }
            ApplyForce(new Vector2(GetInputDirection().x, 1f), jumpPower);
            StoredMotionInput = MotionInputs.NONE;
        }
        #endregion

        public void BoxCollisionedWith(Collider2D collider)
        {
            if (collider == GetComponent<Collider2D>())
                return;
        }

        public void HitboxesEnabled()
        {
            visualState = !visualState;
        }

        public void HitConnect(AttackData attack)
        {
            if (currentHitstopCoroutine != null)
                StopCoroutine(currentHitstopCoroutine);

            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            hitstop = attackLevel + Managers.Instance.GameManager.GetCombatValues().GetHitstopBase();
            StartHitstopCoroutine();

            if (opponent.GetCurrentState() is BlockState)
            {
                CurrentCombo.Clear();
            }
            else
            {
                CurrentCombo.Add(attack);
                Managers.Instance.GameManager.UpdateComboCounter(playerId);

            }
            SameAttackSequence = true;
        }

        public void ResetAttackInfo()
        {
            CurrentCombo.Clear();
            Managers.Instance.GameManager.UpdateComboCounter(playerId);
        }

        bool IsBlocking(AttackData attack)
        {
            if (GetCurrentState() is HitState)
                return false;
            AttackAttribute attribute = attack.GetAttackAttribute();
            switch (GetCurrentState())
            {
                case StandingState:
                    if (attribute == AttackAttribute.Low)
                        return false;
                    break;
                case CrouchingState:
                    if (attribute == AttackAttribute.High)
                        return false;
                    break;
                case JumpingState:
                    if (attack.GetAttackState() != States.Jumping)
                        return false;
                    break;
            }
            if (inputHandler.GetDirection().x == -FaceDir || GetCurrentState() is BlockState)
                return true;
            return false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            switch (LayerMask.LayerToName(collision.gameObject.layer))
            {
                case ("Pushbox"):
                    if (IsAgainstTheWall && !IsGrounded)
                    {
                        if (!opponent.IsAgainstTheWall || Managers.Instance.GameManager.CornerPlayer == this)
                        {
                            opponent.transform.Translate(new Vector2(FaceDir, 0) * Managers.Instance.GameManager.GetCombatValues().GetPushMultiplier() * Time.fixedDeltaTime);
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
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Pushbox")
            {
                if (Managers.Instance.GameManager.CornerPlayer == opponent && !IsAgainstTheWall)
                {
                    transform.Translate(new Vector2(-FaceDir, 0) * Managers.Instance.GameManager.GetCombatValues().GetPushMultiplier() * Time.fixedDeltaTime);
                }
            }
        }

        public void EnableInput()
        {
            inputHandler.EnableInput();
        }

        public void DisableInput()
        {
            inputHandler.DisableInput();
        }

        public void SwitchCurrentActionMap(string mapName)
        {
            inputHandler.PlayerInput.SwitchCurrentActionMap(mapName);
        }

        void OnDestroy()
        {
            inputHandler.UnmapActions();
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
