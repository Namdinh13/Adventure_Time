using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    private const string speedParamName = "Speed";
    private const string groundedParamName = "Grounded";
    private const string fallingParamName = "Falling";

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float maxJumpHeight = 2f;
    [SerializeField] private float maxJumpTime = 0.7f;
    [SerializeField] private float fallMultiplier = 3.0f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float groundedRememberTime = 0.15f;
    private float groundedRemember;

    [Header("Combat")]
    [SerializeField] private float comboResetTime = 1.5f;

    [Header("VFX Slash")]
    [SerializeField] private GameObject slashVFX;
    [SerializeField] private Transform slashPoint1;
    [SerializeField] private Transform slashPoint2;

    [Header("Lock On")]
    [SerializeField] private bool lockedOn;
    [SerializeField] private Transform currentTarget;

    [Header("Hitbox")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private WeaponHitbox weaponHitbox;

    private CharacterController characterController;
    private Animator animator;

    private Vector2 moveInput;

    private float currentSpeed;
    private float speedVelocity;
    private float verticalVelocity;
    private bool isRunning;
    private bool isGrounded;

    private float jumpPressedRemember;
    private float gravity = -9.8f;
    private float groundedGravity = -5f;
    private float initialJumpVelocity;
    private bool isJumpPressed = false;
    private bool isJumping = false;

    private bool isAttacking;
    private bool attackPressed;
    private int comboStep;
    private float lastAttackTime;
    private bool isHit;

    private CharacterStateMachine stateMachine;

    public bool IsGrounded => isGrounded;

    public bool IsJumping => isJumping;

    public float VerticalVelocity => verticalVelocity;

    public bool IsAttacking => isAttacking;

    public bool AttackPressed => attackPressed;

    public int ComboStep => comboStep;

    public bool LockedOn => lockedOn;

    public bool IsHit => isHit;

    public Transform CurrentTarget => currentTarget;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

        SetupJumpVariables();

        SetupStateMachine();
    }

    private void Update()
    {
        jumpPressedRemember -= Time.deltaTime;

        stateMachine.Update();

        GroundedCheck();

        if (jumpPressedRemember > 0f && isGrounded && !isJumping)
        {
            Jump();
            jumpPressedRemember = 0f;
        }

        HandleGravity();

        HandleJumpReset();

        //Debug.Log($"Grounded: {isGrounded} | Velocity: {verticalVelocity}");
    }

    private void SetupStateMachine()
    {
        stateMachine = new CharacterStateMachine();

        var locomotionState = new LocomotionState(this, animator);

        var jumpState = new JumpState(this, animator);

        var attackState = new AttackState(this, animator);

        var hitState = new HitState(this, animator);

        // Transitions JumpState
        At(locomotionState, jumpState, new FuncPredicate(() => IsJumping));

        At(jumpState,locomotionState, new FuncPredicate(() => IsGrounded && VerticalVelocity <= 0f));

        // Transitions AttackState
        At(locomotionState, attackState, new FuncPredicate(() => AttackPressed));

        At(attackState, locomotionState, new FuncPredicate(() => !IsAttacking));

        // Transitions HitState
        At(locomotionState, hitState, new FuncPredicate(() => IsHit));

        At(attackState, hitState, new FuncPredicate(() => IsHit));

        At(hitState, locomotionState, new FuncPredicate(() => !IsHit));

        stateMachine.SetState(locomotionState);
    }

    private void At(IState from, IState to, IPredicate condition)
    {
        stateMachine.AddTransition(from, to, condition);
    }

    public void Move()
    {
        float targetSpeed = (isRunning ? movementSpeed * 2f : movementSpeed) * moveInput.magnitude;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothTime);

        Vector3 forward = cameraTransform.forward;

        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection;

        if (lockedOn && currentTarget != null)
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

        Vector3 horizontalMove = moveDirection * currentSpeed;

        //HandleGravity();

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

    private void HandleLockRotation()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void HandleGravity()
    {
        if (isGrounded)
        {
            verticalVelocity = groundedGravity; 
            return;
        }

        bool isFalling = verticalVelocity <= 0.0f || !isJumpPressed;

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

    public void Jump()
    {
        if (!isGrounded || isJumping) return;

        isJumping = true;

        isJumpPressed = true;

        groundedRemember = 0f;

        verticalVelocity = initialJumpVelocity;

        isGrounded = false;

        //stateMachine.Update();
    }

    private void HandleJumpReset()
    {
        if (isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
            isJumpPressed = false;
        }
    }

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

        if (comboStep > 2)
        {
            comboStep = 0;
        }
    }

    public void ResetCombo()
    {
        comboStep = 0;
    }

    public void EnableWeapon()
    {
        weaponHitbox.ResetHitTargets();
        weaponCollider.enabled = true;
    }

    public void DisableWeapon()
    {
        weaponCollider.enabled = false;
    }

    public void SetLockTarget(Transform target)
    {
        currentTarget = target;

        lockedOn = target != null;

        animator.SetBool("LockedOn", lockedOn);
    }

    public void SpawnSlash1()
    {
        Instantiate(slashVFX, slashPoint1.position, slashPoint1.rotation);
    }

    public void SpawnSlash2()
    {
        Instantiate(slashVFX, slashPoint2.position, slashPoint2.rotation);
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

        animator.SetBool(groundedParamName, isGrounded);

        if (!wasGrounded && isGrounded)
        {
            animator.SetBool(fallingParamName, false);

            if (verticalVelocity < 0f)
            {
                verticalVelocity = groundedGravity;
            }
        }

        if (wasGrounded && !isGrounded)
        {
            animator.SetBool(fallingParamName, true);
        }
    }

    private void UpdateAnimator()
    {
        if (isAttacking) return;

        float normalizedSpeed = currentSpeed / (movementSpeed * 2f);

        animator.SetFloat(speedParamName, normalizedSpeed);

        animator.SetBool(fallingParamName, !isGrounded && verticalVelocity < -0.1f);

        animator.SetFloat("MoveX", moveInput.x, 0.1f, Time.deltaTime);

        animator.SetFloat("MoveY", moveInput.y, 0.1f, Time.deltaTime);
    }

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

    private void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();

        if (moveInput.magnitude < 0.1f)
        {
            isRunning = false;
        }
    }

    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            jumpPressedRemember = jumpBufferTime;
            isJumpPressed = true;
        }
        else
        {
            isJumpPressed = false;
        }
    }

    private void OnSprint(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            isRunning = !isRunning;
        }
    }

    private void OnAttack()
    {
        attackPressed = true;

        if (
            Time.time - lastAttackTime >
            comboResetTime
        )
        {
            comboStep = 0;
        }

        lastAttackTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}