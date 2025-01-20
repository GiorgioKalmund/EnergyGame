using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class BuilderInventory : MonoBehaviour
{
    private Vector3 collapsedPosition;
    private Vector3 expandedPosition;
    private bool expanded;
    
    public static BuilderInventory Instance { get; private set; }

    [Header("Speech Bubble")] 
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TMP_Text speechBubbleName;
    [SerializeField] private TMP_Text speechBubbleOutput;
    [SerializeField] private TMP_Text speechBubbleCO2;
    [SerializeField] private TMP_Text speechBubbleCost;
    public bool speechBubbleOpen;
    [SerializeField] private float speechBubbleScaleDuration = 0.5f;
    
    [Header("Data")] 
    [SerializeField] private ObjectsDatabase database;
    [SerializeField] private List<GameObject> prefabs;

    [Header("Construction")] 
    public bool isConstructionInventory;
    [SerializeField] private int constructionSlotToUnlock;

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
        // Initially hide the bubble with no animation
        speechBubble.transform.localScale = Vector3.zero;

        if (LevelManager.Instance.powerPlantInventoryCapacities.Length != prefabs.Count)
            throw new Exception("BuilderInventory: Capacity array length != Slots length");
         
        for (int index = 0; index < prefabs.Count; index++)
        {
            // Create a new slot for the element from the list
            GameObject slotGameObject = Instantiate(builderInventorySlotGameObject, inventorySlots.transform);
            
            // Assign the proper values so the slot knows which image, cost and prefab to use
            BuilderInventorySlot slot = slotGameObject.GetComponent<BuilderInventorySlot>();
            ProducerDescriptor descriptor = prefabs[index].GetComponent<ProducerDescriptor>();
            descriptor.instanceId = index;
            
            slot.Setup(descriptor, index, LevelManager.Instance.powerPlantInventoryCapacities[index], isConstructionInventory && index != constructionSlotToUnlock);
                
            
            // Update internal List
            builderSlots.Add(slot);
            
            // Update database
            database.Put(new ObjectData(descriptor, index, prefabs[index]));
        }
        
    }

    public void AddSlotCapacity(int capacity, int slotId)
    {
        if (builderSlots[slotId].AddCapacity(capacity))
        {
            UIManager.Instance.HideDON();
        };
    }

    public void RemoveSlotCapacity(int capacity, int slotId)
    {
        if (!builderSlots[slotId].RemoveCapacity(capacity))
        {
            // TODO: Trigger action if we go negative
            UIManager.Instance.ShowDON();
        }
    }
    
   public void ShowInventory()
   {
        expanded = true;
        inventory.GetComponent<RectTransform>().DOLocalMoveY(expandedPosition.y, inventoryAnimTime);
   }
    
    public void HideInventory()
    {
        expanded = false;
        inventory.GetComponent<RectTransform>().DOLocalMoveY(collapsedPosition.y, inventoryAnimTime);
    }

    public void ToggleInventory()
    {
        if (expanded)
        {
           HideInventory(); 
        }
        else
        {
            ShowInventory();
        }
    }

    public void OpenSpeechBubble(ProducerDescriptor descriptor)
    {
        if (speechBubbleOpen)
            return;

        speechBubble.transform.DOScale(1f, speechBubbleScaleDuration).SetEase(Ease.InOutQuad).SetRecyclable();

        speechBubbleName.text = $"{descriptor.GetName()}";
        speechBubbleOutput.text = $"{descriptor.GetMaxProduction()} MW";
        speechBubbleCO2.text = $"{descriptor.GetEnvironmentalImpact()} t CO<sub>2</sub>";
        speechBubbleCost.text = $"{descriptor.GetCost()} Mio €";
    }

    public void CloseSpeechBubble()
    {
        if (!speechBubble)
            return;
        
        speechBubble.transform.DOScale(0f, speechBubbleScaleDuration).SetEase(Ease.InOutQuad).SetRecyclable();
    }


    public bool Expanded()
    {
        return expanded;
    }

    public int getConstructionSlotToUnlock()
    {
        return constructionSlotToUnlock;
    }

}
