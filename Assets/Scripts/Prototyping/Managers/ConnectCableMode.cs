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

    private bool isStartpoint = true;
    public static ConnectCableMode Instance;
    public float cableYOffset = 0.5f;
    private GameObject cachedCable;
    private PowerCable cachedCableScript;

    private GameObject cachedPowerTower;
    
    void Awake(){
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        cachedCable = Instantiate(cablePrefab, new Vector3(0,-1000,0), Quaternion.identity);
        cachedCableScript = cachedCable.GetComponent<PowerCable>();
        cachedPowerTower = Instantiate(powerTowerPrefab,new Vector3(0,-1000,0),Quaternion.identity);
        
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && UIManager.Instance.Mode == UIState.CONNECTING){
            SetConnectionPoints();
            //Test();
        }
    }

    /// <summary>
    /// Sets the start and end points of the cable depending on the <c>isStartpoint</c> parameter, and connects the cable between them if both are set.
    /// </summary>
    /// <param name="isStartpoint">Sets the startpoint if set to true, else the endpoint</param>
    public void SetConnectionPoints()
    {
        if(InputManager.IsPointOverUI()){
            return;
        }
        
        Vector3Int arrPosition = GridDataManager.GetArrayPositionAtMousePosition();
        if(arrPosition.z <0){
            return;
        }


        /* //Kein wandler im tile oder oben drauf weil Endpoint ist tile aber hat Wandler
        if ((GridDataManager.GetGridDataAtPos(arrPosition).GetComponentInChildren<Wandler>()== null
          && GridDataManager.GetGridDataAtPos(new Vector3Int(arrPosition.x,arrPosition.y,1)) == null))
        {
            //Wenn tile auch noch geblocked (Wald etc.) dann ungültig sonst ok
            if(GridDataManager.GetGridDataAtPos(arrPosition).GetComponent<TileDataWrapper>().tileData.currentPlacementType != PlacementType.Default){
                

                return; 
            }
            
        } */
        
        GameObject candidate = GridDataManager.GetGridDataAtPos(new Vector3Int(arrPosition.x, arrPosition.y, 1));
        //Verhindert Kraftwerk an Kraftwerk zu schließen
        /* if(candidate && !candidate.tag.Contains("PowerTower") && !isStartpoint) return;
        //Workaround für Endpoint ist ein Tile und nicht in index 1 in grid data
        if(GridDataManager.GetGridDataAtPos(arrPosition).GetComponentInChildren<Wandler>()){
            candidate = GridDataManager.GetGridDataAtPos(arrPosition);

        } */
        if (candidate != null) //powerplant is present
        {
            if (isStartpoint)
            {
                startpoint = candidate;
                StartDrawingCableAfterStartpointIsSet();
            }
            else
            {

                endpoint = candidate;
            }
        }
        else if(GridDataManager.GetGridDataAtPos(arrPosition).GetComponent<TileDataWrapper>().tileData.currentPlacementType == PlacementType.Default) //powerplant is not present
        {
            GameObject tileBelow = GridDataManager.GetGridDataAtPos(arrPosition);
            Vector3 powerTowerPos = tileBelow.transform.position;
            powerTowerPos.y = PlacementManager.Instance.cellIndicatorPlacementY;
            
            GameObject powerTower = cachedPowerTower;
            powerTower.transform.position = powerTowerPos;
            
            cachedPowerTower = Instantiate(powerTowerPrefab,new Vector3(0,-1000,0),Quaternion.identity);
            if (isStartpoint)
            {
                startpoint = powerTower;
                StartDrawingCableAfterStartpointIsSet();
            }
            else
            {
                endpoint = powerTower;
            }
            GridDataManager.SetGridDataAtPos(new Vector3Int(arrPosition.x,arrPosition.y,1),powerTower);
            
            tileBelow.GetComponent<TileDataWrapper>().tileData.currentPlacementType = PlacementType.Blocked;

            GraphManager.Instance.wandlerArray[GraphManager.Instance.numOfWandler++] = powerTower.GetComponent<Wandler>();

        } else{
            return;
        }
        if(isStartpoint){
            isStartpoint = false;
        } else{
            Debug.Log(endpoint.tag +":"+ endpoint.name);
            
            isStartpoint = true;
            if(startpoint == endpoint){
                Debug.Log("Connect cable to same object, no operation. ");

                return;
            } 
            else if(startpoint.GetComponentInChildren<Wandler>().Endpoint && endpoint.GetComponentInChildren<Wandler>().Endpoint){
                Debug.Log("Connect cable between two Endpoints, no operation. ");

                return;
            }
            else{
                PlaceCable();
                if(endpoint.CompareTag("Endpoint")){
                    UIManager.Instance.DeactivateConnectingMode();
                }
            }
            
        }
    }
    
    public void ResetCablesAfterExitingMode(){
        isStartpoint = true;
        cachedCable.GetComponent<LineRenderer>().enabled = false;
        cachedCableScript.placed = false; //saves calculations


    }
    public GameObject PlaceCable(){
        
        
        GameObject cable = cachedCable;
        PowerCable cableScript = cable.GetComponent<PowerCable>();
        Wandler cableWandler = cable.GetComponent<Wandler>();

        
        cableScript.startPos = startpoint.GetComponent<ProducerDescriptor>().cableAnchor.position;
        cableScript.endPos = endpoint.GetComponent<ProducerDescriptor>().cableAnchor.position;
        
        
        
        float distance = Vector3.Distance(startpoint.transform.position, endpoint.transform.position);
        float effectiveLoss = Mathf.Pow(1 - cableEfficiencyLossPerUnit, distance);
        cableWandler.efficiency = effectiveLoss;

        GraphManager.Instance.wandlerArray[GraphManager.Instance.numOfWandler++] = cableWandler;

        GraphManager.Instance.ConnectWandler(startpoint.GetComponentInChildren<Wandler>(),cableWandler);
        GraphManager.Instance.ConnectWandler(cableWandler,endpoint.GetComponentInChildren<Wandler>());
        cableScript.Place();
        cableScript.DrawCable(); 
        
        
        cachedCable = Instantiate(cablePrefab, new Vector3(0,-1000,0), Quaternion.identity);
        cachedCableScript = cachedCable.GetComponent<PowerCable>();
        return cable;
    }
    private void StartDrawingCableAfterStartpointIsSet(){
        cachedCableScript.placed = false;
        cachedCableScript.startPos = startpoint.GetComponent<ProducerDescriptor>().cableAnchor.position;
        cachedCable.GetComponent<LineRenderer>().enabled = true;
    }
    public void SetStartpoint(GameObject start){
        startpoint = start;
    }
    public void SetEnpoint(GameObject end){
        endpoint = end;
    }
    [Obsolete("Prints the position the mouse is at in the gridData array")]
    private void Test()
    {
        Grid grid = PlacementManager.Instance.Grid;
        Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
        Vector3Int gridPosition = grid.WorldToCell(mousePos + new Vector3(0.5f, 0, 0.5f));
        gridPosition = GridDataManager.ConvertGridPosToArrayPos(gridPosition);
        Debug.Log($"Grid Pos: ${gridPosition}");
        
        Destroy(GridDataManager.GetGridDataAtPos(gridPosition));
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
