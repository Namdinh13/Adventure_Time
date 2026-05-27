using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {
        player.ConsumeJumpBuffer();
        player.ApplyJump();
        animator.CrossFade(JumpHash, CrossFadeDuration);
    }

    public override void Update()
    {
        player.ApplyGravity();
        player.ResetJumpState();
        player.Move();
    }
}
