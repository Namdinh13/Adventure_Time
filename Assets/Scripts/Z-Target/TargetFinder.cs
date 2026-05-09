using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetFinder : MonoBehaviour
{
    public static List<Transform> pool;
    public static DistanceClass dc; 

    private Transform currentTarget;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera targetCamera;
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private PlayerController playerController;


    public List<Transform> poolView;

    private bool lockedOn;
    private bool coolDown;

    #region Input
    private InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
    #endregion Input

    void Start()
    {
        if (pool == null)
        {
            pool = new List<Transform>();
        }

        if (dc == null)
        {
            dc = new DistanceClass();
        }
    }
   
    void Update()
    {
        poolView = pool;

        if (input.CameraControls.Targeting.WasPressedThisFrame())
        {
            if (!lockedOn)
            {
                LockOn();
            }
            else
            {
                LockOff();
            }
        }


        if (lockedOn)
        {
            if (input.Player.Look.ReadValue<Vector2>().x > 5.0f)
            {
                SelectTarget(1);
            }
            else if (input.Player.Look.ReadValue<Vector2>().x <= -5.0f)
            {
                SelectTarget(-1);
            }
            else
            {

            }
        } 
    }

    private void LockOn()
    {
        currentTarget = NearestTarget();

        if (currentTarget)
        {
            targetCamera.LookAt = currentTarget;
            targetCamera.gameObject.SetActive(true);

            playerCamera.gameObject.SetActive(false);
            lockedOn = true;
            targetAnimator.SetBool("lockedOn", true);

            playerController.SetLockTarget(currentTarget);
        }
    }

    private void LockOff()
    {
        currentTarget = null;

        targetCamera.LookAt = null;
        targetCamera.gameObject.SetActive(false);

        playerCamera.gameObject.SetActive(true);
        lockedOn = false;
        targetAnimator.SetBool("lockedOn", false);

        playerController.SetLockTarget(null);
    }

    private void SelectTarget(int next)
    {
        if (!coolDown && pool != null && pool.Count > 1)
        {
            System.Predicate<Transform> predicate = FindTransform;
            int currentIndex = pool.FindIndex(predicate);

            int nextIndex = currentIndex + next;

            if (nextIndex >= 0 && nextIndex < pool.Count) 
            {
                currentTarget = pool[nextIndex];
                targetCamera.LookAt = currentTarget;
                coolDown = true;
                StartCoroutine("ResetCoolDown");

            }
        }
    }

    private bool FindTransform(Transform t)
    {
        return t.Equals(currentTarget);
    }

    IEnumerator ResetCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        coolDown = false;
    }

    private Transform NearestTarget()
    {
        if (pool != null && pool.Count > 0)
        {
            Vector3 center = new Vector3(0.5f, 0.5f, 0);
            Camera cam = Camera.main;

            int minIndex = 0;
            float shortestDistance = 1000.0f;

            for (int i = 0; i < pool.Count; i++)
            {
                Vector3 targetViewport = cam.WorldToViewportPoint(pool[i].position);
                targetViewport -= Vector3.forward * targetViewport.z;

                float targetDistanceFromCenter = Vector3.Distance(center, targetViewport);

                if (targetDistanceFromCenter < shortestDistance)
                {
                    shortestDistance = targetDistanceFromCenter;
                    minIndex = i;
                }

            }
            //cam.WorldToViewportPoint(pool[1].position);
            return pool[minIndex];
        }
        return null;
    }

    public static void AddToPool(Transform target)
    {
        if (pool != null && !pool.Contains(target))
        {
            pool.Add(target);
            pool.Sort(dc);
        }
    }

    public static void RemoveFromPool(Transform target)
    {
        if (pool != null && pool.Contains(target))
        {
            pool.Remove(target);
            pool.Sort(dc);
        }
    }
}
public class DistanceClass : IComparer<Transform>
{
    public int Compare(Transform x, Transform y)
    {
        Camera cam = Camera.main;

        Vector3 xViewport = cam.WorldToViewportPoint(x.position);
        Vector3 yViewport = cam.WorldToViewportPoint(y.position);

        if (xViewport.x < yViewport.x)
        {
            return -1;
        }
        else if (xViewport.x == yViewport.x)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}