using UnityEngine;


namespace BossFSM
{
    public class BCombatMoveState : BossStateBase
    {
        private Transform target;

        private float strafeDirection;

        private float strafeTimer;

        public BCombatMoveState(
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

            Agent.isStopped = false;

            Agent.speed = 1.5f;

            ChooseDirection();
        }

        public override void OnLogic()
        {
            base.OnLogic();

            strafeTimer += Time.deltaTime;

            // Change direction sometimes
            if (strafeTimer >= 2f)
            {
                ChooseDirection();
            }

            // Face player
            Vector3 lookDirection = target.position - Enemy.transform.position;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);

                Enemy.transform.rotation =
                    Quaternion.Slerp(
                        Enemy.transform.rotation,
                        rotation,
                        Time.deltaTime * 6f
                    );
            }

            // Move sideways around player
            Vector3 toPlayer = (target.position - Enemy.transform.position).normalized;

            // Keep combat distance
            Vector3 desiredPosition = target.position - toPlayer * 1.0f;

            // Add strafing
            desiredPosition += Enemy.transform.right * strafeDirection * 1.5f;

            Agent.SetDestination(desiredPosition);
        }

        private void ChooseDirection()
        {
            strafeTimer = 0f;

            strafeDirection = Random.value > 0.5f ? 1f : -1f;

            if (strafeDirection > 0)
            {
                Animator.CrossFade("StrafeRight", 0.15f, 0);
            }
            else
            {
                Animator.CrossFade("StrafeLeft", 0.15f, 0);
            }
        }
    }
}
