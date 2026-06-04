using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{
    [SerializeField] private PlayerController player;

    [Header("Health Settings")]
    [SerializeField] public int maxHealth = 100;

    public int currentHealth;


    private void Start()
    {
        currentHealth = maxHealth;
   
    }
    public void TakeDamage(int damage)
    {
        if (player != null && player.IsInvulnerable || player.IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        player.StartHit();
    }

    public bool Heal(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            return false;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        return true;
    }

    public void Die()
    {
        player.StartDeath();
    }
}
