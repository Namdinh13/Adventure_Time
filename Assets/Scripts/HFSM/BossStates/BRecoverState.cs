using UnityEngine;

namespace BossFSM
{
    public class BRecoverState : BossStateBase
    {
        private Transform target;

        private float recoverTimer;

        private const float RecoverDuration = 1f;

        public BRecoverState(
            bool needsExitTime,
            BossEnemy enemy,
            Transform target
        ) : base(needsExitTime, enemy)
        {
            this.target = target;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Enemy.ResetCombo();

            Agent.isStopped = true;

            recoverTimer = 0f;

            Animator.CrossFade("CombatIdle", 0.15f, 0);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            recoverTimer += Time.deltaTime;

            Vector3 lookDirection = target.position - Enemy.transform.position;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);

                Enemy.transform.rotation =
                    Quaternion.Slerp(
                        Enemy.transform.rotation,
                        rotation,
                        Time.deltaTime * 3f
                    );
            }

            if (recoverTimer >= RecoverDuration)
            {
                fsm.StateCanExit();
            }
        }
    }
}