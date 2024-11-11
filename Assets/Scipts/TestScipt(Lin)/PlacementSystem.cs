using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

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

    [SerializeField] [CanBeNull] private GameObject currentGameObject = null;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)//placement which is linked with Inventory
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        
        gridOnOff.SetActive(true);
        cellIndicator.SetActive(true);
        currentGameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);

        inputManager.OnClicked += PlaceStructure;
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
    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridOnOff.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
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
    }
}
