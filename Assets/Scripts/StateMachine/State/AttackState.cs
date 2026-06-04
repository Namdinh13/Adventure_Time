using UnityEngine;

public class AttackState : BaseState
{
    private const float ComboThreshold = 0.55f;
    private const float AttackEndThreshold = 0.9f;

    private int activeAttackHash;

    public AttackState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef)
    {
    }

    public override void OnEnter()
    {

        player.StartAttack();

        activeAttackHash = PlayAttackAnimation();

        player.ConsumeAttack();

    }

    public override void Update()
    {

        player.ApplyGravity();

        //AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        //if (state.shortNameHash == activeAttackHash)
        //{
        //    float normalizedTime = state.normalizedTime % 1f;

        //    if (normalizedTime >= ComboThreshold && player.AttackPressed)
        //    {
        //        player.NextCombo();

        //        activeAttackHash = PlayAttackAnimation();

        //        player.ConsumeAttack();

        //        return;
        //    } 

        //    if (normalizedTime >= AttackEndThreshold)
        //    {
        //        player.StopAttack();
        //    }
        //}

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
    }

    private int PlayAttackAnimation()
    {
        switch (player.ComboStep)
        {
            case 0:
                animator.CrossFade(Attack1Hash, 0.0f);
                //animator.Play(Attack1Hash);
                return Attack1Hash;

            case 1:
                animator.CrossFade(Attack2Hash, 0.0f);
                //animator.Play(Attack2Hash);
                return Attack2Hash;

            case 2:
                animator.CrossFade(Attack3Hash, 0.0f);
                //animator.Play(Attack3Hash);
                return Attack3Hash;

            default:
                animator.CrossFade(Attack1Hash, 0.0f);
                //animator.Play(Attack1Hash);
                return Attack1Hash;
        }
    }
}
