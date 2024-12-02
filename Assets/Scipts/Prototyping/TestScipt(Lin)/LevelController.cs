using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class LevelController : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private float currentDemand;
    public float currentProduction;
    public float currentEnvironmentalImpact;
    [SerializeField] private float maxEnvironmentalImpact;
    [SerializeField] private bool demandMet = false;
    public static LevelController Instance { get; private set; }

    public int nextID = 0;

    private void Awake()
    {
        // Singleton
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        UIManager.Instance.UpdateCurrentProductionText();
        UIManager.Instance.UpdateCurrentEnvironmentalImpact();
    }

    public void AddProduce(float value)
    {
        Debug.Log("Added produce of " + value);
        currentProduction += value;
        CheckIfDemandIsMet();
        UIManager.Instance.UpdateCurrentProductionText();
    }

    public void ReduceProduce(float value)
    {
        currentProduction = Mathf.Max(0, currentProduction - value);
        CheckIfDemandIsMet();
        UIManager.Instance.UpdateCurrentProductionText();
    }

    public void AddDemand(float value)
    {
        currentDemand += value;
        UIManager.Instance.UpdateCurrentProductionText();
    }

    public void DecreaseDemand(float value)
    {
        currentDemand = Mathf.Max(0f, currentDemand - value);
        CheckIfDemandIsMet();
        UIManager.Instance.UpdateCurrentProductionText();
    }

    public void AddEnvironmentalImpact(float impact)
    {
        currentEnvironmentalImpact += impact;
        UIManager.Instance.UpdateCurrentEnvironmentalImpact();
    }

    public void ReduceEnvironmentalImpact(float impact)
    {
        currentEnvironmentalImpact -= impact;
        UIManager.Instance.UpdateCurrentEnvironmentalImpact();
    }

    public float GetCurrentDemand()
    {
        return currentDemand;
    }

    public float GetCurrentProduction()
    {
        return currentProduction;
    }
    public bool GetDemandMet()
    {
        return demandMet;
    }

    public float GetMaxEnvironmentalImpact()
    {
        return maxEnvironmentalImpact;
    }

    public float GetCurrentEnvironmentalImpact()
    {
        return currentEnvironmentalImpact;
    }

    private void CheckIfDemandIsMet()
    {
        demandMet = currentProduction >= currentDemand;
    }
    
}

  
