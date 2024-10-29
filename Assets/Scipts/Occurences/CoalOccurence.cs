using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// For example coal mining spot
public class CoalOccurence : EnergyOccurence
{   
    public override double GiveResources(){
        if (amount > 1){
            double resourcesToBeReturned =  energyType.GetEnergyPerUnit();
            amount--;
            return resourcesToBeReturned;
        } else {
            return 0;
        }
    }

    void Start(){
    
    }
}
