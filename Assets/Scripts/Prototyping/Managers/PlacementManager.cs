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

    

    

    // Cell Indicator
    [Header("Cell Indicator")]
    private SpriteRenderer cellSprite;
    [SerializeField] private Color spriteColorRegular;
    [SerializeField] private Color spriteColorWarning;
    [SerializeField] private Color spriteColorConnecting;

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
        return currentGameObject != null;
    }

    public void Abort()
    {
        if (Placing())
        {
            if (currentGameObject != null)
            {
               ResetCurrentGameObject(); 
            }
            
            BuilderInventory.Instance.ShowInventory();
            UIManager.Instance.HideOverlay();
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
        switch(currentGameObject.GetComponentInChildren<ProducerDescriptor>().powerPlantType){
            case PowerPlantType.COALPLANT:
                UIManager.Instance.ShowOverlay(OverlayType.COAL);
                break;
            case PowerPlantType.WINDMILL:
                UIManager.Instance.ShowOverlay(OverlayType.WIND);
                break;
            case PowerPlantType.HYDROPOWER:
                UIManager.Instance.ShowOverlay(OverlayType.WATER);
                break;
            case PowerPlantType.SOLARPANEL:
                UIManager.Instance.ShowOverlay(OverlayType.SUN);
                break;
            default:
                Debug.Log("No type");
                break;
        }
        
        SelectionManager.Instance.ClearSelection();
        if (currentGameObject.GetComponent<ProducerDescriptor>())
        {
            validNewPlacement = true;
            cellIndicator.SetActive(true);
            currentGameObject.layer = 2;
            
            BuilderInventory.Instance.HideInventory();
            //UIManager.Instance.HideOverlay();
            
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

        if(blocked || !currentGameObject){
            return;
        }
        

        UIManager.Instance.HideOverlay();
        ProducerDescriptor producerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
        producerDescriptor.Place(lastHoveredTileData);

        

        //insert into gridData array
        Vector3 powerplantPos = producerDescriptor.gameObject.transform.position;
        Vector3Int cellPos = Grid.WorldToCell(powerplantPos);
        cellPos = GridDataManager.ConvertGridPosToArrayPos(cellPos);
        cellPos.z = 1;
        GridDataManager.SetGridDataAtPos(cellPos, producerDescriptor.gameObject);
        

        
        

        /* Handle Tile */
        lastHoveredTileData.setPlacementType(PlacementType.Blocked);
        lastHoveredTileData.SetCurrentBuilding(producerDescriptor);
        
        SetGeneratingValue(true);

        currentGameObject = null;
        //reset CellIndicator
        cellSprite.color = spriteColorRegular;
        cellIndicator.SetActive(false);
        //UIManager.Instance.DeactivateConnectingMode();

        //InputManager.Instance.OnClicked += StopPlacement;
        InputManager.Instance.OnClicked -= PlaceStructure;
        BuilderInventory.Instance.ShowInventory();
    }

    private void SetGeneratingValue(bool setInWandler){
        if(!currentGameObject || lastHoveredTileData == null){
            return;
        }
        float productionValue = currentGameObject.GetComponentInChildren<Wandler>().generating;
        ProducerDescriptor producerDescriptor = currentGameObject.GetComponent<ProducerDescriptor>();
        //Replacement of old modification using name
        //Enum is found on the bottom of this script
        switch (producerDescriptor.powerPlantType)
        {
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
            case PowerPlantType.GAS:
                break;
            case PowerPlantType.NUCLEAR:
                break;
            default:
                Debug.LogWarning("powerPlantType enum not set in powerplant.");
                break;
        }
        if(setInWandler){
            currentGameObject.GetComponentInChildren<Wandler>().generating = productionValue;
        }
        
        currentGameObject.GetComponentInChildren<Wandler>().tagTree.SetProductionText(productionValue);
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

        UIManager.Instance.HideOverlay();

        placingObjectIndex = -1;
        cellIndicator.SetActive(false);
        InputManager.Instance.OnClicked -= PlaceStructure;
        InputManager.Instance.OnClicked -= StopPlacement;
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
        
        
        // Powerplant has been selected from inventory and is glued to the mouse 
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
            //Get Tile Data below the mouse and set cell sprite and output preview accordingly
            Vector3Int arrPosition= GridDataManager.GetArrayPositionAtMousePosition();
            if(arrPosition.z>=0){
                lastHoveredTileData = GridDataManager.GetGridDataAtPos(arrPosition).GetComponent<TileDataWrapper>().tileData;
            }
            if(lastHoveredTileData != null){
                PlacementType groundType = lastHoveredTileData.GetCurrentPlacementType();
                blocked = !currentPlacementType.Equals(groundType);
                cellSprite.color = blocked ? spriteColorWarning : spriteColorRegular;
                SetGeneratingValue(false);
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
        Color old = cellSprite.color;
        cellSprite.color = flickerColor;
        yield return new WaitForSeconds(0.05f);
        cellSprite.color = old;
    }
    
    
}
public enum PowerPlantType{
    NOTSELECTED,
    COALPLANT,
    WINDMILL,
    HYDROPOWER,
    SOLARPANEL,
    GAS,
    NUCLEAR
}
