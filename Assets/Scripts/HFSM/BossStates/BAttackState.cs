using System;
using UnityEngine;
using UnityHFSM;
using Random = UnityEngine.Random;


namespace BossFSM
{
    public class BAttackState : BossStateBase
    {
        public BAttackState(bool needsExitTime, BossEnemy enemy, Action<State<BossState, StateEvent>> onEnter, float ExitTime = 1f)
            : base(needsExitTime, enemy, ExitTime, onEnter)
        {
        }

        public override void OnEnter()
        {
            Agent.isStopped = true;

            Agent.ResetPath();

            base.OnEnter();

            int randomAttack = Random.Range(0, 3);

            switch (randomAttack)
            {
                case 0:
                    Animator.CrossFade("Attack1", 0.1f);
                    break;

                case 1:
                    Animator.CrossFade("Attack2", 0.1f);
                    break;

                case 2:
                    Animator.CrossFade("Attack3", 0.1f);
                    break;
            }
        }
    }
}
