using UnityEngine;

public class AttackState : BaseState
{
    private const float ComboThreshold = 0.85f;
    private const float AttackEndThreshold = 0.95f;

    private int activeAttackHash;

    public AttackState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {

        player.StartAttack();

        player.SetRootMotion(true);

        activeAttackHash = PlayAttackAnimation();

        player.ConsumeAttack();

    }

    public override void Update()
    {

        player.ApplyGravity();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        float normalizedTime = state.normalizedTime % 1f;

        if (state.shortNameHash == activeAttackHash)
        {
            if (normalizedTime >= ComboThreshold && player.AttackPressed)
            {
                player.NextCombo();

                activeAttackHash = PlayAttackAnimation();

                player.ConsumeAttack();

                return;
            }

            if (normalizedTime >= AttackEndThreshold)
            {
                player.StopAttack();
            }
        }
    }

    public override void OnExit()
    {
        player.ResetCombo();

        player.SetRootMotion(false);
    }

    private int PlayAttackAnimation()
    {
        switch (player.ComboStep)
        {
            case 0:
                animator.Play(Attack1Hash);
                return Attack1Hash;

            case 1:
                animator.CrossFade(Attack2Hash, AttackFade);
                return Attack2Hash;

            case 2:
                animator.CrossFade(Attack3Hash, AttackFade);
                return Attack3Hash;

            case 3:
                animator.CrossFade(Attack4Hash, AttackFade);
                return Attack4Hash;

            case 4:
                animator.CrossFade(Attack5Hash, AttackFade);
                return Attack5Hash; 

            default:
                animator.Play(Attack1Hash);
                return Attack1Hash;
        }
    }
}
