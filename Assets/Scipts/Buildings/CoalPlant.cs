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
    void Update()
    {
        double produced = this.Produce();
        print(title + " produced: "+ produced + " energy units left: "+source.GetAmount());
        energyStored += produced;
    }
    
    protected override double Produce(){
        double producedRespources = source.GiveResources();
        return producedRespources * efficiency;
    }
}
