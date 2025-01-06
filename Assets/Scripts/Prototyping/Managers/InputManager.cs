using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] public GameObject pivot;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [Header("Rotation and Movement")]
    //Movement dont put dragSpeed to fast sonst vibriert es
    private float dragSpeed = 0.85f;
    private Vector3 cameraStartPosition;
    
    
    [SerializeField] private float rotationSpeed = 150f;
    public float verticalMinLimit = -10f;
    public float verticalMaxLimit = 10f;
    private float currentVerticalAngle = 0f;

    private Vector3 dragOrigin;
    private bool isDragging;

    private Vector3 lastPosition;

    private Camera mainCamera;

    public event Action OnClicked;

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
            if (!PlacementManager.Instance.Placing() && UIManager.Instance.Mode == UIState.DEFAULT)
                return;
            
            // TODO: Differentiate between reset if already started connecting, or simply canceling all-together
            // Exit out of current mode if connecting
            if (PlacementManager.Instance)
                PlacementManager.Instance.Abort();
            if (UIManager.Instance)
                UIManager.Instance.ResetMode();
        }
    
        zoom();
        move();

        //rest camera
        //just hard coded 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainCamera.transform.position = new Vector3(-30.2f, 29.4f, -29.3f);
            mainCamera.fieldOfView = 12.6f;
            mainCamera.transform.LookAt(pivot.transform.position);
            currentVerticalAngle = 0f;
        }

    }

    private bool IsWithinValidGameField(Vector3 position)
    {
        //check if pivot is collide with any gamefield objects
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.5f);
        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<TileDataWrapper>())
            {
                return true;
            }
        }
        return false;
    }

    private void move()
    {
        //WASD as input
        float moveX = Input.GetAxis("Horizontal") * dragSpeed;
        float moveZ = Input.GetAxis("Vertical") * dragSpeed;

        //relative movement 
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // apply movement
        Vector3 moveDirection = forward * moveZ + right * moveX;


        Vector3 newPosition = pivot.transform.position + moveDirection;

        // check if new position is still in field
        if (IsWithinValidGameField(newPosition))
        {
            Vector3 cameraOffset = mainCamera.transform.position - pivot.transform.position;
            pivot.transform.position = newPosition;
            //update cam
            mainCamera.transform.position = pivot.transform.position + cameraOffset;
        }



        // Rotation around pivot
        if (Input.GetMouseButton(0)) // left mouse button for rotation
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalRotation = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            float newVerticalAngle = currentVerticalAngle + verticalRotation;

            // Rotation limitation in Y direction
            newVerticalAngle = Mathf.Clamp(newVerticalAngle, verticalMinLimit, verticalMaxLimit);

            //X
            mainCamera.transform.RotateAround(pivot.transform.position, Vector3.up, horizontalRotation);
            //Y
            mainCamera.transform.RotateAround(pivot.transform.position, mainCamera.transform.right, newVerticalAngle - currentVerticalAngle);

            currentVerticalAngle = newVerticalAngle;

        }
    }


    private void zoom()
    {
        if (IsPointOverUI())
            return;

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        // Ignore small changes
        if (Mathf.Abs(scrollDelta) > Mathf.Epsilon)
        {
            Vector3 directionToPivot = (pivot.transform.position - mainCamera.transform.position).normalized;
            float zoomAmount = scrollDelta * zoomSpeed * Time.deltaTime;

            Vector3 newCameraPosition = mainCamera.transform.position + directionToPivot * zoomAmount * 100;

            // Abstand zwischen Kamera und Pivot berechnen
            float distanceToPivot = Vector3.Distance(newCameraPosition, pivot.transform.position);

            distanceToPivot = Mathf.Clamp(distanceToPivot, minZoom, maxZoom);
            // Aktualisiere die Kameraposition basierend auf dem geclampeden Abstand
            mainCamera.transform.position = pivot.transform.position - directionToPivot * distanceToPivot;

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
