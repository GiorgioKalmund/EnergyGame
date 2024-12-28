using System;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class BuilderInventory : MonoBehaviour
{
    private Vector3 collapsedPosition;
    private Vector3 expandedPosition;
    private bool expanded;
    
    public static BuilderInventory Instance { get; private set; }
    
    [Header("Data")] 
    [SerializeField] private ObjectsDatabase database;
    [SerializeField] private List<GameObject> prefabs;

    [Header("References")]
    [SerializeField] private GameObject inventory;
    [SerializeField] private Button inventoryToggle;
    [SerializeField] private GameObject inventorySlots;
    [SerializeField] private GameObject builderInventorySlotGameObject;

    private List<BuilderInventorySlot> builderSlots;
    
    [Header("Values")] 
    [SerializeField] private float inventoryAnimTime = 0.5f;
    [SerializeField] private float inventoryYMovement = 150f;
 
    private void Awake()
    {
        // Setup initial positions and state 
        expanded = false;
        collapsedPosition = inventory.GetComponent<RectTransform>().anchoredPosition; 
        expandedPosition = collapsedPosition + new Vector3(0f, inventoryYMovement, 0f);
        database.Clear();

        builderSlots = new List<BuilderInventorySlot>();

        if (!inventorySlots || !inventoryToggle || !builderInventorySlotGameObject || !inventory)
        {
            Debug.LogWarning("The Builder Inventory is missing something!");
            throw new Exception();
        }
        
        // Assign the toggle action on runtime
        inventoryToggle.onClick.AddListener(ToggleInventory);

        if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        for (int index = 0; index < prefabs.Count; index++)
        {
            // Create a new slot for the element from the list
            GameObject slotGameObject = Instantiate(builderInventorySlotGameObject, inventorySlots.transform);
            
            // Assign the proper values so the slot knows which image, cost and prefab to use
            BuilderInventorySlot slot = slotGameObject.GetComponent<BuilderInventorySlot>();
            ProducerDescriptor descriptor = prefabs[index].GetComponent<ProducerDescriptor>();
            
            // TODO: Determine Starting Capacity dynamically
            slot.Setup(descriptor, index, 3);
            
            // Update internal List
            builderSlots.Add(slot);
            
            // Update database
            database.Put(new ObjectData(descriptor, index, prefabs[index]));
        }
    }

    public void AddSlotCapacity(int capacity, int slotId)
    {
        builderSlots[slotId].AddCapacity(capacity);
    }

    public void RemoveSlotCapacity(int capacity, int slotId)
    {
        if (!builderSlots[slotId].RemoveCapacity(capacity))
        {
            // TODO: Trigger action if we go negative
        }
    }
    
    public void SetSlotCapacity(int capacity, int slotId)
    {
        builderSlots[slotId].SetCapacity(capacity);
    }
   public void Expand()
   {
        expanded = true;
        inventory.GetComponent<RectTransform>().DOLocalMoveY(expandedPosition.y, inventoryAnimTime);
   }
    
    public void Collapse()
    {
        expanded = false;
        inventory.GetComponent<RectTransform>().DOLocalMoveY(collapsedPosition.y, inventoryAnimTime);
    }

    private void ToggleInventory()
    {
        if (expanded)
        {
           Collapse(); 
        }
        else
        {
            Expand();
        }
    }
}
