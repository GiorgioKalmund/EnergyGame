using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoalPlant : EnergyProducer
{

    // Start is called before the first frame update
    void Start()
    {
        print("Coal plant appeared");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        double produced = this.Produce();
        print(title + " produced: "+produced);
        energyStored += produced;
    }
    
    override protected double Produce(){
        double producedRespources = energyIn.GiveResources();
        print(energyIn.title + " produces "+ producedRespources);
        return energyIn.GiveResources() * efficiency;
    }
}
