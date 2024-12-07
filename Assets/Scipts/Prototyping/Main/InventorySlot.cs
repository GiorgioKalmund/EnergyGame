using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
                    button.onClick.AddListener(delegate { PlacementManager.Instance.StartPlacement(instanceID);});
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

        public void OnPointerEnter(PointerEventData data)
        {
            Debug.Log("The pointer is over: "+name);
            gameObject.transform.DOScale(11f/10f, 0.2f).SetEase(Ease.InOutElastic);
        }
        
        public void OnPointerExit(PointerEventData data)
        {
            Debug.Log("The pointer exited: "+name);
            gameObject.transform.DOScale(10f/11f, 0.2f).SetEase(Ease.InOutElastic);
        }
    } 