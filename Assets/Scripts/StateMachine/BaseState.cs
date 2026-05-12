using UnityEngine;

public abstract class BaseState : IState
{
    protected readonly PlayerController player;
    protected readonly Animator animator;

    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int Attack1Hash = Animator.StringToHash("Attack1");
    protected static readonly int Attack2Hash = Animator.StringToHash("Attack2");
    protected static readonly int Attack3Hash = Animator.StringToHash("Attack3");
    protected static readonly int HitHash = Animator.StringToHash("Hit");

    protected const float CrossFadeDuration = 0.1f;

    protected BaseState(PlayerController playerNeed, Animator animatorNeed)
    {
        player = playerNeed;
        animator = animatorNeed;
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void OnEnter()
    {
        
    }

    public virtual void OnExit()
    {
        
    }

    public virtual void Update()
    {
            
    }
}

