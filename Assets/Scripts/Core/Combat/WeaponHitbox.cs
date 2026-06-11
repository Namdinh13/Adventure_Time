using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
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

        if (!other.CompareTag("HurtBox")) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            hitTargets.Add(other);

            SoundManager.Instance.PlaySound3D("SwordHit", transform.position);

            Debug.Log($"Hit {other.name}");
        }
    }
}

