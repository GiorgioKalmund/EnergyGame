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
    [SerializeField] public GameObject pivot;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 50f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 80f;

    [Header("Rotation and Movement")]
    //Movement dont put dragSpeed to fast sonst vibriert es
    private float dragSpeed = 0.6f;
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
            mainCamera.transform.position = new Vector3(-30.2f, 29.4f, -30.2f);
            pivot.transform.position = new Vector3(8f, 0f, 8f);
            mainCamera.transform.LookAt(pivot.transform.position);
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



            //code belong is for the version where u zoom into the current mouse position
            //Vector3 mouseScreenPosition = Input.mousePosition;
            //Vector3 mouseWorldPositionBeforeZoom = mainCamera.ScreenToWorldPoint(new Vector3(
            //    mouseScreenPosition.x,
            //    mouseScreenPosition.y,
            //    Mathf.Abs(mainCamera.transform.position.y) // Top-down view
            //));

            // instead using fov move camera position toward it 
            //Vector3 direction = (mouseWorldPositionBeforeZoom - mainCamera.transform.position).normalized;
            //float zoomAmount = scrollDelta * zoomSpeed;

            //mainCamera.transform.position += direction * zoomAmount;

            // Clamp the camera position to ensure it stays within the zoom bounds
            //float distanceToGround = Mathf.Abs(mainCamera.transform.position.y);
            //distanceToGround = Mathf.Clamp(distanceToGround, minZoom, maxZoom);
            //mainCamera.transform.position = new Vector3(
            //    mainCamera.transform.position.x,
            //    -distanceToGround, // Keep top-down view with correct height
            //    mainCamera.transform.position.z
            //);

            // Recalculate the mouse position in world space after the camera moves
            //Vector3 mouseWorldPositionAfterZoom = mainCamera.ScreenToWorldPoint(new Vector3(
            //    mouseScreenPosition.x,
            //    mouseScreenPosition.y,
            //    Mathf.Abs(mainCamera.transform.position.y)
            //));

            // Offset the camera on the mouse position
            //Vector3 cameraOffset = mouseWorldPositionBeforeZoom - mouseWorldPositionAfterZoom;
            //mainCamera.transform.position += cameraOffset;


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
