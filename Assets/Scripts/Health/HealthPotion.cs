using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private int healAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.Heal(healAmount);

            Debug.Log("Player healed by potion");

            Destroy(gameObject);
        }
    }
}
