using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class BuilderInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    public int objectInstanceId;
    [Header("Visual")]
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text slotCapacityText;
    [SerializeField] private Color positiveSlotColor;
    [SerializeField] private Color neutralSlotColor;
    [SerializeField] private Color negativeSlotColor;
    
    private int capacity { get; set; } = 1;
    private ProducerDescriptor entity;

    public bool active { get; private set; } 

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(StartPlacement);
        }
        active = false;
    }
    
    // Assigns image, cost text and id inside the database to the slot
    public void Setup(ProducerDescriptor descriptor, int objId, int startingCapacity)
    {
        entity = descriptor;
        slotImage.sprite = entity.GetSprite();
        objectInstanceId = objId;
        SetCapacity(startingCapacity);
    }

    public void Activate()
    {
        // Show that the slot is available 
        active = true;
        button.interactable = true;
        slotImage.color = Color.white;
    }
    
    public void Deactivate()
    {
        // Show that the slot is available 
        active = false;
        button.interactable = false;
        slotImage.color = Color.gray;
    }

    public bool AddCapacity(int value)
    {
        SetCapacity(capacity + value);
        return capacity >= 0;
    }
    public bool RemoveCapacity(int value)
    {
        SetCapacity(capacity - value);
        
        if (capacity < 0)
        {
            return false;
        }

        return true;
    }

    public void SetCapacity(int value)
    {
        capacity = value;
        slotCapacityText.text = $"{capacity}";
        slotCapacityText.color = GetSlotTextColor();
    }

    private Color GetSlotTextColor()
    {
        if (capacity > 0)
            return positiveSlotColor;
        if (capacity == 0)
            return neutralSlotColor;
        return negativeSlotColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BuilderInventory.Instance)
            BuilderInventory.Instance.OpenSpeechBubble(entity);
        else
            Debug.LogError("No Builder Inventory found!");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (BuilderInventory.Instance)
            BuilderInventory.Instance.CloseSpeechBubble();
        else
            Debug.LogError("No Builder Inventory found!");
    }

    private void StartPlacement()
    {
        if (UIManager.Instance)
            UIManager.Instance.ResetMode();



        if (PlacementManager.Instance)
        {
            if (PlacementManager.Instance.Placing())
                PlacementManager.Instance.Abort();
            PlacementManager.Instance.StartPlacement(objectInstanceId);
        }

    }
} 
