using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class SettingsScreenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   [Header("Visual")] 
   [SerializeField] private Sprite image;
    
   [SerializeField] private TMP_Text descriptionObject;
   public string description;

   private Button button; 

   private void Awake()
   {
      GetComponent<Image>().sprite = image;
      button = GetComponent<Button>();
      descriptionObject.text = description;
   }
   
   public void OnPointerEnter(PointerEventData eventData)
   {
       if (gameObject.transform != null) 
           gameObject.transform.DOScale(1.1f, 0.2f).SetEase(Ease.InOutElastic).SetRecyclable();
   }

   public void OnPointerExit(PointerEventData eventData)
   {
       if (gameObject.transform != null) 
           gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutElastic).SetRecyclable();
   }

   public Button GetButton()
   {
       if (!button)
       {
           button = GetComponent<Button>();
       }
       return button;
   }

   private void OnDestroy()
   {
       DOTween.Kill(gameObject.transform);
   }
}