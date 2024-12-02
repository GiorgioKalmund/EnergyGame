using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
            
            // Space out evenly
            float inventoryWidth = inventory.GetComponent<RectTransform>().rect.width; 
            float slotWidth = inventorySlotObject.GetComponent<RectTransform>().rect.width;
            float spacing = (inventoryWidth - slotWidth * prefabs.Count) / (prefabs.Count + 1); 

            float newXPosition = (index + 1) * spacing + index * slotWidth;
            inventorySlotObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0);
        }
    }

    public void DisableSlot(int instanceId)
    {
       slots[instanceId].Disable();
       if (!activeSlots.Remove(slots[instanceId]))
       {
           Debug.LogWarning("Cannot remove slot!");
       };
       CalculateSlotSpacings();
    }
    
    public void EnableSlot(int instanceId)
    {
        if (slots[instanceId].isEnabled)
        {
            return;
        }
       slots[instanceId].Enable(); 
       activeSlots.Add(slots[instanceId]);
       CalculateSlotSpacings();
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
            float inventoryWidth = inventory.GetComponent<RectTransform>().rect.width; 
            float slotWidth = inventorySlotObject.GetComponent<RectTransform>().rect.width;
            float spacing = (inventoryWidth - slotWidth * activeSlots.Count) / (activeSlots.Count + 1); 

            float newXPosition = (index + 1) * spacing + index * slotWidth;
            inventorySlotObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0);
        } 
    }
    
    public void UpdateInventorySlots()
    {
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
    }
}
