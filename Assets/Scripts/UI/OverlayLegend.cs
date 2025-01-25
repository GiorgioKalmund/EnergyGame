using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class OverlayLegend : MonoBehaviour
{
   [Header("WIND")]
   [SerializeField] private string windTitle = "Windstärke";
   [SerializeField] private string windUnit = "m/s";
   private float[] windIntensities ;
   private List<Color> windColors;
   [Header("SUN")]
   [SerializeField] private string sunTitle = "Sonnenstunden";
   [SerializeField] private string sunUnit = "hr";
   private float[] sunIntensities;
   private List<Color> sunColors;
   [Header("WATER")]
   [SerializeField] private string waterTitle = "Wasserstrom";
   [SerializeField] private string waterUnit = "m/s";
   private float[] waterIntensities;
   private List<Color> waterColors;
   [Header("COAL")]
   [SerializeField] private string coalTitle = "";
   [SerializeField] private string coalUnit = "Kohlevorkommen";
   private float[] coalIntensities;
   private List<Color> coalColors;

   private float[] intensities;

   [Header("Colors")] 
   [SerializeField] private Image intensity1;
   [SerializeField] private Image intensity2;
   [SerializeField] private Image intensity3;
   [SerializeField] private Image intensity4;

   [Header("Activator")] [SerializeField] private GameObject activator;

   [Header("Texts")] 
   [SerializeField] private TMP_Text title;
   [SerializeField] private TMP_Text[] texts;
   
   
   public static OverlayLegend Instance { get; private set; }

   private void Awake()
   {
      if (Instance && Instance != this)
         Destroy(gameObject);
      else
         Instance = this;
   }

   private void OnEnable()
   {
      windIntensities = new float[] { 4, 8, 12, 16};
      sunIntensities = new float[] { 2, 4, 5, 8 };
      waterIntensities= new float[] { 1, 3, 5, 7 };
      coalIntensities= new float[] { -1 };
   }

   private void Start()
   {
      if (GridDataManager.Instance)
      {
         windColors = GridDataManager.Instance.GetWindColors();
         sunColors = GridDataManager.Instance.GetSunColors();
         waterColors = GridDataManager.Instance.GetWaterColors();
         coalColors = GridDataManager.Instance.GetCoalColors();
      }
      else
      {
         Debug.LogError("OverlayLegend: Could not find GridDataManager Instance!");
      }
      
      if (intensities?.Length == 0)
         intensities = new float[] { -1, -1, -1, -1 };
       
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
      string unit = "";
      switch (type)
      {
         case OverlayType.WIND:
            title.text = windTitle;
            unit = windUnit;
            intensities = windIntensities;
            
            intensity1.color = windColors[0];
            intensity2.color = windColors[1];
            intensity3.color = windColors[2];
            intensity4.color = windColors[3];
            break;
         case OverlayType.SUN:
            title.text = sunTitle;
            unit = sunUnit;
            intensities = sunIntensities;
            
            intensity1.color = sunColors[0];
            intensity2.color = sunColors[1];
            intensity3.color = sunColors[2];
            intensity4.color = sunColors[3];
            break;
         case OverlayType.WATER:
            title.text = waterTitle;
            unit = waterUnit;
            intensities = waterIntensities;
            
            intensity1.color = waterColors[0];
            intensity2.color = waterColors[1];
            intensity3.color = waterColors[2];
            intensity4.color = waterColors[3];
            break;
         case OverlayType.COAL:
            title.text = coalTitle;
            unit = coalUnit;
            intensities = coalIntensities;
            
            intensity1.color = coalColors[0];
            intensity2.color = coalColors[1];
            intensity3.color = coalColors[2];
            intensity4.color = coalColors[3];
            break;
      }

      for(int index = 0; index < texts.Length; index++)
      {
         Debug.Log(texts[index].text + " " + index);
         if (texts[index].text.Equals("Kohlevorkommen")) 
         {
               texts[index].transform.DOScale(1f, 0f);
               texts[index].transform.DORotate(new Vector3(0, 0, 0), 0f);
               texts[index].transform.DOLocalMoveY(texts[index].transform.localPosition.y + 110f, 0f);
         }
         if (index < intensities.Length)
         {
            float value = intensities[intensities.Length - index - 1];
            if (value.Equals(-1))
            {
               texts[index].transform.DOLocalMoveY(texts[index].transform.localPosition.y - 110f, 0f);
               texts[index].transform.DORotate(new Vector3(0, 0, 90), 0f);
               texts[index].transform.DOScale(2f, 0f);
               texts[index].text = $"{unit}";
            }
            else
            {
               texts[index].text = $"{value}" + $"{unit}";
            }
         }
         else
            texts[index].text = "";
      }
   }
}