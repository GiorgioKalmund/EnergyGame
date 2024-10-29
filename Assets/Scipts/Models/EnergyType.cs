using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For example coal
public abstract class EnergyType : MonoBehaviour
{
    [SerializeField] public String title;
    [SerializeField] protected double energyPerUnit;

    public double GetEnergyPerUnit(){
        return this.energyPerUnit;
    }
}
