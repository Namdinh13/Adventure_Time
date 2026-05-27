using UnityEngine;


namespace BossFSM
{
    public class BCombatMoveState : BossStateBase
    {
        private Transform Target;

        private float strafeTimer;

        private Vector3 currentMovePosition;

        public BCombatMoveState(bool needsExitTime,BossEnemy enemy,Transform target) : base(needsExitTime, enemy)
        {
            Target = target;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Agent.isStopped = false;

            Agent.speed = 1.2f;

            Animator.CrossFade("Walk", 0.1f);

            PickNewPosition();
        }

        public override void OnLogic()
        {
            base.OnLogic();

            strafeTimer += Time.deltaTime;

            if (strafeTimer >= 2f)
            {
                PickNewPosition();

                strafeTimer = 0f;
            }    

            Vector3 lookDirection = Target.position - Enemy.transform.position;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(lookDirection);

                Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, rotation, Time.deltaTime * 5f);
            }
        }

        private void PickNewPosition()
        {
            Vector3 direction = (Target.position - Enemy.transform.position).normalized;

            Vector3 side = Enemy.transform.right * Random.Range(-1f, 1f);

            currentMovePosition = Target.position - direction * 0.6f + side;

            Agent.SetDestination(currentMovePosition);
        }
    }
}
