using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalToElectricity : Level
{
    // Hard-code content of this particular level!
    public CoalToElectricity()
    {
        Coal coal = this.coal(Vector3.left, 1000, 0, 1000);
        CoalFiredPowerPlant coalFiredPowerPlant = this.coalFiredPowerPlant(Vector3.zero, 10, 0.5f);
        Conversion coalTransport = this.conversion(new Energy[] { coal }, new Energy[] { coalFiredPowerPlant.GetCoal() },
            new int[] { 10 }, new int[] { 10 });
        Electricity electricity = this.electricity(Vector3.right, 0, 0, 1000);
        Conversion electricityTransport = this.conversion(new Energy[] { coalFiredPowerPlant.GetElectricity() },
            new Energy[] { electricity }, new int[] { 5 }, new int[] { 5 });
    }

}
