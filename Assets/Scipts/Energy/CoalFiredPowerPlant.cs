using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalFiredPowerPlant
{
    private Coal coal;
    private Heat heat;
    private Electricity electricity;
    private Conversion conversion;

    public CoalFiredPowerPlant(Vector3 position, int coalChargeRate, float efficiency)
    {
        this.coal = new Coal(position, 0, 0, 0);
        this.heat = new Heat(position, 0, 0, int.MaxValue);
        this.electricity = new Electricity(position, 0, 0, 0);

        Energy[] chargers = {coal};
        Energy[] drainers = {heat, electricity};
        int[] chargeRates = {coalChargeRate};
        int[] drainRates = { (int)(coalChargeRate * (1 - efficiency)), (int)(coalChargeRate * efficiency) };
        this.conversion = new Conversion(chargers, drainers, chargeRates, drainRates);
    }

    public Coal GetCoal()
    {
        return coal;
    }

    public Heat GetHeat() 
    { 
        return heat;
    }

    public Electricity GetElectricity()
    {
        return electricity;
    }
    public Conversion GetConversion()
    {
        return conversion;
    }

}

