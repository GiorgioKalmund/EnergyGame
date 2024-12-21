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
    [SerializeField] public GameObject center;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [Header("Rotation and Movement")]
    //Movement
    public float dragSpeed = 2;

    //Rotation
    //pls manuelly set the rang to -10 and 10
    [SerializeField] private float rotationSpeed = 150f;
    public float verticalMinLimit = 30f;   
    public float verticalMaxLimit = 70f; 
    private float currentVerticalAngle = 0f;

    private Vector3 dragOrigin;
    private bool isDragging;

    


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
        //just hard coded 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainCamera.transform.position = new Vector3(-30.2f,29.4f, -29.3f);
            mainCamera.fieldOfView = 12.6f;
            mainCamera.transform.LookAt(center.transform.position);
        }

    }

    private void move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            // Origin point
            Vector3 originWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(dragOrigin.x, dragOrigin.y, mainCamera.nearClipPlane));
            // Point after move
            Vector3 currentWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, mainCamera.nearClipPlane));

            Vector3 movement = (originWorldPoint - currentWorldPoint)*dragSpeed;

            // Apply the movement to the camera
            mainCamera.transform.position += new Vector3(movement.x, 0, movement.z);   

        }
        // Rotation
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalRotation = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            float newVerticalAngle = currentVerticalAngle + verticalRotation;

            // Rotation limitation in Y direction
            newVerticalAngle = Mathf.Clamp(newVerticalAngle, verticalMinLimit, verticalMaxLimit);

            //X
            mainCamera.transform.RotateAround(center.transform.position, Vector3.up, horizontalRotation);
            //Y
            mainCamera.transform.RotateAround(center.transform.position, mainCamera.transform.right, newVerticalAngle - currentVerticalAngle);
            
            currentVerticalAngle = newVerticalAngle;

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
