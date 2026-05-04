using UnityEngine;

public class AttackState : BaseState
{
    private float attackTimer;

    private const float attackDuration = 0.85f;

    public AttackState(PlayerController player, Animator animator ) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.StartAttack();

        attackTimer = 0f;

        PlayAttackAnimation();

        player.ConsumeAttack();
    }

    public override void Update()
    {
        player.Move();

        attackTimer += Time.deltaTime;

        if (
            attackTimer >= 0.35f &&
            player.AttackPressed
        )
        {
            player.NextCombo();

            PlayAttackAnimation();

            player.ConsumeAttack();

            attackTimer = 0f;
        }

        if (attackTimer >= attackDuration)
        {
            player.StopAttack();
        }
    }

    private void PlayAttackAnimation()
    {
        switch (player.ComboStep)
        {
            case 0:

                animator.CrossFade(
                    Attack1Hash,
                    CrossFadeDuration
                );

                break;

            case 1:

                animator.CrossFade(
                    Attack2Hash,
                    CrossFadeDuration
                );

                break;

            case 2:

                animator.CrossFade(
                    Attack3Hash,
                    CrossFadeDuration
                );

                break;
        }

        Debug.Log($"Play Combo: {player.ComboStep}"
        );
    }
}