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
  [SerializeField] private List<GameObject> tagTree;
  public List<GameObject> activeTagTree;
  public List<TreeTagType> treeTypes;
  private int lastPressedCombination;
  private int tagsOpen;
  public Ease animationEase = Ease.InOutCubic;
  private bool expanded;
  private InputMap inputMap;
  
  [Header("Values")]
  [SerializeField] private TMP_Text currentProductionText;
  [SerializeField] private TMP_Text environmentalImpactText;
  [SerializeField] private TMP_Text financeText;


  private void OnEnable()
  {
    // TODO: Move to central script
    inputMap = new InputMap();
    inputMap.main.Enable();

    inputMap.main.ShowPower.performed += ctx => {ToggleTreeCombination(1);};
    inputMap.main.ShowCO2.performed += ctx => {ToggleTreeCombination(2);};
    inputMap.main.ShowFinance.performed += ctx => {ToggleTreeCombination(4);};
    inputMap.main.CollapseAllTags.performed += ctx => {CollapseTree();};
  }
  
  private void Start()
  {
    // Hide every tag at first
    foreach (var tag in tagTree)
    {
      tag.transform.localScale = Vector3.zero;
    }
    
    transform.parent.GetComponent<Canvas>().worldCamera = UIManager.Instance.sceneCamera;
    RotateTowardsCamera(); 
  }

  private void Update()
  {
    RotateTowardsCamera();
  }

  public void Setup(ProducerDescriptor descriptor)
  {
    currentProductionText.text = $"{descriptor.GetMaxProduction()} MW";
    environmentalImpactText.text = $"{descriptor.GetEnvironmentalImpact()} CO2t";
    financeText.text = $"{descriptor.GetCost()}€";
  }

  public void ToggleTree()
  {
    if (expanded)
    {
      CollapseTree();
    }
    else
    {
      // Assume we want to see everything when calling this function, if single tags want to be visible use the other function
      lastPressedCombination = 7;
      ExpandTree(treeTypes);
      tagsOpen = activeTagTree.Count;
    }
  }

  /**
   *  0 - None  (0b000)
   *  1 - Power (0b001)
   *  2 - CO2   (0b010)
   *  3 - Power + CO2   (Ob011)
   *  4 - Finance (Ob100)
   *  5 - Power + Finance(0b101)
   *  6 - CO2 + Sell    (0b110)
   *  7 - Power + CO2 + Finance (0b111)
   */
  public void ToggleTreeCombination(int combination)
  {
    // If expanded and full, collapse back on toggle call
    if (expanded && lastPressedCombination == combination)
    {
      CollapseTree();
    }
    else
    {
      List<TreeTagType> types = new List<TreeTagType>();
      lastPressedCombination = combination;
      if ((combination & 1) == 1)
      {
        types.Add(TreeTagType.POWER);
      }
      if ((combination & 2) == 2)
      {
        types.Add(TreeTagType.CO2);
      }
      if ((combination & 4) == 4)
      {
        types.Add(TreeTagType.FINANCE);
      }
      ExpandTree(types);
    }
    
  }

  public void CollapseTree()
  {
    if (!expanded)
    {
      return;
    }
    
    for (int index = 0; index < activeTagTree.Count; index++)
    {
      float targetY = GetTargetYPosForTreeElement(index, true);
      activeTagTree[index].transform.DOMoveY(targetY, 0.3f).SetEase(animationEase);
    }

    for (int index = activeTagTree.Count - 1; index >= 0; index--)
    {
      activeTagTree[index].transform.DOScale(0f, 0.5f + 0.2f * index).SetEase(animationEase);
      activeTagTree[index].GetComponent<Image>().DOFade(0f, 0.5f).SetEase(animationEase);
    }
    expanded = false;
    activeTagTree.Clear();
    tagsOpen = activeTagTree.Count;
  }

  public void ExpandTree(List<TreeTagType> types)
  {
    // If all available tags have been displayed, return
    if (activeTagTree.Count == treeTypes.Count)
    {
      return;
    }

    tagsOpen = activeTagTree.Count; 
    
    CalculateTagsToShow(types);
    
    for (int index = tagsOpen; index < activeTagTree.Count; index++)
    {
      activeTagTree[index].transform.DOScale(1f, 0.5f + 0.2f * index).SetEase(animationEase);
      activeTagTree[index].GetComponent<Image>().DOFade(1f, 0.5f).SetEase(animationEase);
    }
    
    for (int index = tagsOpen; index < activeTagTree.Count; index++)
    {
      float targetY = GetTargetYPosForTreeElement(index, false);
      activeTagTree[index].transform.DOMoveY(targetY , 0.3f).SetEase(animationEase);
    }

    RotateTowardsCamera(); 
    expanded = true;
  }
  
  private void CalculateTagsToShow(List<TreeTagType> types)
  {
    // If both the new list, as well as the global list for this Instance contain the requested type, add object to list to be shown
    if (types.Contains(TreeTagType.POWER) && treeTypes.Contains(TreeTagType.POWER))
    {
      activeTagTree.Add(tagTree[0]);
    }
    if (types.Contains(TreeTagType.CO2) && treeTypes.Contains(TreeTagType.CO2))
    {
      activeTagTree.Add(tagTree[1]);
    }
    if (types.Contains(TreeTagType.FINANCE) && treeTypes.Contains(TreeTagType.FINANCE))
    {
      activeTagTree.Add(tagTree[2]);
    }
  }

  private void RotateTowardsCamera()
  {
    transform.parent.LookAt(UIManager.Instance.sceneCamera.transform);
    transform.parent.Rotate(0, 180, 0);
  }

  private float GetTargetYPosForTreeElement(int index, bool collapsing)
  {
      float yPos = activeTagTree[index].transform.position.y;
      float height = activeTagTree[index].GetComponent<RectTransform>().rect.height;
      float screenHeightFactor = Screen.height / 1080f;
      float parentScale = transform.lossyScale.x;
      float targetY = (height * parentScale * screenHeightFactor * 2f * index);
      targetY = collapsing ? targetY : targetY * -1;

      return yPos + targetY;
  }

  public void SetProductionText(float value)
  {
    currentProductionText.text = $"{value:F2}MW";
  }
}


public enum TreeTagType
{
  POWER, CO2, FINANCE
}