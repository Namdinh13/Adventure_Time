using UnityEngine;

namespace BossFSM
{
    public class BHitState : BossStateBase
    {
        public BHitState(bool needsExitTime, BossEnemy enemy) : base(needsExitTime, enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Agent.isStopped = true;

            Agent.ResetPath();

            Animator.Play("Hit");
        }

        public override void OnLogic()
        {
            base.OnLogic();

            AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Hit") && state.normalizedTime >= 1f)
            {
                fsm.StateCanExit();
            }
        }
    }
}
