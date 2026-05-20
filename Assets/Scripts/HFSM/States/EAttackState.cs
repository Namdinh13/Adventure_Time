using System;
using UnityEngine;
using UnityHFSM;

namespace EnemyFSM
{
    public class EAttackState : EnemyStateBase
    {
        public EAttackState(bool needsExitTime, Enemy Enemy, Action<State<EnemyState, StateEvent>> onEnter, 
            float ExitTime = 1f) : base(needsExitTime, Enemy, ExitTime, onEnter) 
        {
        }

        public override void OnEnter()
        {
            Agent.isStopped = true;

            Agent.ResetPath();

            base.OnEnter();
            Animator.Play("Attack");
        }

    }

}
