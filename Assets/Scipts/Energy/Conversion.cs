using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversion
{
    private Energy[] chargers;
    private Energy[] drainers;
    private int[] chargeRates;
    private int[] drainRates;

    private int energy;

    public Conversion(Energy[] chargers, Energy[] drainers, int[] chargeRates, int[] drainRates)
    {
        this.chargers = chargers;
        this.drainers = drainers;
        this.chargeRates = chargeRates;
        this.drainRates = drainRates;
    }

    public void convert()
    {
        int[] availableChargeRates = new int[chargeRates.Length];
        int[] availableDrainRates = new int[drainRates.Length];
        int availableChargeRate = 0;
        int availableDrainRate = 0;

        for (int i = 0; i < chargers.Length; i++)
        {
            availableChargeRates[i] = (chargers[i].getAmount() - chargers[i].getMin()) < chargeRates[i] ?
                (chargers[i].getAmount() - chargers[i].getMin()) : chargeRates[i];
            availableChargeRate += availableChargeRates[i];
        }

        for (int i = 0; i < drainers.Length; i++)
        {
            availableDrainRates[i] = (drainers[i].getMax() - drainers[i].getAmount()) < drainRates[i] ? 
                (drainers[i].getMax() - drainers[i].getAmount()) : drainRates[i];
            availableDrainRate += availableDrainRates[i];
        }

        int conversionRate = (availableChargeRate < availableDrainRate) ? availableChargeRate : availableDrainRate;
        int chargeRate = 0;
        int drainRate = 0;

        for (int i = 0; i < availableChargeRates.Length; i++)
        {
            int cR = (chargeRate + availableChargeRates[i]) < conversionRate ? 
                availableChargeRates[i] : (conversionRate - chargeRate);
            chargers[i].drain(cR);
            chargeRate += cR;
        }

        for (int i = 0; i < availableDrainRates.Length; i++)
        {
            int dR = (drainRate + availableDrainRates[i]) < conversionRate ?
                availableDrainRates[i] : (conversionRate - drainRate);
            drainers[i].charge(dR);
            drainRate += dR;
        }

    }
}
