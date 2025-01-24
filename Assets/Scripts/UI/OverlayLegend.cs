using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class OverlayLegend : MonoBehaviour
{
   public List<Color> windColors;
   public List<Color> sunColors;
   public List<Color> waterColors;
   public List<Color> coalColors;

   [Header("Colors")] 
   [SerializeField] private Image intensity1;
   [SerializeField] private Image intensity2;
   [SerializeField] private Image intensity3;
   [SerializeField] private Image intensity4;

   [Header("Activator")] [SerializeField] private GameObject activator;

   [Header("Texts")] [SerializeField] private TMP_Text[] texts;
   
   
   public static OverlayLegend Instance { get; private set; }

   private void Awake()
   {
      if (Instance && Instance != this)
         Destroy(gameObject);
      else
         Instance = this;
   }

   private void Start()
   {
      if (GridDataManager.Instance)
      {
         windColors = GridDataManager.Instance.GetWindColors();
         sunColors = GridDataManager.Instance.GetSunColors();
         waterColors = GridDataManager.Instance.GetWaterColors();
         coalColors = GridDataManager.Instance.GetCoalColors();
         float[] intensities = GridDataManager.Instance.GetIntensities();
         
         for (int index = 0; index < intensities.Length; index++)
         {
            texts[intensities.Length - index - 1].text = $"{intensities[index]}";
         }
      }
      else
      {
         Debug.LogError("OverlayLegend: Could not find GridDataManager Instance!");
      }
      
      Hide();
   }

   public void Show()
   {
      activator.SetActive(true);
   }
   
   public void Hide()
   {
      activator.SetActive(false);
   }

   public void SetLegend(OverlayType type)
   {
      // If the second element is hidden, this means that we have previously hidden it by showing coal and now want to show it again
      if (!texts[1].transform.gameObject.activeSelf)
      {
         for (int index = 1; index < texts.Length; index++)
         {
            texts[index].transform.gameObject.SetActive(true);
         }
      }
      switch (type)
      {
         case OverlayType.WIND:
            intensity1.color = windColors[0];
            intensity2.color = windColors[1];
            intensity3.color = windColors[2];
            intensity4.color = windColors[3];
            break;
         case OverlayType.SUN:
            intensity1.color = sunColors[0];
            intensity2.color = sunColors[1];
            intensity3.color = sunColors[2];
            intensity4.color = sunColors[3];
            break;
         case OverlayType.WATER:
            intensity1.color = waterColors[0];
            intensity2.color = waterColors[1];
            intensity3.color = waterColors[2];
            intensity4.color = waterColors[3];
            break;
         case OverlayType.COAL:
            intensity1.color = coalColors[0];
            intensity2.color = coalColors[1];
            intensity3.color = coalColors[2];
            intensity4.color = coalColors[3];
            for (int index = 1; index < texts.Length; index++)
            {
               texts[index].transform.gameObject.SetActive(false);
            }
            break;
      }   
   }
}