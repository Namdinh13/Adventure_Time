using UnityEngine;

public class HitState : BaseState
{
    public HitState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(HitHash, CrossFadeDuration);
    }

    public override void Update()
    {
        player.ApplyGravity();
        player.Move();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == HitHash && state.normalizedTime >= 1f)
        {
            player.StopHit();
        }
    }
}
