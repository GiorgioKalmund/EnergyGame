using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private ObjectsDatabase database;
    public List<GameObject> items;
    public List<InventorySlot> slots;
    [SerializeField] private GameObject slotTemplate;

    private int slotsFilled;
    
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
        slotsFilled = 0;
    }

    private void Start()
    {
        for (int index = 0; index < items.Count; index++)
        {
            // Fill the database
            ProducerDescriptor producerDescriptor = items[index].GetComponent<ProducerDescriptor>();
            database.Put(new ObjectData(producerDescriptor, index, items[index]));
    
            // Fill slot from new entry 
            GameObject inventorySlotObject = Instantiate(slotTemplate, inventory.transform.position, Quaternion.identity);
            inventorySlotObject.transform.SetParent(inventory.transform);

            Debug.Log("Instantiated " + inventorySlotObject.name);
            InventorySlot newSlot = inventorySlotObject.GetComponent<InventorySlot>();
    
            newSlot.SetSprite(producerDescriptor.GetSprite());
            newSlot.SetCost(producerDescriptor.GetCost());
            newSlot.SetInstanceID(index);
    
            slots.Add(newSlot);
    
            // Space out evenly
            float inventoryWidth = inventory.GetComponent<RectTransform>().rect.width; 
            float slotWidth = inventorySlotObject.GetComponent<RectTransform>().rect.width;
            float spacing = (inventoryWidth - slotWidth * items.Count) / (items.Count + 1); 

            float newXPosition = (index + 1) * spacing + index * slotWidth;
            inventorySlotObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPosition, 0); 
        }
    }
}
