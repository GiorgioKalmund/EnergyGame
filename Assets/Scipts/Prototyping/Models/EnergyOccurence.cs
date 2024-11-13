using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


// For example coal mining spot
public abstract class EnergyOccurence : MonoBehaviour
{
    [SerializeField] public String title = "";
    [SerializeField] protected EnergyType energyType;
    [SerializeField] protected double amount = 0;

    public abstract double GiveResources();

    public bool IsEmpty(){
        return amount <= 0;
    }

    public void SetAmount(double a){
        this.amount = a;
    }

    public double GetAmount(){
        return this.amount;
    }

    public EnergyType GetEnergyType()
    {
        return  this.energyType;
    }
}
