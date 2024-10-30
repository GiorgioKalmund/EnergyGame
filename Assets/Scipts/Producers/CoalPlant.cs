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

    protected override double Produce(){
        double producedRespources = source.GiveResources();
        return producedRespources * efficiency;
    }

    void SetText()
    {
        textElement.text = $@"
            ${title}

            Stored: $ {energyStored}
            #Available: $ {source.GetAmount()} * ${source.energyType}
        ";
    }
}
