using UnityEngine;

public class DrawWeaponState : BaseState
{

    private bool hasEquipped;

    public DrawWeaponState(IPlayerContext playerContext, Animator animatorRef) : base(playerContext, animatorRef) { }

    public override void OnEnter()
    {
        player.ConsumeToggle();

        player.SetCombatMode(CombatMode.Sword);

        animator.CrossFade(DrawHash, WeaponFade);

        player.SetDrawingWeapon(true);

        hasEquipped = false;
    }

    public override void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == DrawHash && state.normalizedTime >= 0.4f && !hasEquipped)
        {
            player.EquipSwordToHand();

            hasEquipped = true;
        }

        if (state.shortNameHash == DrawHash && state.normalizedTime >= 0.9f)
        {
            player.SetDrawingWeapon(false);
        }
    }

    public override void OnExit() { }
}
