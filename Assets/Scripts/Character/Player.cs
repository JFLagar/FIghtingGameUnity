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
    public class Player : MonoBehaviour, IPhysics, IHitboxResponder
    {
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

        StateMachine stateMachine;
        InputHandler inputHandler;

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

        [Space]
        [SerializeField]
        int wakingUpFrames = 6;
        [SerializeField]
        int jumpStartup = 4;
        [SerializeField]
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

        private MotionInputs storedMotionInput = MotionInputs.NONE;
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

        public void Initialize()
        {
            characterModel = Instantiate(characterData.GetCharacterModel(), model3D.transform);
            characterModel.Initialize(this);
            animator = characterModel.GetComponent<Animator>();
            collisions = characterModel.GetCollisions();
            characterAnimation.Initialize(this, animator);
            inputHandler = new InputHandler();
            inputHandler.Initialize(this);
            stateMachine = new StateMachine();
            stateMachine.Initialize(this);
            attackManager.Initialize(this, characterModel.GetHitboxes());
            gravity = characterData.GetGravity();
            CurrentCombo = new List<AttackData>();
            IsGrounded = true;
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
            stateMachine.Update();
            inputHandler.Update();
            if (opponent == null)
                return;

            xDiff = transform.position.x - opponent.transform.position.x;

            if (GetCurrentState() != States.Jumping && GetCurrentActionState() == ActionStates.None)
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

            if (GetCurrentActionState() == ActionStates.Hit)
            {
                if (currentProjectile != null)
                {
                    DestroyImmediate(currentProjectile.gameObject);
                }
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
                    case ActionStates.Attack:
                        render.color = stateColors[2];
                        break;
                }
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
                animationClips.Add(attack.GetAnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip());
            }
            foreach (var attack in characterData.GetCrouchingAttacks())
            {
                animationClips.Add(attack.GetAnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip());
            }
            foreach (var attack in characterData.GetJumpAttacks())
            {
                animationClips.Add(attack.GetAnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip());
            }
            foreach (var attack in characterData.GetSpecialAttacks())
            {
                animationClips.Add(attack.GetAnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip());
            }
            foreach (var attack in characterData.GetForwardAttacks())
            {
                animationClips.Add(attack.GetAnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip());
            }
            foreach (var attack in characterData.GetGrabData())
            {
                animationClips.Add(attack.GetAnimationClip());
                if (attack.GetFollowUpAttackData() != null)
                    animationClips.Add(attack.GetFollowUpAttackData().GetAnimationClip());
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
            return inputHandler.WasYReleased;
        }

        public void SetDoubleJump(bool value)
        {
            CanDoubleJump = value;
        }

        public States GetCurrentState()
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
            if (GetCurrentActionState() != ActionStates.None)
            {
                if (onGoingAttack != null && onGoingAttack.GetCancelTypes().ToList().Contains(CancelTypes.Jump) && isAnyHitboxOpen && opponent.GetCurrentActionState() == ActionStates.Hit)
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
                if (IsGrounded && GetCurrentActionState() == ActionStates.Landing)
                    return true;
                if (onGoingAttack != null && onGoingAttack.GetCancelTypes().ToList().Contains(CancelTypes.Dash) && opponent.GetCurrentActionState() == ActionStates.Hit && GetInputDirection().x == FaceDir)
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
            if (GetInputDirection().x == FaceDir)
            {
                if (GetCurrentActionState() == ActionStates.Attack)
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().jumpingClips[2], Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                    ApplyForce(new Vector2(FaceDir * Managers.Instance.GameManager.GetCombatValues().GetDashMultiplier(), 0f),
                        Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                    return;
                }
                if (GetCurrentState() == States.Standing)
                {
                    isRunning = true;
                }
                else if ((GetCurrentState() == States.Jumping) && (AirActions > 0))
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().jumpingClips[2], Managers.Instance.GameManager.GetCombatValues().GetAirDashAnimationDuration());
                    ApplyForce(new Vector2(FaceDir * Managers.Instance.GameManager.GetCombatValues().GetDashMultiplier(), 0.1f),
                        Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                    AirActions--;
                }

            }
            else
            {

                if (GetCurrentState() == States.Standing)
                {
                    SetActionState(ActionStates.Landing);
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().standingClips.LastOrDefault(), Managers.Instance.GameManager.GetCombatValues().GetAirDashAnimationDuration());
                    ApplyForce(new Vector2(-FaceDir * 2, 0.5f), Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                }
                else if ((GetCurrentState() == States.Jumping) && (AirActions > 0))
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().standingClips.LastOrDefault(), Managers.Instance.GameManager.GetCombatValues().GetAirDashAnimationDuration());
                    ApplyForce(new Vector2(-FaceDir, 0.1f), Managers.Instance.GameManager.GetCombatValues().GetDashDuration());
                    AirActions--;
                }
            }
        }

        public void PerformJump()
        {
            //SetActionState(ActionStates.None);
            if (GetCurrentState() == States.Jumping)
            {
                if (AirActions > 0)
                    AirActions--;
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
            if (superMeter > 0 && type == InputType.MH && GetInputDirection().x != FaceDir)
            {
                Debug.Log("Barrier");
                superMeter--;
                return;
            }

            if (superMeter >= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter() && type == InputType.MH)
            {
                Debug.Log("GuardBreak");
                superMeter -= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter();
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
            if (superMeter >= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter() && type == InputType.LMH)
            {
                Debug.Log("Rapid");
                superMeter -= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter();
                return;
            }
            if (superMeter >= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter() && type == InputType.MH)
            {
                Debug.Log("GuardBreak");
                superMeter -= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter();
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
                superMeter--;
                return;
            }

            if (superMeter >= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter() && type == InputType.MH && GetInputDirection().x == FaceDir)
            {
                Debug.Log("AttackBreak");
                superMeter -= Managers.Instance.GameManager.GetCombatValues().GetHalfMeter();
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
                    if (GetCurrentState() != States.Jumping)
                    {
                        attackManager.Attack(characterData.FindSpecialAttack(storedMotionInput, type));
                        return;
                    }
                    else 
                    {
                        attackManager.Attack(characterData.FindSpecialAttack(storedMotionInput, type, true));
                    }
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
                                attackData = inputHandler.GetDirection().x == FaceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetStandingAttacks()[((int)type)];
                                break;
                            case 1f:
                                attackData = characterData.GetJumpAttacks()[((int)type)];
                                break;
                            case -1f:
                                attackData = inputHandler.GetDirection().x == FaceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetCrouchingAttacks()[((int)type)];
                                break;
                        }

                    }
                    break;

                case States.Crouching:
                    switch (inputHandler.GetDirection().y)
                    {
                        case 0f:
                            attackData = inputHandler.GetDirection().x == FaceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetStandingAttacks()[((int)type)];
                            break;
                        case -1f:
                            attackData = inputHandler.GetDirection().x == FaceDir ? characterData.GetForwardAttacks()[((int)type)] : characterData.GetCrouchingAttacks()[((int)type)];
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
        }

        public void Attack(AttackData attackData)
        {
            onGoingAttack = attackData;
            SetActionState(ActionStates.Attack);
        }

        public void HurtboxOnCollision(AttackData attack, bool blockCheck = false)
        {
            if (GetCurrentActionState() == ActionStates.Attack)
            {
                PerformGettingHit(attack);
                return;
            }
            if (attack.IsGrab())
            {
                if (GetCurrentState() == States.Jumping || GetCurrentActionState() == ActionStates.Hit)
                    return;
                PerformGettingHit(attack);
                return;
            }

            if (IsBlocking(attack.GetAttackAttribute()))
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
            Vector2 dir = CalculateHitPush(attack);
            PlaySound(attack.GetCollideAudioClip()); ;
            if (attack.CausesLaunch() || isKnockedDown)
            {
                characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[2], CalculateHitstun(attack));
                isKnockedDown = true;
                if (attack.CausesHardKnockdown())
                {
                    isHardKnockDown = true;
                }
            }
            else
            {
                if (attack.IsGrab())
                {
                    characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[0]);
                }
                else
                {
                    if (GetCurrentState() == States.Jumping)
                    {
                        characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().hitClips[(int)GetCurrentState()], CalculateHitstun(attack));
                    }
                    else
                        characterAnimation.PlayHitAnimation(GetCharacterAnimationsData().hitClips[(int)GetCurrentState()], CalculateHitstun(attack));
                }
            }
            if (currentHitstopCoroutine != null)
                StopCoroutine(currentHitstopCoroutine);

            hitstop = attack.GetAttackLevel() + Managers.Instance.GameManager.GetCombatValues().GetHitstopBase();
            currentHitstopCoroutine = StartCoroutine(WaitForHitStopCoroutine());
            if (CurrentHealth > 0)
            {
                CurrentHealth -= attack.GetDamage();
                Managers.Instance.GameManager.UpdateHealth(playerId, CurrentHealth);
            }
            //if its a projectile dont push back the attacking character
            if (IsAgainstTheWall && FaceDir != WallFaceDirection && attack.GetProjectileData() == null)
            {
                ApplyCounterPush(-dir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
            }
            ApplyForce(dir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
            SetActionState(ActionStates.Hit);
        }

        private void PerformBlock(AttackData attack, bool blockCheck = false)
        {
            PlaySound(attack.GetCollideAudioClip());
            Vector2 dir = CalculateHitPush(attack);
            Vector2 blockDir = new(dir.x, 0);
            SetActionState(ActionStates.Block);

            if (!blockCheck)
                characterAnimation.PlayActionAnimation(GetCharacterAnimationsData().blockingClips[(int)GetCurrentState()]);
            if (IsAgainstTheWall && FaceDir != WallFaceDirection)
            {
                ApplyCounterPush(-blockDir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
            }
            else
                ApplyForce(blockDir, Managers.Instance.GameManager.GetCombatValues().GetHitMovementDuration());
        }

        private int CalculateHitstun(AttackData attack)
        {
            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            int result = (attackLevel * 2) + Managers.Instance.GameManager.GetCombatValues().GetHitstunBase() + attack.GetExtraHitstun(); //attacklevel + hitstunbase(10) + extra
            return result;
        }

        private Vector2 CalculateHitPush(AttackData attack)
        {
            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            Vector2 result = new Vector2();
            result.x = ((attackLevel) + attack.GetExtraPush().x) * -FaceDir;
            if (attack.CausesLaunch() || isKnockedDown || GetCurrentState() == States.Jumping)
            {
                if (result.y == 0)
                {
                    result.y = 1;
                }
                result.y = (attackLevel) + attack.GetExtraPush().y + Managers.Instance.GameManager.GetCombatValues().GetHitVerticalBase();
            }

            return result;
        }

        public void CharacterMove()
        {
            if (GetCurrentActionState() != ActionStates.None || GetCurrentState() != States.Standing)
                return;
            float speed = characterData.GetMovementSpeed();
            MovementDirectionX = GetInputDirection().x;
            Vector3 destination = new(GetInputDirection().x, 0);

            if (GetInputDirection().x != 0)
            {
                if (IsAgainstTheWall && Mathf.Sign(destination.x) == WallFaceDirection)
                    destination.x = 0;
                if (GetInputDirection().x != FaceDir)
                {
                    characterAnimation.ChangeMovementState(GetCharacterAnimationsData().standingClips[2]);
                    speed = characterData.GetMovementSpeed() / Managers.Instance.GameManager.GetCombatValues().GetBackWalkReduction();
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
           {
                opponent.ResetAttackInfo();
                MovementDirectionX = 0;
            }
            characterAnimation.OnActionAnimationEnd();
            SetActionState(ActionStates.None);
            isKnockedDown = false;
            isHardKnockDown = false;
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
            if (opponent.GetCurrentState() == States.Jumping)
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
            characterAnimation.ChangeMovementState(GetCharacterAnimationsData().jumpingClips.FirstOrDefault());
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
                attackManager.Attack(CurrentCombo.Last().GetFollowUpAttackData(), true);
            }
            currentHitstopCoroutine = null;
        }

        public IEnumerator JumpCoroutine()
        {
            SetActionState(ActionStates.Landing);
            GetCharacterAnimation().PlayActionAnimation(GetCharacterAnimationsData().stateTransitionClips.LastOrDefault());
            GetCharacterAnimation().PlayActionAnimation(GetCharacterAnimationsData().jumpingClips.FirstOrDefault());
            float jumpPower = GetJumpPower();
            // SuperJump
            if (storedMotionInput == MotionInputs.du)
            {
                jumpPower = jumpPower * Managers.Instance.GameManager.GetCombatValues().GetJumpMultiplier();
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

        public void HitConnect(AttackData attack)
        {
            if (currentHitstopCoroutine != null)
                StopCoroutine(currentHitstopCoroutine);

            int attackLevel = 1;
            if (attack.GetAttackLevel() != 0)
                attackLevel = attack.GetAttackLevel();
            hitstop = attackLevel + Managers.Instance.GameManager.GetCombatValues().GetHitstopBase();
            currentHitstopCoroutine = StartCoroutine(WaitForHitStopCoroutine());

            if (opponent.GetCurrentActionState() == ActionStates.Block)
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
            if (inputHandler.GetDirection().x == -FaceDir || GetCurrentActionState() == ActionStates.Block)
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
