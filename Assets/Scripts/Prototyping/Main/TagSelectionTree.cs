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
  [Header("Tags")] [SerializeField] private TagSelectionElement productionTag;
  [SerializeField] private TagSelectionElement co2Tag;
  [SerializeField] private TagSelectionElement financeTag;

  [Header("Availabilities")] public bool productionAvailable;
  public bool co2Available;
  public bool financeAvailable;

  private List<TagSelectionElement> tags;
  public bool expanded;


  [Header("Values")] [SerializeField] private TMP_Text currentProductionText;
  [SerializeField] private TMP_Text environmentalImpactText;
  [SerializeField] private TMP_Text financeText;


  [Header("Endpoint")] [SerializeField] private bool isEndpoint = false;
  [SerializeField] private Image productionBackdropImage;

  [SerializeField] private Sprite endpointBackdrop;

  private void Awake()
  {
    productionTag.type = TreeTagType.POWER;
    co2Tag.type = TreeTagType.CO2;
    financeTag.type = TreeTagType.FINANCE;
  }

  private void OnEnable()
  {


    tags = new List<TagSelectionElement>() { productionTag, co2Tag, financeTag };


    // Deactivate relevant tree elements
    productionTag.SetActive(productionAvailable);
    co2Tag.SetActive(co2Available);
    financeTag.SetActive(financeAvailable);

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

    if (OverlaysDropdown.Instance)
      OverlaysDropdown.Instance.AddTag(this);

    if (isEndpoint)
    {
      productionBackdropImage.sprite = endpointBackdrop;
      productionTag.OpenSilently();
    }

  }

  private void Update()
  {
    RotateTowardsCamera();
  }

  public void CollapseTree()
  {
    if (isEndpoint)
      return;

    foreach (var tagElement in tags)
    {
      tagElement.Close();
    }
  }

  public void CollapseTreeSilently()
  {
    if (isEndpoint)
      return;

    foreach (var tagElement in tags)
    {
      tagElement.CloseSilently();
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

  public void ExpandTreeSilently()
  {
    if (productionAvailable)
      productionTag.OpenSilently();
    if (co2Available)
      co2Tag.OpenSilently();
    if (financeAvailable)
      financeTag.OpenSilently();
  }

public void ToggleTag(TreeTagType type)
  {
    if (type == TreeTagType.POWER && productionAvailable)
      ToggleProductionTag(); 
    else if (type == TreeTagType.CO2 && co2Available)
      ToggleCo2Tag();
    else if (financeAvailable)
      ToggleFinanceTag();
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
    if (isEndpoint)
      return;
    
    if (type == TreeTagType.POWER)
      productionTag.Close();
    else if (type == TreeTagType.CO2)
      co2Tag.Close();
    else
      financeTag.Close();
  }

  public void Setup(ProducerDescriptor descriptor)
  {
    //currentProductionText.text = $"{descriptor.GetMaxProduction()} MW";
    environmentalImpactText.text = $"{descriptor.GetEnvironmentalImpact():F0} t CO<sub>2</sub>";
    financeText.text = $"{descriptor.GetCost():F0} Mio €";
  }

  private void RotateTowardsCamera()
  {
    transform.parent.LookAt(UIManager.Instance.sceneCamera.transform);
    transform.parent.Rotate(0, 180, 0);
  }

  public void SetProductionText(float value)
  {
    currentProductionText.text = $"{Mathf.Floor(value)}MW";
  }

  public void SetEndpointProductionText(float current, float goal)
  {
    if (!isEndpoint)
      return;
    
    currentProductionText.text = $"{Mathf.Floor(current)}MW / {Mathf.Floor(goal)}MW";
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
    if (isEndpoint)
      return;
    
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