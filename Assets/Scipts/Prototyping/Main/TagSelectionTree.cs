using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.InputSystem;

public class TagSelectionTree : MonoBehaviour
{
  [SerializeField] private List<GameObject> tagTree;
  public List<GameObject> activeTagTree;
  public List<TreeTagType> treeTypes = new List<TreeTagType>();
  public Ease animationEase = Ease.InOutCubic;
  private bool expanded;
  private InputMap inputMap;

  private void Start()
  {
    // Hide every tag at first
    foreach (var tag in tagTree)
    {
      tag.transform.localScale = Vector3.zero;
    }
  }

  private void OnEnable()
  {
    // TODO: Move to central script
    inputMap = new InputMap();
    inputMap.main.Enable();

    inputMap.main.ShowPower.performed += ctx => {ToggleTreeCombination(1);};
    inputMap.main.ShowCO2.performed += ctx => {ToggleTreeCombination(2);};
    inputMap.main.CollapseAllTags.performed += ctx => {CollapseTree();};
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
      ExpandTree(treeTypes);
    }
  }

  /**
   *  0 - None  (0b000)
   *  1 - Power (0b001)
   *  2 - CO2   (0b010)
   *  3 - Power + CO2   (Ob011)
   *  4 - Sell  (Ob100)
   *  5 - Power + Sell  (0b101)
   *  6 - CO2 + Sell    (0b110)
   *  7 - Power + CO2 + Sell  (0b111)
   */
  public void ToggleTreeCombination(int combination)
  {
    if (expanded)
    {
      CollapseTree();
    }
    else
    {
      List<TreeTagType> types = new List<TreeTagType>();
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
        types.Add(TreeTagType.SELL);
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
      float yPos = activeTagTree[index].transform.position.y;
      float height = activeTagTree[index].GetComponent<RectTransform>().rect.height;
      activeTagTree[index].transform.DOMoveY(yPos + height * 1.1f * index, 0.3f).SetEase(animationEase);
    }

    for (int index = activeTagTree.Count - 1; index >= 0; index--)
    {
      activeTagTree[index].transform.DOScale(0f, 0.5f + 0.2f * index).SetEase(animationEase);
      activeTagTree[index].GetComponent<Image>().DOFade(0f, 0.5f).SetEase(animationEase);
    }
    expanded = false;
    activeTagTree.Clear();
  }

  public void ExpandTree(List<TreeTagType> types)
  {
    if (expanded)
    {
      return;
    }
    
    CalculateTagsToShow(types);

    for (int index = 0; index < activeTagTree.Count; index++)
    {
      activeTagTree[index].transform.DOScale(1f, 0.5f + 0.2f * index).SetEase(animationEase);
      activeTagTree[index].GetComponent<Image>().DOFade(1f, 0.5f).SetEase(animationEase);
    }
    for (int index = 0; index < activeTagTree.Count; index++)
    {
      float yPos = activeTagTree[index].transform.position.y;
      float height = activeTagTree[index].GetComponent<RectTransform>().rect.height;
      activeTagTree[index].transform.DOMoveY(yPos - height * 1.1f * index, 0.3f).SetEase(animationEase);
    }
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
    if (types.Contains(TreeTagType.SELL) && treeTypes.Contains(TreeTagType.SELL))
    {
      activeTagTree.Add(tagTree[2]);
    }
  }
}


public enum TreeTagType
{
  POWER, CO2, SELL
}