using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class ConnectCableMode : MonoBehaviour
{
    [SerializeField] private GameObject powerTowerPrefab;
    [SerializeField] private GameObject cablePrefab;
    private float cableEfficiencyLossPerUnit = 0.04f;
    private GameObject startpoint;
    private GameObject endpoint;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets the start and end points of the cable depending on the <c>isStartpoint</c> parameter, but does not place the cable itself.
    /// </summary>
    /// <param name="isStartpoint">Sets the startpoint if set to true, else the endpoint</param>
    private void SetConnectionPoints(bool isStartpoint)
    {
        Grid grid = PlacementManager.Instance.Grid;
        Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
        Vector3Int gridPosition = grid.WorldToCell(mousePos + new Vector3(0.5f, 0, 0.5f));
        Vector3Int arrPosition = GridDataManager.ConvertGridPosToArrayPos(gridPosition);

        if (GridDataManager.GetGridDataAtPos(arrPosition).GetComponent<TileDataWrapper>().tileData.currentPlacementType == PlacementType.Blocked)
        {
            return;
        }

        GameObject candidate = GridDataManager.GetGridDataAtPos(new Vector3Int(arrPosition.x, arrPosition.y, 1));
        if (candidate != null) //powerplant is present
        {
            if (isStartpoint)
            {
                startpoint = candidate;
            }
            else
            {
                endpoint = candidate;
            }
        }
        else //powerplant is not present
        {
            GameObject powerTower = Instantiate(powerTowerPrefab, gridPosition, Quaternion.identity);
            if (isStartpoint)
            {
                startpoint = powerTower;
            }
            else
            {
                endpoint = powerTower;
            }

            GameObject tileBelow = GridDataManager.GetGridDataAtPos(arrPosition);
            tileBelow.GetComponent<TileDataWrapper>().tileData.currentPlacementType = PlacementType.Blocked;

        }

    }

    [Obsolete("Prints the position the mouse is at in the gridData array")]
    private void Test()
    {
        Grid grid = PlacementManager.Instance.Grid;
        Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
        Vector3Int gridPosition = grid.WorldToCell(mousePos + new Vector3(0.5f, 0, 0.5f));
        Debug.Log($"Grid Pos: ${GridDataManager.ConvertGridPosToArrayPos(gridPosition)} + ");
        /* RaycastHit hit;
        Camera mainCamera = Camera.main;
        Vector3 mousePosition = InputManager.Instance.GetMousePositionInWorldSpace();
        Grid Grid = PlacementManager.Instance.Grid;
        if(Physics.Raycast(mainCamera.transform.position, mousePosition - mainCamera.transform.position, out hit, Mathf.Infinity, 1)){
            Vector3Int gridPosition = Grid.WorldToCell(hit.transform.gameObject.transform.position);
            Vector3 targetPostion = Grid.CellToWorld(gridPosition); 
        } */
    }
}
