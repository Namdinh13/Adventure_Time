using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText;

    public int maxHealth = 100;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;

        healthBarSlider.maxValue = maxHealth;
    }

    private void Update()
    {
        healthBarSlider.value = currentHealth;

        healthBarValueText.text = currentHealth + " / " + maxHealth;
    }
}