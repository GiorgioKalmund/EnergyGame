using System;
using System.Collections.Generic;
using UnityEngine;


public class PowerCableManager : MonoBehaviour
{
    public static PowerCableManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public List<PowerCable> cableList;

    public void RemoveCable(Vector3 startPosition)
    {
        PowerCable cableToBeRemoved = cableList.Find((c) => c.IsEqualTo(startPosition));
        if (cableToBeRemoved)
        {
            Debug.Log("Sucessfully found cable to be starting at "+startPosition);
        }
        else
        {
            Debug.LogError("Error deleting at "+startPosition);
        }
        cableList.RemoveAll((c) => c.IsEqualTo(startPosition));
        Destroy(cableToBeRemoved.gameObject);
    }

    public void AddCable(PowerCable cable)
    {
        cableList.Add(cable);
    }

}