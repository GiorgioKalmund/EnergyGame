using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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

    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private LayerMask blockedLayer;
    [SerializeField] private Sprite defaultSelector;
    [SerializeField] private Sprite warningSelector;
    [SerializeField] [CanBeNull] private GameObject currentGameObject = null;
    private SpriteRenderer cellSprite;
    [SerializeField] private bool blocked;

    private void Start()
    {
        StopPlacement();
        if (cellIndicator)
        {
            cellSprite = cellIndicator.GetComponentInChildren<SpriteRenderer>();
            Debug.Log(cellSprite);
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
        else
        {
            gameObject.transform.position = grid.CellToWorld(gridPosition);
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
        selectedObjectIndex = -1;
        gridOnOff.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= ResetCurrentGameObject;
        inputManager.OnExit -= StopPlacement;
        currentGameObject = null;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        mouseIndicator.transform.position = mousePosition;
        Vector3 targetPostion = grid.CellToWorld(gridPosition);
        cellIndicator.transform.position = new Vector3(targetPostion.x + 0.5f, 0.2f, targetPostion.z + 0.5f);
        if (currentGameObject)
        {
            currentGameObject.transform.position = cellIndicator.transform.position;
        }

        RaycastHit hit;
        if (Physics.Raycast(mouseIndicator.transform.position, Vector3.down, out hit, 10f))
        {
            LayerMask hitLayer = hit.transform.gameObject.layer;
            // TODO: Actually compare layers, based on parameterized values up top
            if (hitLayer.value == 0)
            {
                blocked = false;
                cellSprite.sprite = defaultSelector;
            }
            else if (hitLayer.value == 4 || hitLayer.value == 6)
            {
                blocked = true;
                cellSprite.sprite = warningSelector;
            }
        }
    }
}
