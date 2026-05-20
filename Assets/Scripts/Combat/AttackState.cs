using UnityEngine;

public class AttackState : BaseState
{
    private float attackTimer;

    private const float attackDuration = 1f;

    public AttackState(PlayerController player, Animator animator ) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.StartAttack();

        attackTimer = 0f;

        animator.SetFloat("Speed", 0f);

        PlayAttackAnimation();

        player.ConsumeAttack();
    }

    public override void Update()
    {
        player.Move();

        attackTimer += Time.deltaTime;

        if (attackTimer >= 0.35f && player.AttackPressed)
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
        switch (player.CombatMode)
        {
            case CombatMode.Unarmed:
                PlayUnarmedCombo();
                break;

            case CombatMode.Sword:
                PlaySwordCombo();
                break;
        }
    }

    private void PlayUnarmedCombo()
    {
        switch (player.ComboStep)
        {
            case 0:
                animator.Play(Punch1Hash); 
                break;

            case 1:
                animator.CrossFade(Punch2Hash, CrossFadeDuration);
                break;

            case 2:
                animator.CrossFade(Punch3Hash, CrossFadeDuration);
                break;
        }
    }

    private void PlaySwordCombo()
    {
        switch (player.ComboStep)
        {
            case 0:
                animator.CrossFade(Attack1Hash, CrossFadeDuration);
                //animator.Play(Attack1Hash);
                break;

            case 1:
                animator.CrossFade(Attack2Hash, CrossFadeDuration);
                //animator.Play(Attack2Hash);
                break;

            case 2:
                animator.CrossFade(Attack3Hash, CrossFadeDuration);
                //animator.Play(Attack3Hash);
                break;
        }
    }
}