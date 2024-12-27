using System;
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

   public Button button { get; set; }

   private void Awake()
   {
      GetComponent<Image>().sprite = image;
      button = GetComponent<Button>();
      descriptionObject.text = description;
   }
   
   public void OnPointerEnter(PointerEventData eventData)
   {
        gameObject.transform.DOScale(1.1f, 0.2f).SetEase(Ease.InOutElastic);
   }

   public void OnPointerExit(PointerEventData eventData)
   {
        gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutElastic);
   }
}