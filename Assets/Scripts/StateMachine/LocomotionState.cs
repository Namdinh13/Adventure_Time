using UnityEngine;

public class LocomotionState : BaseState
{
    public LocomotionState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        if (player.LockedOn)
        {
            animator.CrossFade(StrafeHash, CrossFadeDuration);
        }
        else
        {
            animator.CrossFade(LocomotionHash, CrossFadeDuration);
        }
    }

    public override void Update()
    {
        //Debug.Log("Locomotion");
        if (player.AttackPressed) return;
        player.Move();
    }
}
