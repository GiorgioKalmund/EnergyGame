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
    public int capacity { get; set; } = 1;

    public bool active = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(delegate { PlacementManager.Instance.StartPlacement(objectInstanceId);});
        }
    }
    
    // Assigns image, cost text and id inside the database to the slot
    public void Setup(ProducerDescriptor entity, int objId)
    {
        slotImage.sprite = entity.GetSprite();
        slotCostText.text = $"{entity.GetCost()}€";
        objectInstanceId = objId;
    }

    public void Activate()
    {
        // Show that the slot is available 
        button.interactable = true;
        slotImage.color = Color.white;
    }
    
    public void Deactivate()
    {
        // Show that the slot is available 
        button.interactable = false;
        slotImage.color = Color.gray;
    }
} 
