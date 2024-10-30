using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class CoalPlant : EnergyProducer
{


    // Start is called before the first frame update
    void Start()
    {
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        double produced = this.Produce();
        energyStored += produced;
        SetText();
    }

    protected new double Produce(){
        double producedRespources = source.GiveResources();
        return producedRespources * efficiency;
    }

    void SetText()
    {
        textElement.text = $@"
            {title}

            Source:  {source}
            Energy Type: {source.GetEnergyType()}
            Energy per Unit: {source.GetEnergyType().GetEnergyPerUnit()}

            Units Available: {source.GetAmount()} * {source.GetEnergyType().GetEnergyPerUnit()}

            Stored: {energyStored}
        ";
    }
}
