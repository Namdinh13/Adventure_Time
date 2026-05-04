using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const string speedParamName = "Speed";
    private const string groundedParamName = "Grounded";
    private const string fallingParamName = "Falling";

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Combat")]
    [SerializeField] private float comboResetTime = 1.5f;

    [Header("VFX Slash")]
    [SerializeField] private GameObject slashVFX;
    [SerializeField] private Transform slashPoint1;
    [SerializeField] private Transform slashPoint2;

    private CharacterController characterController;
    private Animator animator;

    private Vector2 moveInput;

    private float currentSpeed;
    private float speedVelocity;
    private float verticalVelocity;

    private bool isGrounded;
    private bool isRunning;
    private bool isJumping;

    // Combat
    private bool isAttacking;
    private bool attackPressed;

    private int comboStep;

    private float lastAttackTime;

    private StateMachine stateMachine;

    // ================= PROPERTIES =================

    public bool IsGrounded => isGrounded;

    public bool IsJumping => isJumping;

    public float VerticalVelocity => verticalVelocity;

    public bool IsAttacking => isAttacking;

    public bool AttackPressed => attackPressed;

    public int ComboStep => comboStep;

    // ================= UNITY =================

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

        SetupStateMachine();
    }

    private void Update()
    {
        GroundedCheck();

        stateMachine.Update();

        HandleJumpReset();
    }

    // ================= STATE MACHINE =================

    private void SetupStateMachine()
    {
        stateMachine = new StateMachine();

        var locomotionState =
            new LocomotionState(this, animator);

        var jumpState =
            new JumpState(this, animator);

        var attackState =
            new AttackState(this, animator);

        // Jump
        At(
            locomotionState,
            jumpState,
            new FuncPredicate(() => IsJumping)
        );

        At(
            jumpState,
            locomotionState,
            new FuncPredicate(
                () =>
                    IsGrounded &&
                    VerticalVelocity <= 0f
            )
        );

        // Attack
        At(
            locomotionState,
            attackState,
            new FuncPredicate(() => AttackPressed)
        );

        At(
            attackState,
            locomotionState,
            new FuncPredicate(() => !IsAttacking)
        );

        stateMachine.SetState(locomotionState);
    }

    private void At(
        IState from,
        IState to,
        IPredicate condition
    )
    {
        stateMachine.AddTransition(
            from,
            to,
            condition
        );
    }

    // ================= MOVEMENT =================

    public void Move()
    {
        float targetSpeed =
            (isRunning
                ? movementSpeed * 2f
                : movementSpeed)
            * moveInput.magnitude;

        currentSpeed = Mathf.SmoothDamp(
            currentSpeed,
            targetSpeed,
            ref speedVelocity,
            smoothTime
        );

        Vector3 forward =
            cameraTransform.forward;

        Vector3 right =
            cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            (
                forward * moveInput.y +
                right * moveInput.x
            ).normalized;

        HandleRotation(moveDirection);

        Vector3 horizontalMove =
            moveDirection * currentSpeed;

        HandleGravity();

        Vector3 finalMove = horizontalMove;

        finalMove.y = verticalVelocity;

        characterController.Move(
            finalMove * Time.deltaTime
        );

        UpdateAnimator();
    }

    private void HandleRotation(
        Vector3 moveDirection
    )
    {
        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    private void HandleGravity()
    {
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;
    }

    // ================= JUMP =================

    public void Jump()
    {
        if (!isGrounded || isJumping)
            return;

        isJumping = true;

        verticalVelocity =
            Mathf.Sqrt(
                jumpHeight *
                -2f *
                gravity
            );
    }

    private void HandleJumpReset()
    {
        if (
            isGrounded &&
            verticalVelocity <= 0f
        )
        {
            isJumping = false;
        }
    }

    // ================= COMBAT =================

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

    // ================= VFX SLASH EFFECT =================
    public void SpawnSlash1()
    {
        Instantiate(slashVFX, slashPoint1.position, slashPoint1.rotation);
    }

    public void SpawnSlash2()
    {
        Instantiate(slashVFX, slashPoint2.position, slashPoint2.rotation);
    }

    // ================= GROUND =================

    private void GroundedCheck()
    {
        bool wasGrounded = isGrounded;

        isGrounded =
            Physics.CheckSphere(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer
            );

        animator.SetBool(
            groundedParamName,
            isGrounded
        );

        if (!wasGrounded && isGrounded)
        {
            animator.SetBool(
                fallingParamName,
                false
            );
        }
    }

    // ================= ANIMATOR =================

    private void UpdateAnimator()
    {
        if (isAttacking)
            return;

        float normalizedSpeed =
            currentSpeed /
            (movementSpeed * 2f);

        animator.SetFloat(
            speedParamName,
            normalizedSpeed
        );

        animator.SetBool(
            fallingParamName,
            !isGrounded &&
            verticalVelocity < -0.1f
        );
    }

    // ================= INPUT =================

    private void OnMove(
        InputValue inputValue
    )
    {
        moveInput =
            inputValue.Get<Vector2>();

        if (moveInput.magnitude < 0.1f)
        {
            isRunning = false;
        }
    }

    private void OnJump()
    {
        Jump();
    }

    private void OnSprint(
        InputValue inputValue
    )
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

    // ================= GIZMOS =================

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            groundCheckPoint.position,
            groundCheckRadius
        );
    }
}