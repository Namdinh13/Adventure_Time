using UnityEngine;


namespace BossFSM
{
    public class BIdleState : BossStateBase
    {
        private float AnimationLoopCount = 0;

        public BIdleState(bool needsExitTime, BossEnemy enemy) : base(needsExitTime, enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Agent.isStopped = true;

            Animator.CrossFade("Idle", 0.15f);
        }

        //public override void OnLogic()
        //{
        //    AnimatorStateInfo state = Animator.GetCurrentAnimatorStateInfo(0);

        //    if (state.normalizedTime >= AnimationLoopCount + 1)
        //    {
        //        float value = Random.value;
        //        if (value < 0.95f)
        //        {
        //            if (!state.IsName("Idle_A"))
        //            {
        //                AnimationLoopCount = 0;
        //            }
        //            else
        //            {
        //                AnimationLoopCount++;
        //            }
        //            Animator.Play("Idle_A");
        //        }
        //        else if (value < 0.975f)
        //        {
        //            if (!state.IsName("Idle_B"))
        //            {
        //                AnimationLoopCount = 0;
        //            }
        //            else
        //            {
        //                AnimationLoopCount++;
        //            }
        //            Animator.Play("Idle_B");
        //        }
        //        else
        //        {
        //            if (!state.IsName("Idle_C"))
        //            {
        //                AnimationLoopCount = 0;
        //            }
        //            else
        //            {
        //                AnimationLoopCount++;
        //            }
        //            Animator.Play("Idle_C");
        //        }
        //    }
        //    base.OnLogic();
        //}
    }
}
