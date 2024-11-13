using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Level : MonoBehaviour
{
    private List<Conversion> conversions;
    private List<Energy> energies;

    public Level()
    {
        conversions = new List<Conversion>();
        energies = new List<Energy>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tick();

            // DBG
            string dbg = "";
            foreach (Energy energy in energies)
            {
                dbg += energy.dbg();
            }
            Debug.Log(dbg);
        }
    }

    public void tick()
    {
        foreach (Conversion conversion in conversions)
        {
            conversion.convert();
        }
    }


    // The following methods build and register 'Energy'- and 'Conversion'-instances.
    public CoalFiredPowerPlant coalFiredPowerPlant(Vector3 position, int coalChargeRate, float efficiency)
    {
        CoalFiredPowerPlant coalFiredPowerPlant = new CoalFiredPowerPlant(position, coalChargeRate, efficiency);
        energies.Add(coalFiredPowerPlant.GetCoal());
        energies.Add(coalFiredPowerPlant.GetHeat());
        energies.Add(coalFiredPowerPlant.GetElectricity());
        conversions.Add(coalFiredPowerPlant.GetConversion());
        return coalFiredPowerPlant;
    }

    public Coal coal(Vector3 position, int amount, int min, int max)
    {
        Coal coal = new Coal(position, amount, min, max);
        energies.Add(coal);
        return coal;
    }

    public Heat heat(Vector3 position, int amount, int min, int max)
    {
        Heat heat = new Heat(position, amount, min, max);
        energies.Add(heat);
        return heat;    
    }

    public Electricity electricity(Vector3 position, int amount, int min, int max)
    {
        Electricity electricity = new Electricity(position, amount, min, max);
        energies.Add(electricity);
        return electricity;
    }

    public Conversion conversion(Energy[] chargers, Energy[] drainers, int[] chargeRates, int[] drainRates)
    {
        Conversion conversion = new Conversion(chargers, drainers, chargeRates, drainRates);
        conversions.Add(conversion);
        return conversion;
    }

}
