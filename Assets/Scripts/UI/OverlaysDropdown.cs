﻿using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

public class OverlaysDropdown : MonoBehaviour
{
   [Header("Elements")] 
   [SerializeField] private List<GameObject> elements;

   public List<TagSelectionTree> allTags;

   public Ease animationEase = Ease.InOutCubic;

   public bool expanded;
   [FormerlySerializedAs("tagsOpen")] public int tagsClosed;
   
   public static OverlaysDropdown Instance { get; private set; }

   private void Awake()
   {
       if (Instance && Instance != this)
       {
           Destroy(this.gameObject);
       }
       else
       {
           Instance = this;
       }
       
       GetComponent<Button>().onClick.AddListener(Toggle);
       
       // Hide every item at the start
       foreach (var element in elements)
       {
           element.transform.localScale = Vector3.zero;
       }
   }

   private void Start()
   {
       // TODO: Properly distinguish between: valid combinations, overlays
       
       Button powerTags = elements[0].AddComponent<Button>();
       powerTags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.POWER);});
       
       Button co2Tags = elements[1].AddComponent<Button>();
       co2Tags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.CO2);});
       
       Button financeTags = elements[2].AddComponent<Button>();
       financeTags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.FINANCE);});
       
       
   }

   public void Expand()
   {
       if (expanded)
           return;
       for (int index = 0; index < elements.Count; index++)
       {
           elements[index].GetComponent<Image>().DOFade(1f, 0.5f).SetEase(animationEase);
           elements[index].transform.DOScale(1f, 0.5f + 0.1f * index).SetEase(animationEase);
           float targetY = GetTargetYPosForTreeElement(index, false);
           elements[index].transform.DOMoveY(targetY , 0.3f).SetEase(animationEase);
       }
       expanded = true;
   }

   public void Collapse()
   {
       if (!expanded)
           return;

       for (int index = 0; index < elements.Count; index++)
       {
           elements[index].GetComponent<Image>().DOFade(0f, 0.5f).SetEase(animationEase);
           elements[index].transform.DOScale(0f, 0.5f + 0.1f * index).SetEase(animationEase);
           float targetY = GetTargetYPosForTreeElement(index, true);
           elements[index].transform.DOMoveY(targetY , 0.3f).SetEase(animationEase);
       }
       expanded = false;
   }

   
   public void Toggle()
   {
       if (expanded)
           Collapse();
       else
           Expand();
   }
   
   private float GetTargetYPosForTreeElement(int index, bool collapsing)
   {
         float yPos = elements[index].transform.position.y;
         float height = elements[index].GetComponent<RectTransform>().rect.height;
         float screenHeightFactor = Screen.height / 1080f;
         float parentScale = transform.lossyScale.x;
         float targetY = (height * parentScale * screenHeightFactor * 2.5f * index);
         targetY = collapsing ? targetY : targetY * -1;
   
         return yPos + targetY;
   }

   public void ToggleAllTagsWithType(TreeTagType type)
   {
       foreach (var tagTree in allTags)
       {
           tagTree.ToggleTag(type);
       }
   }
   
   public void ExpandAllTagsWithType(TreeTagType type)
   {
       foreach (var tagTree in allTags)
       {
           tagTree.OpenTag(type);
       }
   }
   
   public void CollapseAllTagsWithType(TreeTagType type)
   {
       foreach (var tagTree in allTags)
       {
           tagTree.CloseTag(type);
       }
   }
   
   public void ExpandAllTagsFully()
   {
       foreach (var tagTree in allTags)
       {
          tagTree.ExpandTree();
       }
   }

   public void CollapseAllTags()
   {
       foreach (var tagTree in allTags)
       { 
           tagTree.CollapseTree();
       }

   }

   public void ToggleTags()
   {
       AmountTagsClosed();
       if (!AllTagsClosed() && !AllTagsOpen())
       {
           CollapseAllTags();
       } else if (AllTagsClosed())
       {
          ExpandAllTagsFully(); 
       }
       else
       {
          CollapseAllTags(); 
       }
   }
   
    public void ToggleTagsCombination(TreeTagType type)
    {
          AmountTagsClosed();

          if (AllTagsOpen())
              CollapseAllTags();
          else
              ExpandAllTagsWithType(type);
    }

   private bool AllTagsClosed()
   {
       return tagsClosed == allTags.Count;
   }
   
   private bool AllTagsOpen()
   {
       return tagsClosed == 0;
   }

   private void AmountTagsClosed()
   {
       int amountClosed = 0;
       foreach (var tagTree in allTags)
       {
           if (!tagTree.IsExpanded())
           {
               amountClosed++;
           }
       }

       tagsClosed = amountClosed;
   }

   public void AddTag(TagSelectionTree newTag)
   {
       allTags.Add(newTag);
   }

   public void RemoveTag(TagSelectionTree tagToBeRemoved)
   {
       allTags.Remove(tagToBeRemoved);
   }

   private void OnDestroy()
   {
       DOTween.KillAll();
   }
}