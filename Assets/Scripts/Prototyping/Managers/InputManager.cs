using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] public GameObject pivot;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    [Header("Rotation and Movement")]
    //Movement dont put dragSpeed to fast sonst vibriert es
    public float dragSpeed;
    private Vector3 cameraStartPosition;
    
    
    [SerializeField] private float rotationSpeed = 150f;
    public float verticalMinLimit = -20f;
    public float verticalMaxLimit = 10f;
    private float currentVerticalAngle = 0f;

    private Vector3 lastPosition;

    private Camera mainCamera;

    public event Action OnClicked;

    private Vector3 resetCameraPosition;
    private Vector3 resetPivotPosition;

  

    public InputMap InputMap;
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
        InputMap = new InputMap();
        InputMap.Mouse.Enable();
        resetCameraPosition = mainCamera.transform.position;
        resetPivotPosition = pivot.transform.position;
    }

    private void OnDisable()
    {
        InputMap.Mouse.Disable();
    }

    private void Update() //into Building System
    {
        if (InputMap.Mouse.LeftClick.IsPressed())
            OnClicked?.Invoke();
        if (InputMap.Mouse.RightClick.IsPressed())
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            reset();
        }
            
        

    }
    private void reset()
    {
        mainCamera.transform.position = resetCameraPosition;
        currentVerticalAngle = 0f;
        pivot.transform.position = resetPivotPosition;
        mainCamera.transform.LookAt(pivot.transform.position);
        
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
        //if (IsPointOverUI())
        //    return;

        float distanceToPivot = Vector3.Distance(mainCamera.transform.position, pivot.transform.position);
        float normalizedDistance = Mathf.InverseLerp(minZoom, maxZoom, distanceToPivot);

        //float sensitivity = Mathf.Pow(normalizedDistance, 2); 
        //if still too fast use sensitiveity instead of normalized Dis
        float minDragSpeed = 0.06f;

        float adjustedDragSpeed = Mathf.Max(dragSpeed * normalizedDistance, minDragSpeed);

        //WASD as input
        float moveX = Mathf.Clamp(Input.GetAxis("Horizontal") * adjustedDragSpeed, -1f, 1f);
        float moveZ = Mathf.Clamp(Input.GetAxis("Vertical") * adjustedDragSpeed, -1f, 1f);


        //relative movement 
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // apply movement
        Vector3 moveDirection = forward * moveZ + right * moveX;
        //Vector3 moveDirection = forward *moveInputs.y + right * moveInputs.x;

        Vector3 newPosition = pivot.transform.position + moveDirection;
        Vector3 constrainedPosition = pivot.transform.position;

        if (IsWithinValidGameField(new Vector3(newPosition.x, pivot.transform.position.y, pivot.transform.position.z)))
            constrainedPosition.x = newPosition.x;
        if (IsWithinValidGameField(new Vector3(pivot.transform.position.x, pivot.transform.position.y, newPosition.z)))
            constrainedPosition.z = newPosition.z;

        //check if valid
        if (float.IsNaN(newPosition.x) || float.IsNaN(newPosition.z))
        {
            Debug.LogWarning("Invalid movement detected: NaN values encountered.");
            return;
        }

        // Update position if any axis is valid
        if (constrainedPosition != pivot.transform.position)
        {
            Vector3 cameraOffset = mainCamera.transform.position - pivot.transform.position;
            pivot.transform.position = constrainedPosition;
            mainCamera.transform.position = pivot.transform.position + cameraOffset;
        }






        // Rotation around pivot
        if (Input.GetMouseButton(2)) // left mouse button for rotation, M: Why was here IsPointOverUI anyways?
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
