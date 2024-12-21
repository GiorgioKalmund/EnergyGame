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

    public void RemoveCable(PowerCable cable)
    {
        cable.Sell();
    }

    public void AddCable(PowerCable cable)
    {
        cableList.Add(cable);
    }

}