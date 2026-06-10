using UnityEngine;

public class BossHoverMovement : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 4f;

    [Header("Combat Distance")]
    [SerializeField] private float preferredDistance = 8f;

    [Header("Orbit")]
    [SerializeField] private float orbitSpeed = 1.5f;
    [SerializeField] private float orbitRadius = 3f;

    [Header("Flight")]
    [SerializeField] private float flightHeight = 6f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private float orbitAngle;

    private bool isMoving;

    private string currentAnimation;

    private Vector3 velocity;

    public Vector3 CurrentVelocity => velocity;

    public Transform Player => player;

    private void Start()
    {
        PlayAnimation("HoverIdle");
    }

    private void Update()
    {
        if (!player) return;

        MoveAroundPlayer();

        FacePlayer();

        HandleAnimation();
    }

    private void MoveAroundPlayer()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;

        Vector3 orbitOffset = new Vector3(Mathf.Cos(orbitAngle), 0, Mathf.Sin(orbitAngle)) * orbitRadius;

        Vector3 directionFromPlayer = (transform.position - player.position).normalized;

        Vector3 desiredPosition = player.position + directionFromPlayer * preferredDistance + orbitOffset;

        desiredPosition.y = flightHeight;

        float smoothTime = 1f / moveSpeed;

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                smoothTime
            );
    }

    private void FacePlayer()
    {
        Vector3 lookDir = player.position - transform.position;

        if (lookDir == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDir);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    private void HandleAnimation()
    {
        float speed = velocity.magnitude;

        if (speed > 0.5f)
        {
            if (!isMoving)
            {
                PlayAnimation("FloatMove");

                isMoving = true;
            }
        }
        else
        {
            if (isMoving)
            {
                PlayAnimation("HoverIdle");

                isMoving = false;
            }
        }
    }

    private void PlayAnimation(string stateName)
    {
        if (currentAnimation == stateName) return;

        animator.CrossFade(stateName, 0.2f);

        currentAnimation = stateName;
    }

    public void StopMovement()
    {
        velocity = Vector3.zero;
    }
}