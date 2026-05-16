using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 50;

    private int currentHp;

    private Enemy enemy;

    private void Awake()
    {
        currentHp = maxHp;

        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        Debug.Log($"Enemy HP: {currentHp}");

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
        Debug.Log("Enemy Dead");

        Destroy(gameObject);
    }
}
