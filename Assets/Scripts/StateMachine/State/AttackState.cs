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

        player.NextCombo();

        activeAttackHash = PlayAttackAnimation();

        player.ConsumeAttack();
    }

    public override void Update()
    {

        player.ApplyGravity();

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == activeAttackHash)
        {
            float normalizedTime = state.normalizedTime % 1f;

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

    }

    private int PlayAttackAnimation()
    {
        switch (player.ComboStep)
        {
            case 0:
                animator.CrossFade(Attack1Hash, CrossFadeDuration);
                return Attack1Hash;

            case 1:
                animator.CrossFade(Attack2Hash, CrossFadeDuration);
                return Attack2Hash;

            case 2:
                animator.CrossFade(Attack3Hash, CrossFadeDuration);
                return Attack3Hash;

            default:
                animator.CrossFade(Attack1Hash, CrossFadeDuration);
                return Attack1Hash;
        }
    }


    //private int PlayAttackAnimation()
    //{
    //    switch (player.CombatMode)
    //    {
    //        case CombatMode.Unarmed:
    //            return PlayUnarmedCombo();

    //        case CombatMode.Sword:
    //            return PlaySwordCombo();

    //        default:
    //            return LocomotionHash;
    //    }
    //}

    //private int PlayUnarmedCombo()
    //{
    //    switch (player.ComboStep)
    //    {
    //        case 0:
    //            animator.Play(Punch1Hash);
    //            return Punch1Hash;

    //        case 1:
    //            animator.CrossFade(Punch2Hash, CrossFadeDuration);
    //            return Punch2Hash;

    //        case 2:
    //            animator.CrossFade(Punch3Hash, CrossFadeDuration);
    //            return Punch3Hash;

    //        default:
    //            return Punch1Hash;
    //    }
    //}

    //private int PlaySwordCombo()
    //{
    //    switch (player.ComboStep)
    //    {
    //        case 0:
    //            animator.CrossFade(Attack1Hash, CrossFadeDuration);
    //            return Attack1Hash;

    //        case 1:
    //            animator.CrossFade(Attack2Hash, CrossFadeDuration);
    //            return Attack2Hash;

    //        case 2:
    //            animator.CrossFade(Attack3Hash, CrossFadeDuration);
    //            return Attack3Hash;

    //        default:
    //            return Attack1Hash;
    //    }
    //}
}
