using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Energy
{
    private readonly Vector3 position;
    private int amount;
    private readonly int max;
    private readonly int min;

    public Energy(Vector3 position, int amount, int min, int max)
    {
        if (min > max || amount < min || amount > max) {
            return;
        }

        this.position = position;
        this.amount = amount;
        this.min = min;
        this.max = max;
    }

    public void charge(int amount)
    {
        this.amount += amount;
    }

    public void drain(int amount)
    {
        this.amount -= amount;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public int getAmount() 
    {
        return amount;
    }

    public int getMax()
    {
        return max;
    }

    public int getMin()
    {
        return min;
    }

    // DBG
    public string dbg()
    {
        return this.GetType().Name + " @ " + getPosition().ToString() + ":\t" + getAmount() + "\t[" + getMin() + ", " + getMax() + "]";
    }

}
