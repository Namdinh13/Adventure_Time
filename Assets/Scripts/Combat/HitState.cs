using UnityEngine;

public class HitState : BaseState
{
    private float hitTimer;

    private const float hitDuration = 0.5f;

    public HitState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        hitTimer = 0f;

        animator.CrossFade(HitHash, CrossFadeDuration);
    }

    public override void Update()
    {
        hitTimer += Time.deltaTime;

        if (hitTimer >= hitDuration)
        {
            player.StopHit();
        }
    }
}
