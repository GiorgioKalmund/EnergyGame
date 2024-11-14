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

    [SerializeField]
    private GameObject gridOnOff;

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

    private void Start()
    {
        StopPlacement();
        if (cellIndicator)
        {
            cellSprite = cellIndicator.GetComponentInChildren<SpriteRenderer>();
            Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    public void StartPlacement(int ID)//placement which is linked with Inventory
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);

        gridOnOff.SetActive(true);
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
        //check if building already exist
        if (currentGameObject)
        {
            StopPlacement();
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
        gridOnOff.SetActive(false);
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
        Vector3 adjustedTargetPosition =  new Vector3(targetPostion.x + gridOffset, 0.03f, targetPostion.z + gridOffset); 
        cellIndicator.transform.position = Vector3.Lerp(cellIndicator.transform.position, adjustedTargetPosition, Time.deltaTime * 50f);
        if (currentGameObject)
        {
           currentGameObject.transform.position = cellIndicator.transform.position;
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
