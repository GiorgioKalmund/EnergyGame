using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private Image imageElement;
    [SerializeField] private TMP_Text displayNameElement;
    [SerializeField] private TMP_Text infoTextElement;
    [SerializeField] private TMP_Text statTextElement;
    [SerializeField] private TMP_Text costTextElement;
    [SerializeField] private GameObject sellButton;
    [Header("Building")] 
    public ISelectableEntity currentEntity;
    
    public void SetPanel(ISelectableEntity entity)
    {
        ShowSellButton(false);
       imageElement.sprite = entity.GetSprite();
       displayNameElement.text = entity.GetName();
       infoTextElement.text = "Some random info text might appear here.";
       currentEntity = entity;
       if (currentEntity as ProducerDescriptor)
       {
           ProducerDescriptor currentProducer = (ProducerDescriptor) entity;
           statTextElement.text = $"Max: {currentProducer.GetMaxProduction()} MW\n" +
                                  $"Current: {currentProducer.GetCurrentProduction()} MW\n" +
                                  $"Env : {currentProducer.GetEnvironmentalImpact()} CO2";
           costTextElement.text = $"ID: {currentProducer.GetID()}";
           ShowSellButton(true);
       } else if (currentEntity as ConsumerDescriptor)
       {
           ConsumerDescriptor consumer = (ConsumerDescriptor) entity;
           statTextElement.text = $"{consumer.GetConsumed()} / {consumer.GetDemand()} MW";
           costTextElement.text = $"ID: {consumer.GetID()}";
       }
    }

    public void Close()
    {
       SelectionManager.Instance.ClearSelection(); 
       currentEntity = null;
    }

    public bool IsOpen()
    {
        return currentEntity != null;
    }

    public void ShowSellButton(bool boolean)
    {
        sellButton.SetActive(boolean);
        if (currentEntity as ProducerDescriptor)
        {
            ProducerDescriptor currentProducer = (ProducerDescriptor) currentEntity;
            TMP_Text text = sellButton.GetComponentInChildren<TMP_Text>();
            text.text = $"Sell {BudgetManager.Instance.GetSellingValueOf(currentProducer.GetCost())}";
        }
    }

    public void Sell()
    {
        if (currentEntity as ProducerDescriptor)
        {
            ProducerDescriptor currentProducer = (ProducerDescriptor)currentEntity;
            currentProducer.Sell();
        }
        Close();
    }

    public ISelectableEntity GetEntity()
    {
        return currentEntity;
    }
}
