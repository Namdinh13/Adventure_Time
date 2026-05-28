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
            Debug.Log("ENTER HIT");
        }

        public override void OnLogic()
        {
            base.OnLogic();

            if (Animator.IsInTransition(0)) return;

            AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);

            if (state.normalizedTime >= 0.95f)
            {
                fsm.StateCanExit();
            }
        }
    }
}
