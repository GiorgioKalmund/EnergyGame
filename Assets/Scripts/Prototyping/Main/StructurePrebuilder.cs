using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StructurePrebuilder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _windmill1;
    [SerializeField] private GameObject _windmill2;

    [SerializeField] private GameObject _cable;
    [SerializeField] private GameObject _endpoint;
    void Start()
    {

        _windmill1.GetComponent<Wandler>().tagTree.SetProductionText(19);
        _windmill2.GetComponent<Wandler>().tagTree.SetProductionText(19);
        _cable.GetComponent<Wandler>().generating = 16;
        _cable.GetComponent<Wandler>().tagTree.SetProductionText(16);

        GridDataManager.SetGridDataAtPos(new Vector3Int(0,0,1),_windmill1);
        GridDataManager.SetGridDataAtPos(new Vector3Int(1,0,1),_windmill2);
        GridDataManager.GetGridDataAtPos(new Vector3Int(0, 0, 0)).GetComponent<TileDataWrapper>().tileData.setPlacementType(PlacementType.Blocked);
        GridDataManager.GetGridDataAtPos(new Vector3Int(1, 0, 0)).GetComponent<TileDataWrapper>().tileData.setPlacementType(PlacementType.Blocked);
        //GridDataManager.GetGridDataAtPos(new Vector3Int(2,2,1)).GetComponent<Wandler>().generating = 16;

        StartCoroutine(connectEndpoint());
    }

    private IEnumerator connectEndpoint(){
        while(!SceneManager.GetActiveScene().isLoaded){
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        _cable.GetComponent<Wandler>().addOutputWandler(_endpoint.GetComponent<Wandler>());
        _endpoint.GetComponent<Wandler>().ComputeInput();

        GridDataManager.SetGridDataAtPos(new Vector3Int(2,2,1),_endpoint);
        GridDataManager.GetGridDataAtPos(new Vector3Int(2, 2, 0)).GetComponent<TileDataWrapper>().tileData.setPlacementType(PlacementType.Blocked);
    }
}
