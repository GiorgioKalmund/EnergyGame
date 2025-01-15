using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class SettingsScreenButton : MonoBehaviour
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
       DOTween.KillAll();
   }
}