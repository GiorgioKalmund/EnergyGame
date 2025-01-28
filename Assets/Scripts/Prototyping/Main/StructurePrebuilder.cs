using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StructurePrebuilder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _windmillPrefab;
    [SerializeField] private GameObject _cablePrefab;

    private GameObject _windmill1;
    private GameObject _windmill2;
    private GameObject _cable;
    
    void Start()
    {
       
         _windmill1 = Instantiate(_windmillPrefab,new Vector3(0,PlacementManager.Instance.cellIndicatorPlacementY,0),Quaternion.Euler(-90,180,180));
        _windmill2 = Instantiate(_windmillPrefab, new Vector3(1,PlacementManager.Instance.cellIndicatorPlacementY,0),Quaternion.Euler(-90,180,180));
        
        GridDataManager.SetGridDataAtPos(new Vector3Int(0,0,1),_windmill1);
        GridDataManager.SetGridDataAtPos(new Vector3Int(1,0,1),_windmill2);
        GridDataManager.GetGridDataAtPos(new Vector3Int(0, 0, 0)).GetComponent<TileDataWrapper>().tileData.setPlacementType(PlacementType.Blocked);
        
        StartCoroutine(ConnectWindmills());
    }

    
    private IEnumerator ConnectWindmills(){

        yield return new WaitForEndOfFrame();GridDataManager.GetGridDataAtPos(new Vector3Int(1, 0, 0)).GetComponent<TileDataWrapper>().tileData.setPlacementType(PlacementType.Blocked);
        //_cable = Instantiate(_cablePrefab,Vector3.zero,Quaternion.identity);

        Wandler windmill1Wandler = _windmill1.GetComponent<Wandler>();
        Wandler windmill2Wandler = _windmill2.GetComponent<Wandler>();
        
        windmill1Wandler.tagTree.SetProductionText(windmill1Wandler.generating);
        windmill2Wandler.tagTree.SetProductionText(windmill2Wandler.generating);
            
        ConnectCableMode.Instance.SetStartpoint(_windmill1);
        ConnectCableMode.Instance.SetEnpoint(GridDataManager.GetGridDataAtPos(new Vector3Int(2,2,1)));
        _cable =ConnectCableMode.Instance.PlaceCable();
        _cable.GetComponent<Wandler>().addInputWandler(windmill1Wandler);
        _cable.GetComponent<Wandler>().addOutputWandler(GridDataManager.GetGridDataAtPos(new Vector3Int(2,2,1)).GetComponent<Wandler>());
        GraphManager.Instance.calculateAll();
        _cable.GetComponent<LineRenderer>().enabled = true;

        
    }
}
