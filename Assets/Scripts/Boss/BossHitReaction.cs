using UnityEngine;
using System.Collections;

public class BossHitReaction : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float hitDuration = 0.35f;

    [SerializeField] private BossCaster caster;

    private bool reacting;

    public void PlayHit()
    {
        if (reacting) return;

        if (caster != null && caster.IsCasting)
            return;

        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        reacting = true;

        animator.CrossFade("Hit", 0.05f);

        yield return new WaitForSeconds(hitDuration);

        animator.CrossFade("HoverIdle", 0.15f);

        reacting = false;
    }
}