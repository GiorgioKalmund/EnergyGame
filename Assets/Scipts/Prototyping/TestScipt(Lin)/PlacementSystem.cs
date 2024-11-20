using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;


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
    private int selectedObjectIndex = -1;


    [SerializeField] private float gridOffset;

    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask blockedLayer;
    [SerializeField] private Sprite defaultSelector;
    [SerializeField] private Sprite warningSelector;
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;
    [SerializeField] [CanBeNull] private GameObject currentGameObject = null;
    private SpriteRenderer cellSprite;
    [SerializeField] private bool blocked;

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
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        //placement which is linked with Inventory
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        cellIndicator.SetActive(true);
        currentGameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += ResetCurrentGameObject;
        inputManager.OnExit += StopPlacement;
    }



    private void PlaceStructure()
    {

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //after a building is placed, we want to select a city to connect
        if (currentGameObject && !blocked)
        {
            currentGameObject.GetComponent<BuildingDesriptor>().Place();

            lastPlacedBuilding = currentGameObject;
            currentGameObject = null;

            //prepare for citySelection
            citySelectionActive = true;
            Debug.Log("Building placed. Please select a city.");
            inputManager.OnClicked += SelectCity;
        }

    }
    //Select City current is at layer 6 -> TODO: adding new layer and change it to the city layer
    //TODO: while clicking on a powerPlant highlight the linked City, also opposite way
    private void SelectCity()
    {
        if (!citySelectionActive) return;

        RaycastHit hit;
        if (Physics.Raycast(mouseIndicator.transform.position, Vector3.down, out hit, 10f))
        {
            if (hit.transform.gameObject.layer == 6) // City layer
            {
                GameObject selectedCity = hit.transform.gameObject;
                citySelectionActive = false;

                Vector3 cityPosition = selectedCity.transform.position;
                Vector3 buildingPosition = lastPlacedBuilding.transform.position;

                //caculate the relative dis between city and powerPlant
                float distance = Vector3.Distance(buildingPosition, cityPosition);

                //lastPlacedBuilding is the powerPlant
                if (lastPlacedBuilding)
                {
                    BuildingDesriptor buildingDescriptor = lastPlacedBuilding.GetComponent<BuildingDesriptor>();
                    if (buildingDescriptor != null)
                    {
                        float productionValue = buildingDescriptor.production;
                        LevelController.Instance.AddProduce(productionValue, distance);
                    }
                }
               


                Debug.Log($"City selected. Distance to building: {distance} units.");

                inputManager.OnClicked -= SelectCity;
            }
            else
            {
                Debug.Log("Please select a valid city.");
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
            currentGameObject.GetComponent<BuildingDesriptor>().Place();
        }
        selectedObjectIndex = -1;
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
        ChangeCursor();
        if (selectedObjectIndex < 0)
            return;
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        mouseIndicator.transform.position = mousePosition;
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 targetPostion = grid.CellToWorld(gridPosition); 
        cellIndicator.transform.position = new Vector3(targetPostion.x + gridOffset, 0.03f, targetPostion.z + gridOffset);
        if (currentGameObject)
        {
           currentGameObject.transform.position =  Vector3.Lerp(currentGameObject.transform.position, cellIndicator.transform.position, Time.deltaTime * 50f);
            BuildingDesriptor buildingDescriptor = currentGameObject.GetComponent<BuildingDesriptor>();
            if (!buildingDescriptor)
            {
                throw new MissingComponentException($"{currentGameObject.name} requires  BuildingDescriptor.");
            }
            PlacementType currentPlacementType = currentGameObject.GetComponent<BuildingDesriptor>().Placement;
            RaycastHit hit;
            if (Physics.Raycast(mouseIndicator.transform.position, Vector3.down, out hit, 10f))
            {
                LayerMask hitLayer = hit.transform.gameObject.layer;
                // TODO: Actually compare layers, based on parameterized values up top
                if (hitLayer.value == 6)
                {
                    blocked = true;
                    cellSprite.sprite = warningSelector;
                }
                else
                {
                    if (currentPlacementType.Equals(PlacementType.Water))
                    {
                        blocked = hitLayer.value != 4;
                        cellSprite.sprite = blocked ? warningSelector : defaultSelector;
                    }
                    else
                    {
                        blocked = hitLayer.value != 0; 
                        cellSprite.sprite = blocked ? warningSelector : defaultSelector;
                    }
                }
            }
        }
        
    }
}
