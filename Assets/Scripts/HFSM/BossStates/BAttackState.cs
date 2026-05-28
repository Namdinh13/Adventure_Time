using System;
using UnityEngine;
using UnityHFSM;
using Random = UnityEngine.Random;


namespace BossFSM
{
    public class BAttackState : BossStateBase
    {
        private bool weaponEnabled;

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

        public override void OnLogic()
        {
            base.OnLogic();

            if (Animator.IsInTransition(0)) return;

            AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);

            float time = state.normalizedTime;

            if (time >= 0.05f && time <= 0.20f)
            {
                if (!weaponEnabled)
                {
                    Enemy.EnableWeapon();
                    weaponEnabled = true;
                }
            }
            else
            {
                if (weaponEnabled)
                {
                    Enemy.DisableWeapon();
                    weaponEnabled = false;
                }
            }

            if (time >= 0.95f)
            {
                Enemy.DisableWeapon();

                fsm.StateCanExit();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            Enemy.DisableWeapon();

            Agent.isStopped = false;
        }

    }
}

//using System;
//using UnityEngine;
//using UnityHFSM;
//using Random = UnityEngine.Random;

//namespace BossFSM
//{
//    public class BAttackState : BossStateBase
//    {
//        // ======================
//        // TIMERS
//        // ======================

//        private float windupTimer;
//        private float recoverTimer;

//        // ======================
//        // FLAGS
//        // ======================

//        private bool attackStarted;
//        private bool weaponEnabled;

//        // ======================
//        // SETTINGS
//        // ======================

//        private const float WindupDuration = 0.4f;
//        private const float RecoverDuration = 0.35f;

//        public BAttackState(
//            bool needsExitTime,
//            BossEnemy enemy,
//            Action<State<BossState, StateEvent>> onEnter,
//            float ExitTime = 1f
//        )
//            : base(needsExitTime, enemy, ExitTime, onEnter)
//        {
//        }

//        public override void OnEnter()
//        {
//            Agent.isStopped = true;
//            Agent.ResetPath();

//            base.OnEnter();

//            // Reset timers
//            windupTimer = 0f;
//            recoverTimer = 0f;

//            // Reset flags
//            attackStarted = false;
//            weaponEnabled = false;

//            // Look at player before attack
//            Vector3 lookDirection =
//                Enemy.Player.transform.position - Enemy.transform.position;

//            lookDirection.y = 0f;

//            if (lookDirection != Vector3.zero)
//            {
//                Enemy.transform.rotation =
//                    Quaternion.LookRotation(lookDirection);
//            }

//            // Stay idle during windup
//            Animator.CrossFade("Idle", 0.1f);
//        }

//        public override void OnLogic()
//        {
//            base.OnLogic();

//            // ======================
//            // WINDUP
//            // ======================

//            if (!attackStarted)
//            {
//                windupTimer += Time.deltaTime;

//                if (windupTimer >= WindupDuration)
//                {
//                    attackStarted = true;

//                    int randomAttack = Random.Range(0, 3);

//                    switch (randomAttack)
//                    {
//                        case 0:
//                            Animator.CrossFade("Attack1", 0.1f);
//                            break;

//                        case 1:
//                            Animator.CrossFade("Attack2", 0.1f);
//                            break;

//                        case 2:
//                            Animator.CrossFade("Attack3", 0.1f);
//                            break;
//                    }
//                }

//                return;
//            }

//            // ======================
//            // WAIT TRANSITION
//            // ======================

//            if (Animator.IsInTransition(0)) return;

//            AnimatorStateInfo state =
//                Animator.GetCurrentAnimatorStateInfo(0);

//            float time = state.normalizedTime;

//            // ======================
//            // ACTIVE FRAMES
//            // ======================

//            if (time >= 0.05f && time <= 0.20f)
//            {
//                if (!weaponEnabled)
//                {
//                    Enemy.EnableWeapon();
//                    weaponEnabled = true;
//                }
//            }
//            else
//            {
//                if (weaponEnabled)
//                {
//                    Enemy.DisableWeapon();
//                    weaponEnabled = false;
//                }
//            }

//            // ======================
//            // RECOVER
//            // ======================

//            if (time >= 0.95f)
//            {
//                Enemy.DisableWeapon();

//                recoverTimer += Time.deltaTime;

//                if (recoverTimer >= RecoverDuration)
//                {
//                    fsm.StateCanExit();
//                }
//            }
//        }

//        public override void OnExit()
//        {
//            base.OnExit();

//            Enemy.DisableWeapon();

//            Agent.isStopped = false;
//        }
//    }
//}

