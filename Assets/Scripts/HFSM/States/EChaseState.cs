using UnityEngine;

namespace EnemyFSM
{
    public class EChaseState : EnemyStateBase
    {
        private Transform Target;

        public EChaseState(bool needsExitTime, Enemy Enemy, Transform Target) : base(needsExitTime, Enemy)
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
