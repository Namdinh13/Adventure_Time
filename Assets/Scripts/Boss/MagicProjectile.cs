//using UnityEngine;

//public class MagicProjectile : MonoBehaviour
//{
//    [SerializeField] private float speed = 12f;

//    [SerializeField] private float rotateSpeed = 5f;

//    [SerializeField] private float lifeTime = 5f;

//    private Transform target;

//    public void Initialize(Transform newTarget)
//    {
//        target = newTarget;
//    }

//    private void Start()
//    {
//        Destroy(gameObject, lifeTime);
//    }

//    private void Update()
//    {
//        if (!target) return;

//        Vector3 direction = (target.position - transform.position).normalized;

//        Vector3 smoothedDirection = Vector3.Lerp(transform.forward, direction, rotateSpeed * Time.deltaTime);

//        transform.forward = smoothedDirection;

//        transform.position += transform.forward * speed * Time.deltaTime;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            Debug.Log("PLAYER HIT");

//            Destroy(gameObject);
//        }
//    }
//}


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