using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class LevelManager : MonoBehaviour
{
    [Header("Stats")] 
    public float currentProduction;
    public float currentEnvironmentalImpact;
    [SerializeField] private float maxEnvironmentalImpact;
    public int endpointsCompleted= 0;
    public int endpointsCount = 0;
    public static LevelManager Instance { get; private set; }

    //Endpoints von oben links nach unten rechts auf der Map durchnummeriert
    public int[] endpointDemands;

    public int[] powerPlantInventoryCapacities = new int[6];
      
    public int nextID = 0;

    private bool isWon = false;

    private int storedLevelCompletedCount = 0;
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

    public float GetMaxEnvironmentalImpact()
    {
        return maxEnvironmentalImpact;
    }

    public float GetCurrentEnvironmentalImpact()
    {
        return currentEnvironmentalImpact;
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
            
            CheckLevelWon();
            
            
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
    
    public void LoadNextLevel(){
        if(!isWon) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    /// <summary>
    /// Checks and sets isWon if all conditions are met
    /// </summary>
    private void CheckLevelWon(){
        bool hasEnoughBudget = BudgetManager.Instance.budget>=0;
        bool notExcededPollution = currentEnvironmentalImpact <= maxEnvironmentalImpact;
        isWon = hasEnoughBudget && notExcededPollution;
        if(isWon) UnlockNextLevel();
    }
    public void UnlockNextLevel(){
        Debug.Log("===GAME WON===");
        UIManager.Instance.UnlockNextLevelButton();

        storedLevelCompletedCount = PlayerPrefs.GetInt("levels_completed");
        PlayerPrefs.SetInt("levels_completed", ++storedLevelCompletedCount);
    }
}

  
