using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;        
    [SerializeField] private float zoomLerpSpeed = 12f;   
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 20f;

    private InputSystem_Actions controls;
    private CinemachineOrbitalFollow orbital;

    private float targetZoom;
    private float currentZoom;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Enable();

        
        controls.CameraControls.MouseZoom.performed += HandleMouseScroll;
    }

    private void Start()
    {
       
        var vcam = GetComponent<CinemachineCamera>();
        if (vcam != null)
        {
            orbital = vcam.GetComponent<CinemachineOrbitalFollow>();
        }

        if (orbital == null)
        {
            Debug.LogError("Khong tim thay CinemachineOrbitalFollow tren Virtual Camera!");
            return;
        }

        
        currentZoom = targetZoom = orbital.Radius;
    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        Vector2 scroll = context.ReadValue<Vector2>();
        Debug.Log($"Mouse Scroll: {scroll.y}");   

        if (orbital != null)
        {
            
            float zoomChange = -scroll.y * 0.01f * zoomSpeed;   
            targetZoom = Mathf.Clamp(targetZoom + zoomChange, minDistance, maxDistance);
        }
    }

    private void Update()
    {
        if (orbital == null) return;

        
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
        orbital.Radius = currentZoom;
    }

    private void OnDestroy()
    {
        if (controls != null)
        {
            controls.CameraControls.MouseZoom.performed -= HandleMouseScroll;
            controls.Disable();
        }
    }
}
