using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

// For example a coal-fired power plant
public abstract class EnergyProducer : MonoBehaviour
{
    [SerializeField] protected EnergyOccurence source;
    [SerializeField] protected double efficiency = 1;
    [SerializeField] public string title = "";
    //[SerializeField] protected string coordinate;

    [SerializeField] protected double energyStored = 0;

    protected double Produce()
    {
        double producedRespources = source.GiveResources();
        return producedRespources * efficiency;
    }

    public double GetEnergyOut(double amount)
    {
        double difference = this.energyStored - amount;
        if (difference < 0)
        {
            return 0;
        }
        return difference;
    }

    public double GetEfficiency()
    {
        return this.efficiency;
    }


}
