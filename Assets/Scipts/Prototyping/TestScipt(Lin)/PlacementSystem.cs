using System;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;
using Scipts.Prototyping.TestScipt_Lin_;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabase database;
    private int placingObjectIndex = -1;
    
    // TODO: Possibly outsource?
    [Header("Cables")]
    private LineRenderer _lineRenderer;
    [SerializeField] private int lineVertexCount;
    [SerializeField] private float lineFunctionDivisor = 64;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float gridOffset;

    // TODO: Check using these instead of numbers, however currently numbers to do match :/
    [Header("Layers")] 
    [SerializeField] private LayerMask defaultLayer; 
    [SerializeField]private LayerMask waterLayer; 
    [SerializeField] private LayerMask blockedLayer;

    // Cell Indicator
    [Header("Cell Indicator")]
    private SpriteRenderer cellSprite;
    [SerializeField] private Color spriteColorRegular;
    [SerializeField] private Color spriteColorWarning;
    [SerializeField] private Color spriteColorConnecting;
    private bool _flickering;

    [Header("Cursor")]
    [SerializeField]
    private Texture2D cursorDefaultTexture, cursorDownTexture;
    [CanBeNull] private GameObject currentGameObject = null;
    [SerializeField] private bool blocked;
    // TODO: UI Should be handled via some type of UI Manager
    [SerializeField] private GameObject connectingModeIndicatorImage;

    /// <changes>
    private GameObject lastPlacedBuilding = null;
    private bool citySelectionActive = false;
    /// </changes>

    private void Start()
    {
        StopPlacement();
        if (cellIndicator)
        {
            cellSprite = cellIndicator.GetComponentInChildren<SpriteRenderer>();
            Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
        }
        connectingModeIndicatorImage.SetActive(false);

    }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = lineVertexCount;
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        //placement which is linked with Inventory
        placingObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        cellIndicator.SetActive(true);
        currentGameObject = Instantiate(database.objectsData[placingObjectIndex].Prefab);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += ResetCurrentGameObject;
        inputManager.OnExit += StopPlacement;
    }



    private void PlaceStructure()
    {

        //Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //after a building is placed, we want to select a city to connect
        if (currentGameObject && !blocked)
        {
            currentGameObject.GetComponent<BuildingDescriptor>().Place();

            lastPlacedBuilding = currentGameObject;
            currentGameObject = null;

            //prepare for citySelection
            citySelectionActive = true;
            Debug.Log("Building placed. Please select a city.");
            cellSprite.color = spriteColorConnecting;
            connectingModeIndicatorImage.SetActive(true);
            inputManager.OnClicked += SelectCity;
        }

    }
    //Select City current is at layer 6 -> TODO: adding new layer and change it to the city layer
    //TODO: while clicking on a powerPlant highlight the linked City, also opposite way
    private void SelectCity()
    {
        if (!citySelectionActive)
        {
            cellSprite.color = spriteColorRegular;
            connectingModeIndicatorImage.SetActive(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(mouseIndicator.transform.position, Vector3.down, out hit, 10f))
        {
            if (hit.transform.gameObject.layer == 6) // City layer
            {
                citySelectionActive = false;
                Vector3 cityPosition = cellIndicator.transform.position;
                Vector3 buildingPosition = lastPlacedBuilding.transform.position;

                //caculate the relative dis between city and powerPlant
                float distance = Vector3.Distance(buildingPosition, cityPosition);

                //lastPlacedBuilding is the powerPlant
                if (lastPlacedBuilding)
                {
                    BuildingDescriptor buildingDescriptor = lastPlacedBuilding.GetComponent<BuildingDescriptor>();
                    if (buildingDescriptor != null)
                    {
                        float productionValue = buildingDescriptor.GetProduction();
                        LevelController.Instance.AddProduce(productionValue, distance);
                    }
                }

                Debug.Log($"City selected. Distance to building: {distance} units.");

                //turn curser off
                cellSprite.color = spriteColorRegular;
                cellIndicator.SetActive(false);
                connectingModeIndicatorImage.SetActive(false);
                inputManager.OnClicked -= SelectCity;
            }
            else
            {
                Debug.Log("Please select a valid city.");
                if (!_flickering)
                {
                    StartCoroutine(FlickerIndicator(spriteColorWarning));
                }
            }
        }
    }

    private void ResetCurrentGameObject()
    {
        Destroy(currentGameObject);
        currentGameObject = null;
    }
    private void StopPlacement()
    {
        if (blocked && currentGameObject)
        {
            Debug.Log("You are not allowed to place here!");
            return;
        }

        if (currentGameObject)
        {
            currentGameObject.GetComponent<BuildingDescriptor>().Place();
        }
        placingObjectIndex = -1;
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= ResetCurrentGameObject;
        inputManager.OnExit -= StopPlacement;
        currentGameObject = null;
    }

    private void ChangeCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorDownTexture, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    private void Update()
    {
        // TODO: Make reactive instead of busy waiting
        ChangeCursor();
        // if (placingObjectIndex < 0)
        //     return;
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 targetPostion = grid.CellToWorld(gridPosition); 
        cellIndicator.transform.position = new Vector3(targetPostion.x + gridOffset, 0.03f, targetPostion.z + gridOffset);
        // If we are placing 
        if (currentGameObject)
        {
           currentGameObject.transform.position =  Vector3.Lerp(currentGameObject.transform.position, cellIndicator.transform.position, Time.deltaTime * 50f);
            BuildingDescriptor buildingDescriptor = currentGameObject.GetComponent<BuildingDescriptor>();
            if (!buildingDescriptor)
            {
                throw new MissingComponentException($"{currentGameObject.name} requires  BuildingDescriptor.");
            }
            PlacementType currentPlacementType = currentGameObject.GetComponent<BuildingDescriptor>().Placement;
            RaycastHit hit;
            if (Physics.Raycast(mouseIndicator.transform.position, Vector3.down, out hit, 10f))
            {
                LayerMask hitLayer = hit.transform.gameObject.layer;
                // TODO: Actually compare layers, based on parameterized values up top
                if (hitLayer.value == 6)
                {
                    blocked = true;
                    cellSprite.color = spriteColorWarning;
                }
                else
                {
                    if (currentPlacementType.Equals(PlacementType.Water))
                    {
                        blocked = hitLayer.value != 4;
                        cellSprite.color = blocked ? spriteColorWarning : spriteColorRegular;
                    }
                    else
                    {
                        blocked = hitLayer.value != 0; 
                        cellSprite.color = blocked ? spriteColorWarning : spriteColorRegular;
                    }
                }
            }
        }
        else // If we are strolling & selecting
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit,  blockedLayer))
            {
                
                    Debug.Log("Found selectable Object");
                    GameObject selectedGameObject = hit.collider.gameObject;
                    BuildingDescriptor buildingDescriptor = selectedGameObject.GetComponent<BuildingDescriptor>();
                    if (buildingDescriptor)
                    {
                        SelectionManager.Instance.Select(buildingDescriptor);
                    }
            }
        }
    }
    
    void LateUpdate() {
        DrawCable();
    }
    
    void DrawCable()
    {
        if (!citySelectionActive)
        {
            return;
        }
        Vector3 startPos = lastPlacedBuilding.transform.position + (Vector3.up * 0.3f);
        Vector3 endPos = cellIndicator.transform.position + (Vector3.up * 0.3f);
        Vector3 direction = endPos - startPos;
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(lineVertexCount - 1 , endPos);
        int half = lineVertexCount / 2;
        for (int index = 1; index < lineVertexCount - 1; index++)
        {
            Vector3 pointToDraw = startPos + (index * direction / lineVertexCount);
            pointToDraw.y += MathIsMathin((index - half) / lineFunctionDivisor) - MathIsMathin((half) / lineFunctionDivisor);
            _lineRenderer.SetPosition(index, pointToDraw);
        }
    }

    float MathIsMathin(float x)
    {
        return Mathf.Sqrt(Mathf.Pow(x, 2f) + 1);
        //return Mathf.Sin(x);
    }
    
    IEnumerator FlickerIndicator(Color flickerColor)
    {
        _flickering = true;
        Color old = cellSprite.color;
        cellSprite.color = flickerColor;
        yield return new WaitForSeconds(0.05f);
        cellSprite.color = old;
        _flickering = false;
    }
}

