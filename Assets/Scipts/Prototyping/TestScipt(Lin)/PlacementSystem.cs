using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;
using Scipts.Prototyping.TestScipt_Lin_;
using Unity.VisualScripting;
using System.IO.Pipes;
using UnityEngine.Serialization;


public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabase database;
    private int placingObjectIndex = -1;
    
    // TODO: Possibly outsource?
    [Header("Cables")]
    private LineRenderer _lineRenderer;
    [SerializeField] private int lineVertexCount = 10;
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

    [CanBeNull] private GameObject currentGameObject;
    [SerializeField] private bool blocked;
    // TODO: UI Should be handled via some type of UI Manager
    [Header("Visuals")]
    [SerializeField] private GameObject connectingModeIndicatorImage;
    [FormerlySerializedAs("buildingPlacementY")] public float cellIndicatorPlacementY = 1.51f;
    
    private GameObject lastPlacedBuilding;
    private TileData lastHoveredTileData;
    private bool citySelectionActive;
    private GameObject ground = null;

    private void Start()
    {
        StopPlacement();
        if (cellIndicator)
        {
            cellSprite = cellIndicator.GetComponentInChildren<SpriteRenderer>();
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
        currentGameObject.layer = 2;

        InputManager.Instance.OnClicked += PlaceStructure;
        InputManager.Instance.OnExit += ResetCurrentGameObject;
        InputManager.Instance.OnExit += StopPlacement;
    }



    private void PlaceStructure()
    {

        //after a building is placed, we want to select a city to connect
        if (currentGameObject && !blocked)
        {
            ProducerDescriptor producerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
            producerDescriptor.Place();
            
            lastPlacedBuilding = currentGameObject;
            lastHoveredTileData.setPlacementType(PlacementType.Blocked);
            Debug.Log("Tile at "+lastHoveredTileData.coords + " should be blocked");
            lastHoveredTileData.setCurrentBuilding(producerDescriptor);
            Debug.Log("Tile at "+lastHoveredTileData.coords + "should have the building: "+producerDescriptor.buildingName);
            currentGameObject = null;

            //prepare for citySelection
            citySelectionActive = true;
            Debug.Log("Building placed. Please select a city.");
            cellSprite.color = spriteColorConnecting;
            connectingModeIndicatorImage.SetActive(true);
            InputManager.Instance.OnClicked += SelectCity;
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

        if (!lastPlacedBuilding)
        {
            return;
        }
        ProducerDescriptor producerDescriptor = lastPlacedBuilding.GetComponent<ProducerDescriptor>();
        RaycastHit hit;

        
        if (Physics.Raycast(cellIndicator.transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 10f))
        {
            Transform hitTransform = hit.transform;
            if (hitTransform.GetComponent<TileDataWrapper>().tileData.placementType == PlacementType.Endpoint) // City layer
            {
                citySelectionActive = false;
                Vector3 cityPosition = cellIndicator.transform.position;
                Vector3 buildingPosition = lastPlacedBuilding.transform.position;

                //caculate the relative dis between city and powerPlant
                float distance = Vector3.Distance(buildingPosition, cityPosition);

                //lastPlacedBuilding is the powerPlant
                float productionValue = producerDescriptor.GetProduction();
                productionValue -= distance;
                LevelController.Instance.AddProduce(productionValue);

                Debug.Log($"City selected. Distance to building: {distance} units.");

                //reset CellIndicator
                cellSprite.color = spriteColorRegular;
                cellIndicator.SetActive(false);
                connectingModeIndicatorImage.SetActive(false);
                InputManager.Instance.OnClicked -= SelectCity;
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
        else
        {
            Debug.LogError("Raycast missed!");
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
            currentGameObject.GetComponent<ProducerDescriptor>().Place();
        }
        placingObjectIndex = -1;
        cellIndicator.SetActive(false);
        
        InputManager.Instance.OnClicked -= PlaceStructure;
        InputManager.Instance.OnExit -= ResetCurrentGameObject;
        InputManager.Instance.OnExit -= StopPlacement;
        currentGameObject = null;
    }


    private void Update()
    {
        Vector3 mousePosition = InputManager.Instance.GetMousePositionInWorldSpace();
        //Debug.Log($"{mousePosition}");
        mouseIndicator.transform.position = mousePosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 targetPostion = grid.CellToWorld(gridPosition); 
        cellIndicator.transform.position = new Vector3(targetPostion.x + gridOffset, cellIndicatorPlacementY, targetPostion.z + gridOffset);
        
        // If we are placing 
        if (currentGameObject)
        {
            
            //currentGameObject.transform.position =  Vector3.Lerp(currentGameObject.transform.position, cellIndicator.transform.position, Time.deltaTime * 50f);
            currentGameObject.transform.position = cellIndicator.transform.position - Vector3.up * 0.01f; 
            ProducerDescriptor ProducerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
            //currentBuilding for placing 
            if (!ProducerDescriptor)
            {
                throw new MissingComponentException($"{currentGameObject.name} requires ProducerDescriptor!");
            }
            
            PlacementType currentPlacementType = ProducerDescriptor.GetPlacementType(); 
            RaycastHit hit;
            if (Physics.Raycast( cellIndicator.transform.position + Vector3.up * 0.2f, Vector3.down, out hit, 10f))
            {
                ground = hit.transform.gameObject;
                if (ground.GetComponent<TileDataWrapper>())
                {
                    TileData tileData = ground.GetComponent<TileDataWrapper>().tileData;
                    lastHoveredTileData = tileData; 
                    PlacementType groundType = tileData.placementType;
                    blocked = !currentPlacementType.Equals(groundType);
                    cellSprite.color = blocked ? spriteColorWarning : spriteColorRegular;
                }
                
            }
        }
        else if (!citySelectionActive) // If we are strolling & selecting
        {
            // TODO
            SelectionManager.Instance.CheckForSelection();
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

