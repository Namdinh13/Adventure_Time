using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerContext
{
    private CombatMode combatMode = CombatMode.Unarmed;
    private LocomotionMode lastLocomotionMode = LocomotionMode.Idle;
    private RuntimeAnimatorController baseController;
    private InputSystem_Actions inputActions;
    private CharacterController characterController;
    private Animator animator;
    private CharacterStateMachine stateMachine;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform modelHolder;
    [SerializeField] private AnimatorOverrideController swordOverride;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float maxJumpHeight = 2f;
    [SerializeField] private float maxJumpTime = 0.7f;
    [SerializeField] private float fallMultiplier = 3.0f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private float groundedRememberTime = 0.15f;
   
    [Header("Combat")]
    [SerializeField] private GameObject swordTrailVFX;

    [Header("Lock On")]
    [SerializeField] private bool lockedOn;
    [SerializeField] private Transform currentTarget;
   

    [Header("Hitbox")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private WeaponHitbox weaponHitbox;

    [Header("ChangeSwordSocket")]
    [SerializeField] private Transform sword;
    [SerializeField] private Transform swordBackSocket;
    [SerializeField] private Transform swordHandSocket;

    private Vector2 moveInput;
    private Vector3 currentMoveDirection;

    private int comboStep;

    private float currentSpeed;
    private float speedVelocity;
    private float verticalVelocity;
    private float jumpPressedRemember;
    private float gravity = -9.8f;
    private float groundedGravity = -5f;
    private float initialJumpVelocity;
    private float lastToggleCombatTime;
    private float groundedRemember;

    private bool isRunning;
    private bool isGrounded;
    private bool isJumpPressed;
    private bool isJumping;
    private bool isAttacking;
    private bool attackPressed;
    private bool isHit;
    private bool isDrawingWeapon;
    private bool isSheathingWeapon;
    private bool togglePressed;
    private bool isAttackHeld;
    private bool dodgePressed;
    private bool isDodging;
    private bool isInvulnerable;
    private bool isDead;
    private bool isWeaponEquipped;
    private bool isStopping;
    private bool useRootMotion;

    public void ConsumeToggle() => togglePressed = false;

    public int ComboStep => comboStep;

    public float VerticalVelocity => verticalVelocity;
    public float CurrentSpeed => currentSpeed;

    public bool IsGrounded => isGrounded;
    public bool IsJumping => isJumping;
    public bool IsAttacking => isAttacking;
    public bool AttackPressed => attackPressed;
    public bool IsAttackHeld => isAttackHeld;
    public bool IsHit => isHit;
    public bool HasJumpBuffered => jumpPressedRemember > 0f;
    public bool LockedOn => lockedOn;
    public bool DodgePressed => dodgePressed;
    public bool IsDodging => isDodging;
    public bool IsInvulnerable => isInvulnerable;
    public bool IsDrawingWeapon => isDrawingWeapon;
    public bool IsSheathingWeapon => isSheathingWeapon;
    public bool WeaponEquipped => isWeaponEquipped;
    public bool TogglePressed => togglePressed;
    public bool IsDead => isDead;
    public bool IsMoving => currentSpeed > 0.1f;
    public bool IsRunning => isRunning;
    public bool IsStopping => isStopping;

    public Transform CurrentTarget => currentTarget;
    public Transform ModelHolder => modelHolder;
    public Transform PlayerTransform => transform;
    public Vector3 CurrentMoveDirection => currentMoveDirection;
    public CombatMode CombatMode => combatMode;
    public LocomotionMode LastLocomotionMode => lastLocomotionMode;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        baseController = animator.runtimeAnimatorController;

        inputActions = new InputSystem_Actions();

        SetupJumpVariables();

        SetupStateMachine();

        swordTrailVFX.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;

        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Attack.canceled += OnAttack;

        inputActions.Player.Dodge.performed += OnDodge;

        inputActions.Player.ToggleCombat.performed += OnToggleCombat;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Jump.canceled -= OnJump;

        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;

        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Attack.canceled -= OnAttack;

        inputActions.Player.Dodge.performed -= OnDodge;

        inputActions.Player.ToggleCombat.performed -= OnToggleCombat;

        inputActions.Disable();
    }

    private void Update()
    {

        jumpPressedRemember -= Time.deltaTime;
        GroundedCheck();
        stateMachine.Update();
    }

    #region State Machine Setup
    private void SetupStateMachine()
    {
        stateMachine = new CharacterStateMachine();

        var locomotionState = new LocomotionState(this, animator);
        var jumpState = new JumpState(this, animator);
        var attackState = new AttackState(this, animator);
        var hitState = new HitState(this, animator);
        var dodgeState = new DodgeState(this, animator);
        var drawState = new DrawWeaponState(this, animator);
        var sheatheState = new SheatheWeaponState(this, animator);
        var deathState = new DeathState(this, animator);
        var stoppingState = new StoppingState(this, animator);

        At(locomotionState, stoppingState, new FuncPredicate(() => CurrentMoveDirection.sqrMagnitude < 0.01f && IsMoving && !LockedOn));
        At(stoppingState, locomotionState, new FuncPredicate(() => !IsStopping));

        At(locomotionState, drawState, new FuncPredicate(() => TogglePressed && !WeaponEquipped));
        At(locomotionState, sheatheState, new FuncPredicate(() => TogglePressed && WeaponEquipped));
        At(drawState, locomotionState, new FuncPredicate(() => !IsDrawingWeapon));
        At(sheatheState, locomotionState, new FuncPredicate(() => !IsSheathingWeapon));

        At(locomotionState, jumpState, new FuncPredicate(() => HasJumpBuffered == true && IsGrounded == true));
        At(jumpState, locomotionState, new FuncPredicate(() => IsGrounded == true && VerticalVelocity <= 0f && !AttackPressed));

        At(locomotionState, attackState, new FuncPredicate(() => AttackPressed == true));
        At(attackState, locomotionState, new FuncPredicate(() => !IsAttacking));

        At(locomotionState, dodgeState, new FuncPredicate(() => DodgePressed == true && IsGrounded == true));
        At(dodgeState, locomotionState, new FuncPredicate(() => !IsDodging));

        stateMachine.AddAnyTransition(hitState, new FuncPredicate(() => IsHit == true));
        At(hitState, locomotionState, new FuncPredicate(() => !IsHit));

        stateMachine.AddAnyTransition(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(locomotionState);
    }

    private void At(IState from, IState to, IPredicate condition)
    {
        stateMachine.AddTransition(from, to, condition);
    }
    #endregion

    #region Movement Methods
    public void Move()
    {

        float targetSpeed = (isRunning ? movementSpeed * 2f : movementSpeed) * moveInput.magnitude;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothTime);

        if (moveInput.magnitude > 0.1f)
        {
            lastLocomotionMode = isRunning ? LocomotionMode.Run : LocomotionMode.Walk;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection;

        if (lockedOn == true && currentTarget != null)
        {
            Vector3 toTarget = (currentTarget.position - transform.position).normalized;
            toTarget.y = 0f;

            Vector3 strafeRight = Vector3.Cross(Vector3.up, toTarget).normalized;

            moveDirection = (toTarget * moveInput.y + strafeRight * moveInput.x).normalized;

            HandleLockRotation();
        }
        else
        {
            moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
            HandleRotation(moveDirection);
        }

        currentMoveDirection = moveDirection;

        Vector3 horizontalMove = moveDirection * currentSpeed;
        Vector3 finalMove = horizontalMove;

        finalMove.y = verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);

        UpdateAnimator();
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void HandleLockRotation()
    {
        if (currentTarget == null) 
            return;

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Debug.Log("LOCK ROTATION");
    }

    public void SetStopping(bool value) => isStopping = value;
    #endregion

    #region Jump Methods
    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;

        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);

        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    public void ApplyGravity()
    {
        if (isGrounded)
        {
            verticalVelocity = groundedGravity;
            return;
        }

        bool isFalling = verticalVelocity <= 0f || !isJumpPressed;

        if (isFalling)
        {
            verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (verticalVelocity < -30f)
        {
            verticalVelocity = -30f;
        }
    }

    public void ApplyJump()
    {
        if (!isGrounded || isJumping) return;

        isJumping = true;

        isJumpPressed = true;

        groundedRemember = 0f;

        verticalVelocity = initialJumpVelocity;

        isGrounded = false;
    }

    public void ResetJumpState()
    {
        if (isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;

            isJumpPressed = false;
        }
    }

    public void ConsumeJumpBuffer()
    {
        jumpPressedRemember = 0f;
    }
    private void GroundedCheck()
    {
        bool wasGrounded = isGrounded;

        bool rawGrounded = characterController.isGrounded;

        if (verticalVelocity > 1f)
        {
            rawGrounded = false;
        }

        if (rawGrounded)
        {
            groundedRemember = groundedRememberTime;
        }
        else
        {
            groundedRemember -= Time.deltaTime;
        }

        isGrounded = groundedRemember > 0f;

        animator.SetBool("Grounded", isGrounded);

        if (!wasGrounded && isGrounded)
        {
            animator.SetBool("Falling", false);

            if (verticalVelocity < 0f)
            {
                verticalVelocity = groundedGravity;
            }
        }

        if (wasGrounded && !isGrounded)
        {
            animator.SetBool("Falling", true);
        }
    }


    #endregion

    #region Dodge Methods
    public void DodgeMove(Vector3 direction, float speed)
    {
        Vector3 horizontal = direction * speed;

        Vector3 finalMove = horizontal;

        finalMove.y = verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);

        UpdateAnimator();
    }

    public void ConsumeDodge()
    {
        dodgePressed = false;
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public void SetDodging(bool value)
    {
        isDodging = value;
    }
    public void ResetModelRotation()
    {
        modelHolder.localRotation = Quaternion.identity;
    }
    #endregion

    #region Attack Methods
    public void StartAttack()
    {
        isAttacking = true;
    }

    public void StopAttack()
    {
        isAttacking = false;
    }

    public void ConsumeAttack()
    {
        attackPressed = false;
    }

    public void NextCombo()
    {
        comboStep++;

        if (comboStep > 4)
        {
            ResetCombo();
        }
    }

    public void ResetCombo()
    {
        comboStep = 0;
    }

    private void ApplyCombatOverride()
    {
        switch (combatMode)
        {
            case CombatMode.Unarmed:
                animator.runtimeAnimatorController = baseController;
                break;

            case CombatMode.Sword:
                animator.runtimeAnimatorController = swordOverride;
                break;
        }
    }

    public void EnableWeapon()
    {
        weaponHitbox.ResetHitTargets();

        weaponCollider.enabled = true;

        swordTrailVFX.SetActive(true);

        SoundManager.Instance.PlaySound2D("SwordSwing");
    }

    public void DisableWeapon()
    {
        weaponCollider.enabled = false;

        swordTrailVFX.SetActive(false);
    }

    public void EquipSwordToHand()
    {

        sword.SetParent(swordHandSocket, false);

        sword.localPosition = Vector3.zero;

        sword.localRotation = Quaternion.identity;
    }

    public void EquipSwordToBack()
    {

        sword.SetParent(swordBackSocket, false);

        sword.localPosition = Vector3.zero;

        sword.localRotation = Quaternion.identity;
    }

    public void SetCombatMode(CombatMode mode)
    {
        combatMode = mode;

        ApplyCombatOverride();
    }

    public void SetDrawingWeapon(bool value)
    {
        isDrawingWeapon = value;

        if (value)
        {
            isWeaponEquipped = true; 
        }
    }

    public void SetSheathingWeapon(bool value)
    {
        isSheathingWeapon = value;

        if (!value)
        {
            isWeaponEquipped = false;  
        }
    }

    public void SetRootMotion(bool value)
    {
        useRootMotion = value;
    } 

    #endregion

    #region Hit Methods
    public void StartHit()
    {
        isHit = true;

        isAttacking = false;

        ResetCombo();
    }

    public void StopHit()
    {
        isHit = false;
    }
    #endregion

    #region Death Methods
    public void StartDeath()
    {
        isDead = true;
    }

    public void TriggerGameOver()
    {
        //GameOverUI.Instance.Show(); 
    }
    #endregion
    public void SetLockTarget(Transform target)
    {
        currentTarget = target;

        lockedOn = target != null;
    }

    private void UpdateAnimator()
    {

        if (isAttacking) return;

        float normalizedSpeed = currentSpeed / (movementSpeed * 2f);

        animator.SetFloat("Speed", normalizedSpeed);

        animator.SetBool("Falling", !isGrounded && verticalVelocity < -0.1f);

        animator.SetFloat("MoveX", moveInput.x, 0.1f, Time.deltaTime);

        animator.SetFloat("MoveY", moveInput.y, 0.1f, Time.deltaTime);
    }

    #region Actions Events
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        if (moveInput.magnitude < 0.1f)
        {
            isRunning = false;
        }
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            jumpPressedRemember = jumpBufferTime;

            isJumpPressed = true;
        }
        else if (ctx.canceled)
        {
            isJumpPressed = false;
        }
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        if (lockedOn || isDead) return;

        isRunning = ctx.performed;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (isDead) return;

        if (ctx.performed)
        {
            attackPressed = true;

            isAttackHeld = true;
        }
        else if (ctx.canceled)
        {
            isAttackHeld = false;
        }
    }

    private void OnDodge(InputAction.CallbackContext ctx)
    {
        if (isDead) return;

        dodgePressed = true;
    }

    private void OnToggleCombat(InputAction.CallbackContext ctx)
    {
        if (lockedOn || isDead) return;

        if (Time.time - lastToggleCombatTime < 0.2f) return;

        lastToggleCombatTime = Time.time;

        togglePressed = true;
    }

    private void OnAnimatorMove()
    {
        if (useRootMotion)
        {
            Vector3 rootDelta = animator.deltaPosition;

            rootDelta.y = verticalVelocity * Time.deltaTime;

            characterController.Move(rootDelta);
        }
    }

    #endregion
}
