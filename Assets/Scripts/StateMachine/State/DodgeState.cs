using UnityEngine;

public class DodgeState : BaseState
{
    private const float DodgeSpeed = 6.5f;

    private Vector3 dodgeDirection;

    public DodgeState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {
        player.ConsumeDodge();

        dodgeDirection = player.CurrentMoveDirection;

        if (dodgeDirection.sqrMagnitude < 0.01f)
        {
            dodgeDirection = player.PlayerTransform.forward;
        }

        Quaternion dodgeRotation = Quaternion.LookRotation(dodgeDirection);

        player.ModelHolder.rotation = dodgeRotation;

        player.SetDodging(true);

        player.SetInvulnerable(true);

        animator.CrossFade(DodgeHash, CrossFadeDuration);
    }

    public override void Update()
    {

        player.ApplyGravity();

        player.DodgeMove(dodgeDirection, DodgeSpeed);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == DodgeHash && state.normalizedTime >= 0.9f)
        {
            player.SetInvulnerable(false);

            player.SetDodging(false);
        }
    }

    public override void OnExit()
    {
        player.ResetModelRotation();

        player.SetInvulnerable(false);

        player.SetDodging(false);
    }
}


