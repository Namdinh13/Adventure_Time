using UnityEngine;

namespace BossFSM
{
    public class BChaseState : BossStateBase
    {
        private Transform Target;

        public BChaseState(bool needsExitTime, BossEnemy enemy, Transform Target) : base(needsExitTime, enemy)
        {
            this.Target = Target;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Agent.enabled = true;
            Agent.isStopped = false;
            Animator.Play("Chase");
        }

        public override void OnLogic()
        {
            base.OnLogic();

            if (!RequestedExit)
            {
                Agent.SetDestination(Target.position);
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                fsm.StateCanExit();
            }
        }
    }
}
