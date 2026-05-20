using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Transform lockPoint;
    public Transform LockPoint => lockPoint;

    private Collider m_collider;
    private Camera cam;

    private Plane[] frustrum;
    private bool visible;
    private bool inPool;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam != null)
        {
            frustrum = GeometryUtility.CalculateFrustumPlanes(cam);

            visible = GeometryUtility.TestPlanesAABB(frustrum, m_collider.bounds);

            if (visible && !inPool)
            {
                TargetFinder.AddToPool(lockPoint);
                inPool = true;
            }
            else 
            {
                if (!visible)
                {
                    TargetFinder.RemoveFromPool(lockPoint);
                    inPool = false;
                }
            }
        }
    }
}
