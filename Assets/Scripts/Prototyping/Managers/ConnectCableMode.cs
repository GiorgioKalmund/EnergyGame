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
        if(Input.GetMouseButtonDown(0)){
            test();
        }
    }
    /// <summary>
    /// Sets the startpoint variable to the powerplant at the position of the cursor.
    /// </summary>
    private void SetStartpoint()
    {

        Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position,mousePos,out hit))
        {

            Wandler wandler = hit.transform.gameObject.GetComponentInParent<Wandler>();
            if (!wandler)
            {
                Grid grid = PlacementManager.Instance.Grid;
                Vector3Int gridPosition = grid.WorldToCell(hit.transform.gameObject.transform.position);
                Vector3 targetPostion = grid.CellToWorld(gridPosition);
                targetPostion.x += PlacementManager.Instance.GridOffset;
                targetPostion.z += PlacementManager.Instance.GridOffset;
                targetPostion.y += PlacementManager.Instance.cellIndicatorPlacementY;
                startpoint = Instantiate(powerTowerPrefab, targetPostion, Quaternion.identity);
            }
            else
            {
                startpoint = hit.transform.gameObject;
            }
        }
    }

    private void test(){
        Grid grid = PlacementManager.Instance.Grid;
        Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
        Vector3Int gridPosition = grid.WorldToCell(mousePos+new Vector3(0.5f,0,0.5f));
        Debug.Log($"Grid Pos: ${GridDataManager.ConvertGridPostoArrayPos(gridPosition)} + ");
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
