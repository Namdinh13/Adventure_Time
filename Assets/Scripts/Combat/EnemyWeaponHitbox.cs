using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private HashSet<Collider> hitTargets = new();

    public void ResetHitTargets()
    {
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitTargets.Contains(other)) return;

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            hitTargets.Add(other);

            Debug.Log($"Enemy hit {other.name}");
        }
    }
}
