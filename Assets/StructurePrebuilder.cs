using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePrebuilder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _windmillPrefab;
    [SerializeField] private GameObject _cablePrefab;

    private GameObject windmill1;
    private GameObject _cable;
    private int counter = 0;
    void Start()
    {
        windmill1 = Instantiate(_windmillPrefab,new Vector3(0,PlacementManager.Instance.cellIndicatorPlacementY,0),Quaternion.identity);
        windmill1.transform.rotation = Quaternion.Euler(-90,180,180);
        GameObject windmill2 = Instantiate(_windmillPrefab,new Vector3(1,PlacementManager.Instance.cellIndicatorPlacementY,0),Quaternion.identity);
        windmill2.transform.rotation = Quaternion.Euler(-90,180,180);
        GridDataManager.SetGridDataAtPos(new Vector3Int(0,0,1),windmill1);
        GridDataManager.SetGridDataAtPos(new Vector3Int(1,0,1),windmill2);
        //_cable = Instantiate(_cablePrefab,Vector3.zero,Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < 10){
            counter++;
        }
        else if(counter == 10){
            ConnectCableMode.Instance.SetStartpoint(windmill1);
            ConnectCableMode.Instance.SetEnpoint(GridDataManager.GetGridDataAtPos(new Vector3Int(2,2,1)));
            _cable =ConnectCableMode.Instance.PlaceCable();
            _cable.GetComponent<LineRenderer>().enabled = true;

            ++counter;
        }
    }
}
