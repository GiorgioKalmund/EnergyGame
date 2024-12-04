using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private ObjectsDatabase database;
    public List<GameObject> prefabs;
    public List<InventorySlot> slots;
    public List<InventorySlot> activeSlots;
    [SerializeField] private GameObject slotTemplate;
    public float spacing = 75f; 

    
    public static InventoryManager Instance { get; private set; }
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
        database.Clear();
        slots = new List<InventorySlot>();
        activeSlots = new List<InventorySlot>();
    }

    private void Start()
    {
        for (int index = 0; index < prefabs.Count; index++)
        {
            // Fill the database
            ProducerDescriptor producerDescriptor = prefabs[index].GetComponent<ProducerDescriptor>();
            producerDescriptor.SetDBInstanceID(index);
            database.Put(new ObjectData(producerDescriptor, index, prefabs[index]));
    
            // Fill slot from new entry 
            GameObject inventorySlotObject = Instantiate(slotTemplate, inventory.transform.position, Quaternion.identity);
            inventorySlotObject.transform.SetParent(inventory.transform);

            Debug.Log("Instantiated " + inventorySlotObject.name);
            InventorySlot newSlot = inventorySlotObject.GetComponent<InventorySlot>();
    
            newSlot.SetSprite(producerDescriptor.GetSprite());
            newSlot.SetCost(producerDescriptor.GetCost());
            newSlot.SetInstanceID(index);
    
            slots.Add(newSlot);
            activeSlots.Add(newSlot);
            
        }
        CalculateSlotSpacings();
    }
    
    public void DisableSlot(int instanceId)
    {
       slots[instanceId].Disable();
       if (!activeSlots.Remove(slots[instanceId]))
       {
           Debug.LogWarning("Cannot remove slot!");
       };
    }
    
    public void EnableSlot(int instanceId)
    { 
        if (!slots[instanceId].isEnabled)
        {
           slots[instanceId].Enable(); 
           activeSlots.Add(slots[instanceId]);
        }
    }

    public void ToggleSlot(int instanceId)
    {
        if (slots[instanceId].isEnabled)
        {
            DisableSlot(instanceId);
        }
        else
        {
            EnableSlot(instanceId);
        }
    }

    private void CalculateSlotSpacings()
    {
        activeSlots.Sort((a,b) => a.instanceID.CompareTo(b.instanceID));
        for (int index = 0;  index < activeSlots.Count;  index++)
        {
            GameObject inventorySlotObject = activeSlots[index].gameObject;
            float slotWidth = inventorySlotObject.GetComponent<RectTransform>().rect.width;

            int tempIndex = index;
            tempIndex -= activeSlots.Count / 2;
            
            float newXPosition = (slotWidth + spacing) * tempIndex; 
            if (tempIndex < 0)
            {
               newXPosition *= 1; 
            }
            if (activeSlots.Count % 2 == 0)
            {
                newXPosition += (slotWidth + spacing) / 2;
            }
            
            inventorySlotObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(newXPosition, 0, 0), 0.5f);
        }
        float targetWidth = (slotTemplate.GetComponent<RectTransform>().rect.width + spacing) * activeSlots.Count;
        targetWidth -= spacing / 2; // clean up edges
        Debug.Log("Target inventory width: "+targetWidth);
        RectTransform newRect = inventory.GetComponent<RectTransform>();
        inventory.GetComponent<RectTransform>().DOSizeDelta(new Vector2(targetWidth, newRect.rect.height), 0.5f);

    }
    
    public void UpdateInventorySlots()
    {
        Debug.LogWarning("Updated Inventory Slots");
        for (int i = 0; i < database.Count(); i++)
        {
            ProducerDescriptor producerDescriptor = (ProducerDescriptor) database.Get(i).Entity;
            if (producerDescriptor.GetCost() > BudgetManager.Instance.GetBudget())
            {
               DisableSlot(i); 
            }
            else
            {
                EnableSlot(i); 
            }
        }
        CalculateSlotSpacings();
    }
}
