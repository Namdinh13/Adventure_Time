using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetFinder : MonoBehaviour
{
    public static List<Transform> pool;
    public List<Transform> poolView;

    #region Input
    private DefaultInputActions input;

    private void Awake()
    {
        input = new DefaultInputActions();
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pool == null)
        {
            pool = new List<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        poolView = pool;
    }

    public static void AddToPool(Transform target)
    {
        if (pool != null && !pool.Contains(target))
        {
            pool.Add(target);
        }
    }

    public static void RemoveFromPool(Transform target)
    {
        if (pool != null && pool.Contains(target))
        {
            pool.Remove(target);
        }
    }
}
