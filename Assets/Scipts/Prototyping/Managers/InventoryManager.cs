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
    public float innerSpacing = 75f; 
    public float margin = 10f; 
    public float scalingFactor = 0.01f;
    private bool inventoryHidden;

    
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
        inventoryHidden = false;
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

            // Debug.Log("Instantiated " + inventorySlotObject.name);
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

    public void HideInventory()
    {
        //Debug.LogWarning("Hiding Inventory");
        inventoryHidden = true;
        Vector3 currentPos = inventory.transform.position;
        currentPos.y -= 220f;
        inventory.transform.DOMoveY(currentPos.y, 0.75f);
    }

    public void ShowInventory()
    {
        //Debug.LogWarning("Showing Inventory");
        inventoryHidden = false;
        Vector3 currentPos = inventory.transform.position;
        currentPos.y += 220f;
        inventory.transform.DOMoveY(currentPos.y, 0.75f);
    }

    private void CalculateSlotSpacings()
    {
        Debug.Log("Inventory has items: "+activeSlots.Count);

        if (IsEmpty())
        {
            HideInventory();
        }
        
        activeSlots.Sort((a,b) => a.instanceID.CompareTo(b.instanceID));
        for (int index = 0;  index < activeSlots.Count;  index++)
        {
            GameObject inventorySlotObject = activeSlots[index].gameObject;
            float slotWidth = inventorySlotObject.GetComponent<RectTransform>().rect.width;

            int tempIndex = index;
            tempIndex -= activeSlots.Count / 2;
            
            float newXPosition = (slotWidth + innerSpacing) * tempIndex; 
            if (tempIndex < 0)
            {
               newXPosition *= 1; 
            }
            if (activeSlots.Count % 2 == 0)
            {
                newXPosition += (slotWidth + innerSpacing) / 2;
            }

            newXPosition *= scalingFactor; 
            inventorySlotObject.GetComponent<RectTransform>().DOAnchorPos(new Vector3(newXPosition, 0, 0), 0.5f);
        }
        float targetWidth = (slotTemplate.GetComponent<RectTransform>().rect.width + innerSpacing) * activeSlots.Count;
        targetWidth -= innerSpacing;
        targetWidth += margin * 2;
        float targetHeight = (slotTemplate.GetComponent<RectTransform>().rect.height);
        RectTransform newRect = inventory.GetComponent<RectTransform>();
        newRect.DOSizeDelta(new Vector2(targetWidth * scalingFactor, targetHeight * scalingFactor), 0.5f);

    }
     
    // Called in ProducerDescriptor when we sell and add money to our budget
    // Called in PlacementManager when we place and remove money from our budget
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
        CalculateSlotSpacings();
    }

    public bool IsEmpty()
    {
        return activeSlots.Count == 0;
    }

    public bool InventoryHidden()
    {
        return inventoryHidden;
    }
}
