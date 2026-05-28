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

            Enemy.ContinueCombo = Random.value > 0.40f;

            int comboAttack = Enemy.GetNextComboAttack();

            Animator.CrossFade($"Attack{comboAttack}", 0.1f, 0);
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

                Enemy.ResetDecisionTimer();

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

