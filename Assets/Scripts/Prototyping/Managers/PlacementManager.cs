using System;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;
using Unity.VisualScripting;


public class PlacementManager : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    
    [SerializeField] public Grid Grid;
    [SerializeField] public float GridOffset;
    [SerializeField]
    private ObjectsDatabase database;
    private int placingObjectIndex = -1;
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    

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
    public float cellIndicatorPlacementY = 1.51f;
    
    private GameObject lastPlacedBuilding;
    private TileData lastHoveredTileData;
    private bool citySelectionActive;
    private GameObject ground = null;

    private bool validNewPlacement = false;
    
    public static PlacementManager Instance { get; private set; }


    //private int coalCounter = 0;
    //[SerializeField] private GameObject notification;

    public bool Placing()
    {
        return currentGameObject != null || lastPlacedBuilding != null && citySelectionActive;
    }

    public void Abort()
    {
        if (Placing())
        {
            if (currentGameObject != null)
            {
               ResetCurrentGameObject(); 
            }
            else
            {
                lastPlacedBuilding.GetComponent<ProducerDescriptor>().Sell();
                lastPlacedBuilding = null;
                currentGameObject = null;
                placingObjectIndex = -1;
                cellSprite.color = spriteColorRegular;
                cellIndicator.SetActive(false);
                UIManager.Instance.ToggleConnectionModeIndicator(false);
                InputManager.Instance.OnClicked -= SelectCity;
            }
            BuilderInventory.Instance.ShowInventory();
        }
    }

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
    }

    public void StartPlacement(int ID)
    {
        if (currentGameObject)
        {
            BuilderInventory.Instance.HideInventory(); 
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
            
            BuilderInventory.Instance.HideInventory();
            
            InputManager.Instance.OnClicked += PlaceStructure;
        } 
        BuilderInventory.Instance.RemoveSlotCapacity(1, placingObjectIndex);
    }



    private void PlaceStructure()
    {
        if (InputManager.IsPointOverUI())
        {
            return;
        }
        // After a building is placed, we want to select a city to connect
        if (currentGameObject && !blocked)
        {
            ProducerDescriptor producerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
            producerDescriptor.Place(lastHoveredTileData);

            //insert into gridData array
            Vector3 powerplantPos = producerDescriptor.gameObject.transform.position;
            Vector3Int cellPos =Grid.WorldToCell(powerplantPos);
            cellPos = GridDataManager.ConvertGridPosToArrayPos(cellPos);
            cellPos.z = 1;
            GridDataManager.GridData[cellPos.x,cellPos.y,cellPos.z] = producerDescriptor.gameObject; 
            
            Debug.Log($"Inserted ${producerDescriptor.powerPlantType} onto ${cellPos}");

            /* Handle Building */
            lastPlacedBuilding = currentGameObject;
            
            /* Handle Tile */
            lastHoveredTileData.setPlacementType(PlacementType.Blocked);
            lastHoveredTileData.SetCurrentBuilding(producerDescriptor);
            currentGameObject = null;
            // Prepare for citySelection
            //citySelectionActive = true;
            //Debug.Log("Building placed. Please select a city.");
            //cellSprite.color = spriteColorConnecting;
            //UIManager.Instance.ActiveConnectingMode();
            
            // Instantiate new cable
            /* GameObject cable = Instantiate(cablePrefab, lastPlacedBuilding.transform.position, Quaternion.identity);
            lastPlacedCable = cable.GetComponent<PowerCable>();
            producerDescriptor.AddCable(lastPlacedCable); */

            InputManager.Instance.OnClicked += SelectCity;
            /* if(cable.GetComponent<Wandler>() != null){
                Wandler cwandler = cable.GetComponent<Wandler>();
                //Debug.Log("Got cable Wandler!");
                cwandler.onStartConnectTo = lastPlacedBuilding.GetComponent<Wandler>();
                //Debug.Log("Connected Cable to plant!");
            } */
        }

    }
    //Select City current is at layer 6 -> TODO: adding new layer and change it to the city layer
    //TODO: while clicking on a powerPlant highlight the linked City, also opposite way
    private void SelectCity()
    {
        if (!citySelectionActive)
        {
            cellSprite.color = spriteColorRegular;
            //UIManager.Instance.ToggleConnectionModeIndicator(false);
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
                float productionValue = lastPlacedBuilding.GetComponentInChildren<Wandler>().generating;
                
                
                //Replacement of old modification using name
                //Enum is found on the bottom of this script
                switch(producerDescriptor.powerPlantType){
                    case PowerPlantType.COALPLANT:
                        productionValue *= lastHoveredTileData.coalAmount;
                        break;
                    case PowerPlantType.WINDMILL:
                        productionValue *= lastHoveredTileData.windSpeed;
                        break;
                    case PowerPlantType.HYDROPOWER:
                        productionValue *= lastHoveredTileData.waterSpeed;
                        break;
                    case PowerPlantType.SOLARPANEL:
                        productionValue *= lastHoveredTileData.sunlightHours;
                        break;
                    default:
                        Debug.LogWarning("powerPlantType enum not set in powerplant.");
                        break;
                }
                lastPlacedBuilding.GetComponentInChildren<Wandler>().generating = productionValue;

                //Debug.Log($"alpha = {lastHoveredTileData.waterSpeed}");

                //cost for cable
                // TODO: This is not being returned when sold
                // BudgetManager.Instance.UseBudget(distance);

                /* if(hitTransform.GetComponentInChildren<Wandler>() != null){
                    //Debug.Log(hitTransform.GetComponentInChildren<Wandler>());
                    Wandler endWandler = hitTransform.GetComponentInChildren<Wandler>();
                    //TODO REMOVE Endpoint addition
                    //GraphManager.Instance.Endpoints[GraphManager.Instance.numOfEndpoints++] = endWandler;
                    //--------------------------------------
                    lastPlacedCable.GetComponent<Wandler>().addOutputWandler(endWandler);
                }
                else{
                    Debug.Log(hit.transform.name + " does not have the Wandler Component");
                }
                
                float effectiveLoss = Mathf.Pow((1 - cableEffLossPerUnit), distance);
                lastPlacedCable.GetComponentInChildren<Wandler>().efficiency = effectiveLoss;

                //von Wandler ersetzt
                distance *= effectiveLoss;
                productionValue -= distance;
                productionValue = Mathf.Max(0, productionValue);
                producerDescriptor.SetProduction(productionValue);
                LevelManager.Instance.AddProduce(productionValue);


                lastPlacedCable.Place(); */

                //Debug.Log($"City selected. Distance to building: {distance} units.");

                //reset CellIndicator
                cellSprite.color = spriteColorRegular;
                cellIndicator.SetActive(false);
                UIManager.Instance.DeactivateConnectingMode();
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
        if (!currentGameObject)
        {
            return;
        }
        
        int instanceId = currentGameObject.GetComponent<ProducerDescriptor>().instanceId;
        Debug.Log("Resetting Instance: " + instanceId + " OBJ: " + currentGameObject.GetComponent<ProducerDescriptor>().buildingName);
        Destroy(currentGameObject);
        currentGameObject = null; 
        placingObjectIndex = -1;
        cellIndicator.SetActive(false);
        validNewPlacement = false; 
        BuilderInventory.Instance.AddSlotCapacity(1, instanceId);
    }

    private void StopPlacement()
    {
        if (blocked && currentGameObject)
        {
            Debug.Log("You are not allowed to place here!");
            return;
        }

        if (citySelectionActive)
        {
            Debug.Log("Cannot cancel while in selection mode!");
            return;
        }

        placingObjectIndex = -1;
        cellIndicator.SetActive(false);
        InputManager.Instance.OnClicked -= PlaceStructure;
        currentGameObject = null;
        validNewPlacement = false; 
    }


    private void Update()
    {
        Vector3 mousePosition = InputManager.Instance.GetMousePositionInWorldSpace();
        //Debug.Log($"{mousePosition}");
        mouseIndicator.transform.position = mousePosition;
        RaycastHit hit;
        
        if(Physics.Raycast(mainCamera.transform.position, mousePosition - mainCamera.transform.position, out hit, Mathf.Infinity, 1)){
            Vector3Int gridPosition = Grid.WorldToCell(hit.transform.gameObject.transform.position);
            Vector3 targetPostion = Grid.CellToWorld(gridPosition); 
            cellIndicator.transform.position = new Vector3(targetPostion.x + GridOffset, cellIndicatorPlacementY, targetPostion.z + GridOffset);
        }  
        
        
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
            //RaycastHit hit;
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
            // NOTE: This has been moved to the Update loop of the UI Manager. If any specific behaviour is necessary it should be handled there
            //SelectionManager.Instance.CheckForSelection();
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
public enum PowerPlantType{
    NOTSELECTED,
    COALPLANT,
    WINDMILL,
    HYDROPOWER,
    SOLARPANEL
}
