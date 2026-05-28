using UnityEngine;
using System.Collections;

public class BossCaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHoverMovement movement;

    [SerializeField] private Animator animator;

    [Header("Spell")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private Transform spellSpawnPoint;

    [Header("Timing")]
    [SerializeField] private float castCooldown = 4f;

    [SerializeField] private float castDuration = 2f;

    [SerializeField] private float chargeTime = 1f;

    private bool isCasting;

    private float nextCastTime;

    private void Update()
    {
        if (isCasting) return;

        if (Time.time >= nextCastTime)
        {
            StartCoroutine(CastRoutine());
        }
    }

    private IEnumerator CastRoutine()
    {
        isCasting = true;

        movement.StopMovement();

        movement.enabled = false;

        animator.CrossFade("Cast", 0.2f);

        yield return new WaitForSeconds(chargeTime);

        SpawnProjectile();

        yield return new WaitForSeconds(castDuration);

        animator.CrossFade("HoverIdle", 0.2f);

        movement.enabled = true;

        nextCastTime = Time.time + castCooldown;

        isCasting = false;
    }

    private void SpawnProjectile()
    {
        GameObject spell = Instantiate(projectilePrefab, spellSpawnPoint.position, Quaternion.identity);

        MagicProjectile projectile = spell.GetComponent<MagicProjectile>();

        projectile.Initialize(movement.Player);
    }
}