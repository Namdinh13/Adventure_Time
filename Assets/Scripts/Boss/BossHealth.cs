using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 200;
    [SerializeField] private BossHitReaction hitReaction;
    [SerializeField] private BossDeathHandler deathHandler;

    private int currentHp;
    private bool isDead;
    //private BossEnemy enemy;

    private void Awake()
    {
        currentHp = maxHp;

        //enemy = GetComponent<BossEnemy>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;

        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        Debug.Log($"Boss HP: {currentHp}");

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        hitReaction.PlayHit();

    }

    //public void Heal(int amount)
    //{
    //    if (isDead) return;

    //    currentHp += amount;

    //    currentHp = Mathf.Clamp(currentHp, 0, maxHp);
    //}

    private void Die()
    {
        isDead = true;

        Debug.Log("Boss Dead");

        deathHandler.TriggerDeath();
    }
}
