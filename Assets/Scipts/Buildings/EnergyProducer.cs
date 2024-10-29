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
    protected EnergyOccurence energyIn;
    protected double energyOut;
    private double efficiency;
    protected String title;
    protected String coordinate;

    public EnergyProducer(String title, String coordinate, EnergyOccurence energyIn, double efficiency){
        this.title = title;
        this.coordinate = coordinate;
        this.energyIn = energyIn;
        this.efficiency = efficiency;
        
        energyOut = 0;
    }

    protected abstract double Produce();

    void Update(){

    }

}
