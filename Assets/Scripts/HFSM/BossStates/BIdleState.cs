using UnityEngine;


namespace BossFSM
{
    public class BIdleState : BossStateBase
    {

        public BIdleState(bool needsExitTime, BossEnemy enemy) : base(needsExitTime, enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
                
            Agent.isStopped = true;

            Animator.CrossFade("Idle", 0.15f);
        }
    }
}
