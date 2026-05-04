using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        //Debug.Log("Enter Jump State");
        animator.CrossFade(JumpHash, CrossFadeDuration);      
    }

    public override void Update()
    {
        player.Move();
    }
}