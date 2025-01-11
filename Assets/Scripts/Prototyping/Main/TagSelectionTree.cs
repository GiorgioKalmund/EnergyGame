using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TagSelectionTree : MonoBehaviour
{
  [Header("Tags")] 
  [SerializeField] private TagSelectionElement productionTag;
  [SerializeField] private TagSelectionElement co2Tag;
  [SerializeField] private TagSelectionElement financeTag;

  [Header("Availabilities")] 
  public bool productionAvailable;
  public bool co2Available;
  public bool financeAvailable;

  private List<TagSelectionElement> tags;
  public bool expanded;

  private InputMap inputMap;
  
  [Header("Values")]
  [SerializeField] private TMP_Text currentProductionText;
  [SerializeField] private TMP_Text environmentalImpactText;
  [SerializeField] private TMP_Text financeText;

  private Action<InputAction.CallbackContext> showPowerAction;
  private Action<InputAction.CallbackContext> showCO2Action;
  private Action<InputAction.CallbackContext> showFinanceAction;
  private Action<InputAction.CallbackContext> collapseAllTagsAction;

  private void Awake()
  {
    productionTag.type = TreeTagType.POWER;
    co2Tag.type = TreeTagType.CO2;
    financeTag.type = TreeTagType.FINANCE;
  }

  private void OnEnable()
  {
    
    // Create a new map and subscribe every combo to it
    inputMap = new InputMap();
    inputMap.main.Enable();

    tags = new List<TagSelectionElement>() { productionTag, co2Tag, financeTag };

    showPowerAction = ctx => { ToggleProductionTag(); };
    showCO2Action = ctx => { ToggleCo2Tag(); };
    showFinanceAction = ctx => { ToggleFinanceTag(); };
    collapseAllTagsAction = ctx => { CollapseTree(); };

    inputMap.main.ShowPower.performed += showPowerAction;
    inputMap.main.ShowCO2.performed += showCO2Action;
    inputMap.main.ShowFinance.performed += showFinanceAction;
    inputMap.main.CollapseAllTags.performed += collapseAllTagsAction;
    
    // Deactivate relevant tree elements
    productionTag.SetActive(productionAvailable);
    co2Tag.SetActive(co2Available);
    financeTag.SetActive(financeAvailable);

  }

  private void OnDisable()
  {
    // Unsubscribe and disable input for this specific instance 
    inputMap.main.ShowPower.performed -= showPowerAction;
    inputMap.main.ShowCO2.performed -= showCO2Action;
    inputMap.main.ShowFinance.performed -= showFinanceAction;
    inputMap.main.CollapseAllTags.performed -= collapseAllTagsAction;

    inputMap.main.Disable();
  }

  private void Start()
  {
    transform.parent.GetComponent<Canvas>().worldCamera = UIManager.Instance.sceneCamera;
    RotateTowardsCamera(); 
    
    var globallyActive = OverlaysDropdown.Instance.globallyActiveTypes;
    if (globallyActive.Contains(TreeTagType.POWER))
      productionTag.Open();
    if (globallyActive.Contains(TreeTagType.CO2))
      co2Tag.Open();
    if (globallyActive.Contains(TreeTagType.FINANCE))
      financeTag.Open();
  }

  private void Update()
  {
    RotateTowardsCamera();
  }

  public void CollapseTree()
  {
    foreach (var tagElement in tags)
    {
      tagElement.Close(); 
    }
  }
  public void ExpandTree()
  {
    if (productionAvailable)
      productionTag.Open();
    if (co2Available)
      co2Tag.Open();
    if (financeAvailable)
      financeTag.Open();
  }
  public void ToggleTag(TreeTagType type)
  {
    if (type == TreeTagType.POWER && productionAvailable)
      productionTag.Toggle();
    else if (type == TreeTagType.CO2 && co2Available)
      co2Tag.Toggle();
    else if (financeAvailable)
      financeTag.Toggle();
  }

  public void OpenTag(TreeTagType type)
  {
    if (type == TreeTagType.POWER && productionAvailable)
      productionTag.Open();
    else if (type == TreeTagType.CO2 && co2Available)
      co2Tag.Open();
    else if (financeAvailable)
      financeTag.Open();
  }
  
  public void CloseTag(TreeTagType type)
  {
    if (type == TreeTagType.POWER)
      productionTag.Close();
    else if (type == TreeTagType.CO2)
      co2Tag.Close();
    else
      financeTag.Close();
  }

  public void Setup(ProducerDescriptor descriptor)
  {
    currentProductionText.text = $"{descriptor.GetMaxProduction()} MW";
    environmentalImpactText.text = $"{descriptor.GetEnvironmentalImpact()} CO2t";
    financeText.text = $"{descriptor.GetCost()}€";
    
    // Add itself to the global list contained in the OverlaysDropdown
    if (OverlaysDropdown.Instance)
      OverlaysDropdown.Instance.AddTag(this);
  }

  private void RotateTowardsCamera()
  {
    transform.parent.LookAt(UIManager.Instance.sceneCamera.transform);
    transform.parent.Rotate(0, 180, 0);
  }

  public void SetProductionText(float value)
  {
    currentProductionText.text = $"{value:F2}MW";
  }

  public bool IsExpanded()
  {
    return expanded;
  }

  private void OnDestroy()
  {
    if (OverlaysDropdown.Instance)
      OverlaysDropdown.Instance.RemoveTag(this);
  }

  private void ToggleProductionTag()
  {
    productionTag.Toggle();
  }
  private void ToggleCo2Tag()
  {
    co2Tag.Toggle();
  }
  private void ToggleFinanceTag()
  {
    financeTag.Toggle();
  }
}


public enum TreeTagType
{
  POWER, CO2, FINANCE
}