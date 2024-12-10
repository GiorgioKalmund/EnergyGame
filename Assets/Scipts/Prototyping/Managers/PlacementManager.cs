using System;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;


public class PlacementManager : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabase database;
    private int placingObjectIndex = -1;
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float gridOffset;

    // TODO: Check using these instead of numbers, however currently numbers to do match :/
    [Header("Layers")] 
    [SerializeField] private LayerMask defaultLayer; 
    [SerializeField]private LayerMask waterLayer; 
    [SerializeField] private LayerMask blockedLayer;

    [Header("Cable")] 
    [SerializeField] private GameObject cablePrefab;

    [SerializeField] private float cableEffLossPerUnit = 0.04f;

    private PowerCable lastPlacedCable;

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
     public float cellIndicatorPlacementY = 1.51f;
    
    private GameObject lastPlacedBuilding;
    private TileData lastHoveredTileData;
    private bool citySelectionActive;
    private GameObject ground = null;

    private bool validNewPlacement = false;
    
    public static PlacementManager Instance { get; private set; }


    //private int coalCounter = 0;
    //[SerializeField] private GameObject notification;


    private void Awake()
    {
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
        StopPlacement();
        if (cellIndicator)
        {
            cellSprite = cellIndicator.GetComponentInChildren<SpriteRenderer>();
        }
        connectingModeIndicatorImage.SetActive(false);
    }

    public void StartPlacement(int ID)
    {
        
        if (currentGameObject)
        {
            StopPlacement();
            return;
        }
        StopPlacement();
        //placement which is linked with Inventory
        placingObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        currentGameObject = Instantiate(database.objectsData[placingObjectIndex].Prefab);
        if (!currentGameObject)
        {
            throw new Exception("GameObject could not be instantiated!");
        }
        SelectionManager.Instance.ClearSelection();
        if (currentGameObject.GetComponent<ProducerDescriptor>())
        {
            validNewPlacement = true;
            cellIndicator.SetActive(true);
            currentGameObject.layer = 2;
            ProducerDescriptor producerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
            if (!BudgetManager.Instance.CanHandleCost(producerDescriptor.GetCost()))
            {
                StopPlacement();
                Debug.LogError("Insufficient budget!");
                producerDescriptor.Destroy();
                return;
            }
            InputManager.Instance.OnClicked += PlaceStructure;
            InputManager.Instance.OnExit += ResetCurrentGameObject;
            InputManager.Instance.OnExit += StopPlacement;
        } 
    }



    private void PlaceStructure()
    {

        // After a building is placed, we want to select a city to connect
        if (currentGameObject && !blocked)
        {
            ProducerDescriptor producerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
            if (producerDescriptor.Place(lastHoveredTileData)) // second check, however, might not be needed as this is check in StartPlacmement
            {
                //after first coal is placed we decrease enviroment impact and give a notification
                /*
                if (producerDescriptor.GetName() == "Cole")
                {
                    coalCounter +=  1;
                    if(coalCounter == 1)
                    {
                        notification.SetActive(true);
                        LevelManager.Instance.SetMaxEnvironmentalImpact(-100);
                        UIManager.Instance.UpdateMaxEnvironmentalImpact();
                    }
                }
                */
                /* Handle Building */
                lastPlacedBuilding = currentGameObject;
                
                /* Handle Tile */
                lastHoveredTileData.setPlacementType(PlacementType.Blocked);
                lastHoveredTileData.SetCurrentBuilding(producerDescriptor);
                currentGameObject = null;

                // Prepare for citySelection
                citySelectionActive = true;
                //Debug.Log("Building placed. Please select a city.");
                InventoryManager.Instance.UpdateInventorySlots(); 
                InventoryManager.Instance.HideInventory(); 
                cellSprite.color = spriteColorConnecting;
                connectingModeIndicatorImage.SetActive(true);
                
                // Instantiate new cable
                GameObject cable = Instantiate(cablePrefab, lastPlacedBuilding.transform.position, Quaternion.identity);
                lastPlacedCable = cable.GetComponent<PowerCable>();
                producerDescriptor.AddCable(lastPlacedCable);
                
                InputManager.Instance.OnClicked += SelectCity;
            }
            else
            {
                Debug.LogError("Object somehow managed to pass invalid money check!");
            }
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
            if (hitTransform.GetComponent<TileDataWrapper>().tileData.GetCurrentPlacementType()== PlacementType.Endpoint) // City layer
            {
                citySelectionActive = false;
                Vector3 cityPosition = cellIndicator.transform.position;
                Vector3 buildingPosition = lastPlacedBuilding.transform.position;

                //caculate the relative dis between city and powerPlant
                float distance = Vector3.Distance(buildingPosition, cityPosition);

                //lastPlacedBuilding is the powerPlant
                float productionValue = producerDescriptor.GetMaxProduction();
                
                // TODO: THIS IS NOT THE WAY TO DO IT, maybe an Enum for BuildingType?
                if (producerDescriptor.buildingName == "Windmill")
                {
                    //TODO kommt später weg
                    //lastHoveredTileData.windSpeed += 0.1f;
                    productionValue *= lastHoveredTileData.windSpeed;

                } 
                else if (producerDescriptor.buildingName == "Hydropower")
                {
                    //lastHoveredTileData.waterSpeed += 0.1f;
                    //Debug.Log("Applied a debuff of "+lastHoveredTileData.waterSpeed + " to "+producerDescriptor.buildingName);
                    productionValue *= lastHoveredTileData.waterSpeed;
                }
                else if (producerDescriptor.buildingName == "Coal Plant")
                {
                    productionValue *= lastHoveredTileData.coalAmount;
                } else if (producerDescriptor.buildingName == "Solar Panels")
                {
                    productionValue *= lastHoveredTileData.sunlightHours;
                }

                //Debug.Log($"alpha = {lastHoveredTileData.waterSpeed}");

                //cost for cable
                // TODO: This is not being returned when sold
                // BudgetManager.Instance.UseBudget(distance);

                lastPlacedCable.Place();
                
                float effectiveLoss = Mathf.Pow((1 - cableEffLossPerUnit), distance);
                distance *= effectiveLoss;
                productionValue -= distance;
                productionValue = Mathf.Max(0, productionValue);
                producerDescriptor.SetProduction(productionValue);
                LevelManager.Instance.AddProduce(productionValue);

                //Debug.Log($"City selected. Distance to building: {distance} units.");
                if (!InventoryManager.Instance.IsEmpty())
                {
                    InventoryManager.Instance.ShowInventory();
                }

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

        
        placingObjectIndex = -1;
        cellIndicator.SetActive(false);
        InputManager.Instance.OnClicked -= PlaceStructure;
        InputManager.Instance.OnExit -= ResetCurrentGameObject;
        InputManager.Instance.OnExit -= StopPlacement;
        currentGameObject = null;
        validNewPlacement = false; 
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
        if (currentGameObject && validNewPlacement)
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
                    PlacementType groundType = tileData.GetCurrentPlacementType();
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

    public Transform GetCellIndicatorTransform()
    {
        return cellIndicator.transform;
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

