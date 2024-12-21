using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;
    private Transform initialCameraPosition;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] public GameObject center;
    public float minVerticalAngle = 10.0f; // Minimum vertical rotation angle
    public float maxVerticalAngle = 80.0f; // Maximum vertical rotation angle

    [Header("Layers")]
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask blockedLayer;


    private Vector3 lastPosition;

    

    public event Action OnClicked;
    public event Action OnExit;

    private void Awake()
    {
        // Singleton
       if (Instance && Instance != this)
       {
           Destroy(this);
       }
       else
       {
           Instance = this;
       }
       
    }

    private void Start()
    {
        
    }

    private void Update() //into Building System
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetMouseButtonDown(1))
            OnExit?.Invoke();
        //we dont need this movement shit
      
        zoom();
        move();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SettingsManager.Instance)
            {
                SettingsManager.Instance.ToggleSettingsPanel(false);
            }
        }

        //rest camera
        //just hard coded change later
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainCamera.transform.position = new Vector3(-30.2f,29.4f, -29.3f);
            mainCamera.fieldOfView = 12.6f;
            mainCamera.transform.LookAt(center.transform.position);
        }

    }

    private void move()
    {   
        // Rotation
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalRotation = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            mainCamera.transform.RotateAround(center.transform.position, Vector3.up, horizontalRotation);
            mainCamera.transform.RotateAround(center.transform.position, mainCamera.transform.right, verticalRotation);


            // Make sure the camera is not going under the plane
            Vector3 cameraPosition = mainCamera.transform.position;
            if (cameraPosition.y < center.transform.position.y)
            {
                cameraPosition.y = 0f;
                mainCamera.transform.position = cameraPosition;

                mainCamera.transform.LookAt(center.transform.position);
            }
            
        }
    }

    private void zoom()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        //ignore small changes
        if (Mathf.Abs(scrollDelta) > Mathf.Epsilon)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPositionBeforeZoom = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.y) //topdown view
            ));

            // Adjust the field of view or orthographic size based on the zoom
            mainCamera.fieldOfView = Mathf.Clamp(
                mainCamera.fieldOfView - scrollDelta * zoomSpeed,
                minZoom,
                maxZoom
            );

            // Convert the mouse position to world space again after zooming
            Vector3 mouseWorldPositionAfterZoom = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.y)
            ));

            // Calculate the offset and move the camera to compensate
            Vector3 cameraOffset = mouseWorldPositionBeforeZoom - mouseWorldPositionAfterZoom;
            mainCamera.transform.position += cameraOffset;
        }
    }


    public static bool IsPointOverUI()
        => EventSystem.current.IsPointerOverGameObject();


    public Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 mousePos = Input.mousePosition;
        
        mousePos.z = mainCamera.nearClipPlane;

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        //Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit))
        {
            lastPosition = hit.point;
        }
        
        return lastPosition;
    }
    
}
