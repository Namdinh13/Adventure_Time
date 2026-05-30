using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 200;

    [SerializeField] private BossHitReaction hitReaction;

    private int currentHp;

    private BossEnemy enemy;

    private void Awake()
    {
        currentHp = maxHp;

        enemy = GetComponent<BossEnemy>();
    }

    public void TakeDamage(int damage)
    {
        //currentHp -= damage;

        //hitReaction.PlayHit();

        //StartCoroutine(HitFlash());

        //SpawnHitVFX();

        //if (currentHp <= 0)
        //{
        //    Die();
        //}

        currentHp -= damage;

        hitReaction.PlayHit();

        Debug.Log($"Boss HP: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHp += amount;

        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    }

    private void Die()
    {
        Debug.Log("BossEnemy Dead");

        Destroy(gameObject);
    }
}
