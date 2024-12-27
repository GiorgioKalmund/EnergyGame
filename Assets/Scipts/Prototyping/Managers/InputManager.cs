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
    //Movement dont put dragSpeed to fast sonst vibriert es
    private float dragSpeed = 0.85f;
    private Vector3 cameraStartPosition;

    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    //Rotation
    //pls manuelly set the rang to -10 and 10
    [SerializeField] private float rotationSpeed = 150f;
    public float verticalMinLimit = -10f;
    public float verticalMaxLimit = 10f;
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
        minX = -35f; 
        maxX = -15f;  
        minZ = -35f; 
        maxZ = -15f;  
    }

    private void Update() //into Building System
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetMouseButtonDown(1))
            OnExit?.Invoke();

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
            mainCamera.transform.position = new Vector3(-30.2f, 29.4f, -29.3f);
            mainCamera.fieldOfView = 12.6f;
            mainCamera.transform.LookAt(center.transform.position);
        }

    }

    private void move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = GetMousePositionInWorldSpace();
            cameraStartPosition = mainCamera.transform.position;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            // movement
            Vector3 mouseDelta = (GetMousePositionInWorldSpace() - dragOrigin) * dragSpeed;

            // Change camera based on movement
            Vector3 newCameraPosition = new Vector3(
                cameraStartPosition.x - mouseDelta.x,
                cameraStartPosition.y,
                cameraStartPosition.z - mouseDelta.z
            );


            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, minX, maxX);
            newCameraPosition.z = Mathf.Clamp(newCameraPosition.z, minZ, maxZ);


            mainCamera.transform.position = newCameraPosition;

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
        // Ignore small changes
        if (Mathf.Abs(scrollDelta) > Mathf.Epsilon)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPositionBeforeZoom = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.y) // Top-down view
            ));

            // instead using fov move camera position toward it 
            Vector3 direction = (mouseWorldPositionBeforeZoom - mainCamera.transform.position).normalized;
            float zoomAmount = scrollDelta * zoomSpeed;

            mainCamera.transform.position += direction * zoomAmount;

            // Clamp the camera position to ensure it stays within the zoom bounds
            float distanceToGround = Mathf.Abs(mainCamera.transform.position.y);
            distanceToGround = Mathf.Clamp(distanceToGround, minZoom, maxZoom);
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x,
                -distanceToGround, // Keep top-down view with correct height
                mainCamera.transform.position.z
            );

            // Recalculate the mouse position in world space after the camera moves
            Vector3 mouseWorldPositionAfterZoom = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.y)
            ));

            // Offset the camera on the mouse position
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

        if (Physics.Raycast(ray, out hit))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }


    

}
