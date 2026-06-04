using UnityEngine;

public class HitState : BaseState
{
    public HitState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {

        animator.CrossFade(HitHash, 0.0f);
        //animator.Play(HitHash);

    }

    public override void Update()
    {
        player.ApplyGravity();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == HitHash)
        {
            if (state.normalizedTime >= 0.95f)
            {
                player.StopHit();
            }
        }

    }

    public override void OnExit()
    {
        player.StopHit();
    }
}
