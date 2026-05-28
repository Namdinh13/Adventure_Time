using UnityEngine;

namespace BossFSM
{
    public class BCombatIdleState : BossStateBase
    {
        private Transform target;

        private float idleTimer;

        private const float IdleDuration = 1.2f;

        public BCombatIdleState(bool needsExitTime, BossEnemy enemy, Transform target) : base(needsExitTime, enemy)
        {
            this.target = target;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Agent.isStopped = true;

            idleTimer = 0f;

            Animator.CrossFade("CombatIdle", 0.15f);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            idleTimer += Time.deltaTime;

            // Always face player
            Vector3 lookDirection = target.position - Enemy.transform.position;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);

                Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, rotation, Time.deltaTime * 5f);
            }

            // Wait before next action
            if (idleTimer >= IdleDuration)
            {
                fsm.StateCanExit();
            }
        }
    }
}