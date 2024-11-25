using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Unity.VisualScripting;
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
       Debug.Log("Input Manager Started");
    }

    private void Start()
    {
        
    }

    private void Update() //into Building System
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }


    public bool IsPointOverUI()
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
    

    public void CheckForSelection()
    {
        if (IsPointOverUI())
        {
            return;
        }
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            Debug.Log("Found selectable Object");
            TileData selectedTileData = hit.transform.gameObject.GetComponent<TileDataWrapper>().tileData;
            BuildingDescriptor buildingDescriptor = selectedTileData.currentBuilding;
            Debug.Log("Clicked on tile: "+selectedTileData.coords);
            Debug.Log("Building exists: : "+buildingDescriptor);
            if (buildingDescriptor && !buildingDescriptor.IsSelected())
            {
                Debug.Log("Selected "+buildingDescriptor.buildingName);
                SelectionManager.Instance.Select(buildingDescriptor);
            }
            else
            {
                Debug.Log("Cleared Selection");
                SelectionManager.Instance.ClearSelection();
            }
        }
    }
}
