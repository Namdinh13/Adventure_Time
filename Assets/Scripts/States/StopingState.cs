using UnityEngine;
public class StoppingState : BaseState
{
    private float phaseTimer;
    private const float RunStopDuration = 0.9f;
    private const float WalkStopDuration = 1.667f;

    public StoppingState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef) { }

    public override void OnEnter()
    {
        player.SetStopping(true);

        if (player.WasRunningBeforeStop)
        {
            phaseTimer = RunStopDuration;
            animator.CrossFade(RunStopHash, CrossFadeDuration);
        }
        else
        {
            phaseTimer = WalkStopDuration;
            animator.CrossFade(WalkStopHash, CrossFadeDuration);
        }
    }

    public override void Update()
    {
        player.ApplyGravity();

        player.Move();

        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0f)
            player.SetStopping(false);
    }

    public override void OnExit()
    {
        player.SetStopping(false);
    }
}