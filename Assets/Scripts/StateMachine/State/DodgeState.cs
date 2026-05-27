using UnityEngine;

public class DodgeState : BaseState
{
    private const float DodgeSpeed = 6.5f;
    private const float DodgeDuration = 0.55f;

    private static readonly int DodgeHash = Animator.StringToHash("Dodge");

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

        animator.CrossFade(DodgeHash, CrossFadeDuration);
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        // Keep character grounded feel during dodge.
        player.ApplyGravity();
        player.DodgeMove(dodgeDirection, DodgeSpeed);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if ((state.shortNameHash == DodgeHash && state.normalizedTime >= 1f) || timer >= DodgeDuration)
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
}

