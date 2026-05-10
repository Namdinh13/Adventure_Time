using UnityEngine;

public class WoodenBox : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 20;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject dropItem;

    private bool isDestroyed;

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        hp -= damage;

        Debug.Log("Box hit!");

        if (hp <= 0)
        {
            Break();
        }
    }

    public void Heal(int amount)
    {
        
    }

    private void Break()
    {
        isDestroyed = true;

        Debug.Log("Box destroyed");

        if (dropItem != null)
        {
            Instantiate(dropItem, spawnPoint.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
