using UnityEngine;

public class LocomotionState : BaseState
{
    private bool wasLockedOn;
    private CombatMode wasCombatMode;

    public LocomotionState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {
        PlayLocomotionAnimation();
        CacheStance();
    }

    public override void Update()
    {
        if (player.LockedOn != wasLockedOn || player.CombatMode != wasCombatMode)
        {
            PlayLocomotionAnimation();
            CacheStance();
        }

        player.ApplyGravity();
        player.Move();
    }

    private void CacheStance()
    {
        wasLockedOn = player.LockedOn;
        wasCombatMode = player.CombatMode;
    }

    private void PlayLocomotionAnimation()
    {
        if (player.LockedOn)
        {
            animator.CrossFade(CombatStrafeHash, CrossFadeDuration);
        }
        else
        {
            animator.CrossFade(LocomotionHash, CrossFadeDuration);
        }
    }
}
