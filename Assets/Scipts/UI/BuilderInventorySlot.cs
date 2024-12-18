using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BuilderInventorySlot : MonoBehaviour
{
    private Button button;
    public int objectInstanceId;
    [SerializeField] private Image buildingImage;
    [SerializeField] private TMP_Text buildingCostText;

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
        buildingImage.sprite = entity.GetSprite();
        buildingCostText.text = $"{entity.GetCost()}€";
        objectInstanceId = objId;
    }
} 
