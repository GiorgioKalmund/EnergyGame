using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// For example a coal-fired power plant
public abstract class EnergyProducer : MonoBehaviour
{
    [SerializeField] protected EnergyOccurence source;
    [SerializeField] protected double efficiency = 1;
    [SerializeField] public string title = "";

    //[SerializeField] protected string coordinate;

    [SerializeField] protected double energyStored = 0;
    protected double energyOut = 0;

    protected abstract double Produce();

}
