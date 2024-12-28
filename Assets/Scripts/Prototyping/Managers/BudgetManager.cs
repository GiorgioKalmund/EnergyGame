using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    public float budget;
    public float sellingFactor;

    public static BudgetManager Instance { get; private set; }

    private void Awake()
    {
        
       if (Instance && Instance != this)
       {
           Destroy(this);
       }
       else
       {
           Instance = this;
       }
    }

    private void Start()
    {
       UIManager.Instance.RenderBudget();
    }

    public void Sell(float originalAmount)
    {
        Debug.Log("Sold for "+ originalAmount * sellingFactor);
        AddBudget(originalAmount * sellingFactor);
    }
    

    public void AddBudget(float amount)
    {
        budget += amount;
        UIManager.Instance.RenderBudget();
    }

    public bool UseBudget(float amount)
    {
        float moneyLeft = budget - amount;
        if (moneyLeft < 0)
        {
            return false;
        }
        budget = moneyLeft;
        UIManager.Instance.RenderBudget();
        return true;
    }

    public bool CanHandleCost(float amount)
    {
        return budget - amount >= 0;
    }

    public float GetSellingFactor()
    {
        return sellingFactor;
    }
    
    public float GetSellingValueOf(float originalAmount)
    {
        return originalAmount * sellingFactor;
    }
    
    public void SetBudget(float amount)
    {
        budget = amount;
        UIManager.Instance.RenderBudget();
    }
    
    public float GetBudget()
    {
        return budget;
    }

}
