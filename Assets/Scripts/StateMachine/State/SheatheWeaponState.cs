using UnityEngine;

public class SheatheWeaponState : BaseState
{
    private bool hasSheathed;

    public SheatheWeaponState(IPlayerContext playerContext, Animator animatorRef): base(playerContext, animatorRef) { }

    public override void OnEnter()
    {

        player.ConsumeToggle();

        animator.CrossFade(SheatheHash, 0f);

        player.SetSheathingWeapon(true);

        hasSheathed = false;
    }

    public override void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.shortNameHash == SheatheHash && state.normalizedTime >= 0.9f && !hasSheathed)
        {
            hasSheathed = true;

            player.EquipSwordToBack();      
            
            player.SetCombatMode(CombatMode.Unarmed); 

            player.SetSheathingWeapon(false);
        }
    }

    public override void OnExit() { }
}
