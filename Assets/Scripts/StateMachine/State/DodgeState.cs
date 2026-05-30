using UnityEngine;

public class DodgeState : BaseState
{
    private const float DodgeSpeed = 6.5f;
    private const float DodgeDuration = 0.55f;

    private float timer;
    private Vector3 dodgeDirection;

    public DodgeState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {
        player.ConsumeDodge();

        timer = 0f;
        dodgeDirection = player.GetDodgeDirection();

        player.SetDodging(true);
        player.SetInvulnerable(true);

        int dodgeAnim = GetDodgeAnimation();
        animator.CrossFade(dodgeAnim, CrossFadeDuration);
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        player.ApplyGravity();
        player.DodgeMove(dodgeDirection, DodgeSpeed);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if ((state.normalizedTime >= 1f) || timer >= DodgeDuration)
        {
            player.SetInvulnerable(false);
            player.SetDodging(false);
        }
    }

    public override void OnExit()
    {
        player.SetInvulnerable(false);
        player.SetDodging(false);
    }

    private int GetDodgeAnimation()
    {
        Vector3 dodgeDir = dodgeDirection;

        Vector3 forward = player.PlayerTransform.forward;
        Vector3 right = player.PlayerTransform.right;

        float forwardDot = Vector3.Dot(forward, dodgeDir);
        float rightDot = Vector3.Dot(right, dodgeDir);

        if (forwardDot > 0.5f) return DodgeForwardHash;

        if (forwardDot < -0.5f) return DodgeBackHash;

        if (rightDot > 0.5f) return DodgeRightHash;

        return DodgeLeftHash;
    }
}

