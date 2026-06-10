using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    [SerializeField] private int healAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        IHealable healable = other.GetComponent<IHealable>();

        if (healable != null)
        {
            bool healed = healable.Heal(healAmount);

            if (healed)
            {
                Debug.Log("Player healed by potion");

                Destroy(gameObject);
            }
        }
    }
}
