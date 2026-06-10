using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;

    [SerializeField] private float lifeTime = 5f;

    private Vector3 moveDirection;

    public void Initialize(Transform target)
    {
        moveDirection = (target.position - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER HIT");

            Destroy(gameObject);
        }
    }
}