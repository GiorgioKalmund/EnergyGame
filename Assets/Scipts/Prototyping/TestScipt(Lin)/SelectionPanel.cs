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
    [Header("Building")] 
    [SerializeField] private SelectableEntity currentEntity;
    
    public void SetPanel(SelectableEntity entity)
    {
       imageElement.sprite = entity.GetSprite();
       displayNameElement.text = entity.GetName();
       infoTextElement.text = "Some random info text might appear here.";
       if (entity as ProducerDescriptor)
       {
           ProducerDescriptor producer = (ProducerDescriptor) entity;
           statTextElement.text = $"Production {producer.GetProduction()} MW";
           costTextElement.text = $"ID: {producer.GetID()}";
       } else if (entity as ConsumerDescriptor)
       {
           ConsumerDescriptor consumer = (ConsumerDescriptor) entity;
           statTextElement.text = $"{consumer.GetConsumed()} / {consumer.GetDemand()} MW";
           costTextElement.text = $"ID: {consumer.GetID()}";
           
       }
       currentEntity = entity;
    }

    public void Close()
    {
       SelectionManager.Instance.ClearSelection(); 
    }

    public SelectableEntity GetEntity()
    {
        return currentEntity;
    }
}
