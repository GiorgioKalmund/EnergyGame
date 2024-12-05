using Scipts.Prototyping.TestScipt_Lin_;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class InventorySlot : MonoBehaviour
    {
        public Image spriteImage;
        public TMP_Text costText;
        public int instanceID;
        public bool isEnabled;
        public Button button;
        
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
                button = GetComponent<Button>();
                if (button != null)
                {
                    // Adding the click listener if not set through the inspector
                    button.onClick.AddListener(delegate { PlacementSystem.Instance.StartPlacement(instanceID);});
                }
                else
                {
                    Debug.LogError("Button component not found!");
                }
                isEnabled = true;
        }

        public void Disable()
        {
            isEnabled = false;
            spriteImage.enabled = isEnabled;
            //button.image.enabled = isEnabled;
            button.enabled = isEnabled;
            costText.enabled = isEnabled;
        }
        
        public void Enable()
        {
            isEnabled = true;
            spriteImage.enabled = isEnabled;
            // button.image.enabled = isEnabled;
            button.enabled = isEnabled;
            costText.enabled = isEnabled;
        }
    } 