using UnityEngine;

public class FloatingVisual : MonoBehaviour
{
    [Header("Hover")]
    [SerializeField] private float hoverHeight = 0.3f;
    [SerializeField] private float hoverSpeed = 2f;

    [Header("Rotation")]
    [SerializeField] private float tiltAmount = 8f;
    [SerializeField] private float tiltSpeed = 3f;

    [SerializeField] private BossHoverMovement movement;

    private Quaternion initialRotation;

    private Vector3 startLocalPos;

    private void Start()
    {
        startLocalPos = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        Hover();

        Vector3 localVelocity = transform.InverseTransformDirection(movement.CurrentVelocity);

        float tiltZ = -localVelocity.x * tiltAmount;
        float tiltX = localVelocity.z * tiltAmount;

        Quaternion targetRot = initialRotation * Quaternion.Euler(tiltX, 0, tiltZ);

        transform.localRotation =
            Quaternion.Lerp(
                transform.localRotation,
                targetRot,
                tiltSpeed * Time.deltaTime
            );
    }

    private void Hover()
    {
        float wave = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        Vector3 pos = startLocalPos;
        pos.y += wave;

        transform.localPosition = pos;
    }
}