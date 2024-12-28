using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class BuilderInventorySlot : MonoBehaviour
{
    private Button button;
    public int objectInstanceId;
    [FormerlySerializedAs("buildingImage")]
    [Header("Visual")]
    [SerializeField] private Image slotImage;
     [SerializeField] private TMP_Text slotCostText;
    [SerializeField] private TMP_Text slotCapacityText;
    private int capacity { get; set; } = 1;

    public bool active { get; private set; } 

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(delegate { PlacementManager.Instance.StartPlacement(objectInstanceId);});
        }
        active = false;
    }
    
    // Assigns image, cost text and id inside the database to the slot
    public void Setup(ProducerDescriptor entity, int objId, int startingCapacity)
    {
        slotImage.sprite = entity.GetSprite();
        slotCostText.text = $"{entity.GetCost()}€";
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

    public void AddCapacity(int value)
    {
        capacity += value;
        slotCapacityText.text = $"{capacity}";
    }
    public bool RemoveCapacity(int value)
    {
        int difference = capacity - value;
        if (difference < 0)
        {
            return false;
        }
        capacity = difference;
        slotCapacityText.text = $"{capacity}";
        return true;
    }

    public void SetCapacity(int value)
    {
        capacity = value;
        slotCapacityText.text = $"{capacity}";
    }
} 
