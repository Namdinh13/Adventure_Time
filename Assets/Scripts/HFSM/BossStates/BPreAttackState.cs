using UnityEngine;

namespace BossFSM
{
    public class BPreAttackState : BossStateBase
    {
        private Transform target;

        public BPreAttackState(
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

            Agent.isStopped = true;

            Agent.ResetPath();

            Animator.CrossFade("PreAttack", 0.1f);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            // Always face player
            Vector3 lookDirection = target.position - Enemy.transform.position;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);

                Enemy.transform.rotation =
                    Quaternion.Slerp(
                        Enemy.transform.rotation,
                        rotation,
                        Time.deltaTime * 7f
                    );
            }

            if (Animator.IsInTransition(0)) return;

            AnimatorStateInfo state =  Animator.GetCurrentAnimatorStateInfo(0);

            // End pre attack animation
            if (state.normalizedTime >= 0.95f)
            {
                fsm.StateCanExit();
            }
        }
    }
}