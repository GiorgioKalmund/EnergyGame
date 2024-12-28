using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
    public bool inventoryHidden;

    public float hidingYOffset = 220f;
    public Vector3 startPosition;
    public Vector3 hiddenPosition;
    
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
        startPosition = gameObject.transform.position;
        hiddenPosition = startPosition;
        hiddenPosition.y -= hidingYOffset;
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
       if (activeSlots.RemoveAll((slot) => slot.instanceID == instanceId) == 0)
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
        if (inventoryHidden)
        {
            return;
        }
        //Debug.LogWarning("Hiding Inventory");
        inventoryHidden = true;
        inventory.transform.DOMoveY(hiddenPosition.y, 0.75f);
    }

    public void ShowInventory()
    {
        if (!inventoryHidden)
        {
            return;
        }
        // Debug.LogWarning("Showing Inventory");
        inventoryHidden = false;
        Vector3 currentPos = inventory.transform.position;
        currentPos.y += 220f;
        inventory.transform.DOMoveY(startPosition.y, 0.75f);
    }

    private void CalculateSlotSpacings()
    {
        if (IsEmpty() && !inventoryHidden)
        {
            HideInventory();
        } else if (!IsEmpty() && inventoryHidden)
        {
            ShowInventory();
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
