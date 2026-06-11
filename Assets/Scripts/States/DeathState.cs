using UnityEngine;

public class DeathState : BaseState
{
    private bool hasTriggeredGameOver;

    public DeathState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef) { }

    public override void OnEnter()
    {
        hasTriggeredGameOver = false;

        player.StartDeath();

        animator.CrossFade(DeathHash, DeathFade);
    }

    public override void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == DeathHash && state.normalizedTime >= 0.95f && !hasTriggeredGameOver)
        {
            hasTriggeredGameOver = true;

            player.TriggerGameOver();
        }
    }

    public override void OnExit() { }
}
