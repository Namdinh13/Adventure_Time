using UnityEngine;
using System.Collections;

public class BossDeathHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BossHoverMovement movement;
    [SerializeField] private BossCaster caster;
    [SerializeField] private float destroyDelay = 5.0f;

    public void TriggerDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {

        if (caster != null) caster.enabled = false;

        if (movement != null) movement.enabled = false;

        animator.CrossFade("Death", 0.1f);
      
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}