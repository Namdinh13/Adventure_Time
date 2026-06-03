using UnityEngine;

public class HitState : BaseState
{
    private float Timer = 1.5f;

    public HitState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {
        Timer = 1.5f;
        animator.CrossFade(HitHash, CrossFadeDuration);
    }

    public override void Update()
    {
        player.ApplyGravity();

        Timer -= Time.deltaTime;

        if (Timer <= 0f)
        {
            player.StopHit();
        }

        //AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        //if (state.shortNameHash == HitHash && state.normalizedTime >= 1f)
        //{
        //    player.StopHit();
        //}
    }
}
