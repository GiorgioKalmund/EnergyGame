using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] public GameObject center;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [Header("Rotation and Movement")]
    //Movement dont put dragSpeed to fast sonst vibriert es
    private float dragSpeed = 0.85f;
    private Vector3 cameraStartPosition;
    //private float min = -35;
    //private float max = 25;

    //Rotation
    //pls manuelly set the rang to -10 and 10
    [SerializeField] private float rotationSpeed = 150f;
    public float verticalMinLimit = -10f;
    public float verticalMaxLimit = 10f;
    private float currentVerticalAngle = 0f;

    private Vector3 dragOrigin;
    private bool isDragging;

    private Vector3 lastPosition;

    private Camera mainCamera;

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
        mainCamera = UIManager.Instance.sceneCamera;
    }

    private void Update() //into Building System
    {
        
        
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetMouseButtonDown(1))
        {
            
            // TODO: Differentiate between reset if already started connecting, or simply canceling all-together
            // Exit out of current mode if connecting
            if (UIManager.Instance)
                UIManager.Instance.ResetMode();
            
            OnExit?.Invoke();
        }
    
        zoom();
        move();

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
        
        if (IsPointOverUI())
            return;
        
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

            //newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, min, max);
            //newCameraPosition.z = Mathf.Clamp(newCameraPosition.z, min, max);

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
        if (IsPointOverUI())
            return;
        
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

        if (Physics.Raycast(ray, out hit))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
}
