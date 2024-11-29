using Scipts.Prototyping.TestScipt_Lin_;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 public class InventorySlot : MonoBehaviour
    {
        public Image spriteImage;
        public TMP_Text costText;
        public int instanceID;
        public void SetSprite(Sprite sprite)
        {
            spriteImage.sprite = sprite;
        }

        public void SetCost(float cost)
        {
            costText.text = $"{cost}€";
        }

        public void SetInstanceID(int id)
        {
            instanceID = id;
        }
        
         void Start()
            {
                Button button = GetComponent<Button>();
                if (button != null)
                {
                    // Adding the click listener if not set through the inspector
                    button.onClick.AddListener(delegate { PlacementSystem.Instance.StartPlacement(instanceID);});
                }
                else
                {
                    Debug.LogError("Button component not found!");
                }
        }
         
    } 