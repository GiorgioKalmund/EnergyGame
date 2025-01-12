using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class LevelManager : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private float currentDemand;
    public float currentProduction;
    public float currentEnvironmentalImpact;
    [SerializeField] private float maxEnvironmentalImpact;
    [SerializeField] private bool demandMet = false;
    public int endpointsCompleted= 0;
    public int endpointsCount = 0;
    public static LevelManager Instance { get; private set; }

    //Endpoints von oben links nach unten rechts auf der Map durchnummeriert
    public int[] endpointDemands;
     
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

    }

    private void Start()
    {
        UIManager.Instance.UpdateCurrentEnvironmentalImpact();
    }

    public void AddProduce(float value)
    {
        currentProduction += value;
        CheckIfDemandIsMet();
    }

    public void ReduceProduce(float value)
    {
        currentProduction = Mathf.Max(0, currentProduction - value);
        CheckIfDemandIsMet();
    }

    public void AddDemand(float value)
    {
        currentDemand += value;
    }

    public void DecreaseDemand(float value)
    {
        currentDemand = Mathf.Max(0f, currentDemand - value);
        CheckIfDemandIsMet();
    }

    public void AddEnvironmentalImpact(float impact)
    {
        currentEnvironmentalImpact += impact;
        if (currentEnvironmentalImpact > maxEnvironmentalImpact)
            CharacterSpeechManager.Instance.Co2BubbleAction(SpeechBubbleAction.OPEN);
        UIManager.Instance.UpdateCurrentEnvironmentalImpact();
    }

    public void ReduceEnvironmentalImpact(float impact)
    {
        currentEnvironmentalImpact -= impact;
        if (currentEnvironmentalImpact <= maxEnvironmentalImpact)
            CharacterSpeechManager.Instance.Co2BubbleAction(SpeechBubbleAction.CLOSE);
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

    public void SetMaxEnvironmentalImpact(float increase)
    {
        maxEnvironmentalImpact += increase;
    }

    /// <summary>
    /// Calls UI as well
    /// </summary>
    /// <returns>Number of completed endpoints</returns>
    public int CompleteEndpoint()
    {
        endpointsCompleted++;
        UIManager.Instance.SetEndpointsCompleted(endpointsCompleted);
        if (endpointsCompleted == endpointsCount)
        {
            // TODO: Trigger Win action
            Debug.LogWarning("===GAME WON===");
        }
        
        return endpointsCompleted;
    }
    /// <summary>
    /// Calls UI as well
    /// </summary>
    /// <returns>Number of completed endpoints</returns>
    public int UncompleteEndpoint(){
        endpointsCompleted--;
        UIManager.Instance.SetEndpointsCompleted(endpointsCompleted);
        return endpointsCompleted;
    }


}

  
