using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("Health UI")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI healthBarValue;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        Debug.Log($"Player take {damage} damage");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;

        //currentHealth = (int)healthBarSlider.value;

        healthBarValue.text = currentHealth + " / " + maxHealth;
    }

    private void Die()
    {
        // Handle player death (e.g., play animation, disable controls, etc.)
        Debug.Log("Player has died.");
    }
}
