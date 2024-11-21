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
   [SerializeField] private float currentDemand;
   [SerializeField] private float currentProduction;
   [SerializeField] private bool demandMet = false;
   [SerializeField] 
   private TMP_Text demandText, budgetText;
   public Dictionary<float, float> SubGoals;
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
       UpdateDemandText();
   }

   public void AddProduce(float value, float distance)
   {
       currentProduction += value - distance;
       CheckIfDemandIsMet();
       UpdateDemandText();
   }
   
   public void ReduceProduce(float value)
   {
       currentProduction = Mathf.Max(0, currentProduction - value);
       CheckIfDemandIsMet();
       UpdateDemandText();
   }
   
   public void IncreaseDemand(float value)
   {
       currentDemand += value;
       UpdateDemandText();
   }
   
   public void DecreaseDemand(float value)
   {
       currentDemand = Mathf.Max(0f, currentDemand - value);
       CheckIfDemandIsMet();
       UpdateDemandText();
   }

   public bool GetDemandMet()
   {
       return demandMet;
   }

   private void CheckIfDemandIsMet()
   {
       demandMet = currentProduction >= currentDemand;
   }

   private void UpdateDemandText()
   {
       demandText.text = $"{currentProduction}/{currentDemand} MW";
   }
}
