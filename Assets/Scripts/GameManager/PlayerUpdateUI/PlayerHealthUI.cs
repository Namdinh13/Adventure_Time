using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private PlayerHealth playerHealth;


    private void Update()
    {
        healthBarSlider.maxValue = playerHealth.maxHealth;

        healthBarSlider.value = playerHealth.currentHealth;
    }
}
