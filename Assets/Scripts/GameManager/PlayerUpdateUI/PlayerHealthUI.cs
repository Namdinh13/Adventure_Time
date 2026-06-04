using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI deathText;

    private void Start()
    {
        deathText.gameObject.SetActive(false);
    }


    private void Update()
    {
        healthBarSlider.maxValue = playerHealth.maxHealth;

        healthBarSlider.value = playerHealth.currentHealth;

        if(healthBarSlider.value <= 0)
        {
            deathText.gameObject.SetActive(true);
        }
    }


}
