using UnityEngine;

public abstract class BaseState : IState
{
    protected readonly IPlayerContext player;
    protected readonly Animator animator;

    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int CombatStrafeHash = Animator.StringToHash("CombatStrafe");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int Attack1Hash = Animator.StringToHash("Attack_1");
    protected static readonly int Attack2Hash = Animator.StringToHash("Attack_2");
    protected static readonly int Attack3Hash = Animator.StringToHash("Attack_3");
    protected static readonly int HitHash = Animator.StringToHash("Hit");
    protected static readonly int DodgeHash = Animator.StringToHash("Dodge");
    protected static readonly int SheatheHash = Animator.StringToHash("SheatheWeapon");
    protected static readonly int DrawHash = Animator.StringToHash("DrawWeapon");

    protected const float CrossFadeDuration = 0.1f;

    protected BaseState(IPlayerContext playerContext, Animator animatorRef)
    {
        player = playerContext;
        animator = animatorRef;
    }

    public virtual void FixedUpdate() { }

    public virtual void OnEnter() { }

    public virtual void OnExit() { }

    public virtual void Update() { }
}
