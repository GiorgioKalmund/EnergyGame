using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("Layers")]
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask blockedLayer;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;

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
       Debug.Log("Input Manager Started");
    }

    private void Start()
    {
       ChangeCursor();
    }

    private void Update() //into Building System
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
        ChangeCursor();
    }


    public bool IsPointOverUI()
        => EventSystem.current.IsPointerOverGameObject();


    public Vector3 GetMousePositionInWorldSpace()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, 100, defaultLayer))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
    
    private void ChangeCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorDownTexture, Vector2.zero, CursorMode.Auto);
            return;
        }
        Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
    }

    public void CheckForSelection()
    {
        if (IsPointOverUI())
        {
            return;
        }
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, blockedLayer))
        {
            Debug.Log("Found selectable Object");
            GameObject selectedGameObject = hit.collider.gameObject;
            BuildingDescriptor buildingDescriptor = selectedGameObject.GetComponent<BuildingDescriptor>();
            if (buildingDescriptor)
            {
                SelectionManager.Instance.Select(buildingDescriptor);
            }
            else
            {
                SelectionManager.Instance.ClearSelection();
            }
        }
    }
}
