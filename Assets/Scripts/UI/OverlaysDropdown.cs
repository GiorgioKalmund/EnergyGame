﻿using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;


public class OverlaysDropdown : MonoBehaviour
{
    [Header("Text")] 
    [SerializeField] private TMP_Text toggleText;
    public float textAnimationTime = 0.1f;
    
   [Header("Elements")] 
   [SerializeField] private List<GameObject> elements;

   public List<TagSelectionTree> allTags;
   public HashSet<TreeTagType> globallyActiveTypes;
    
   [Tooltip("4 - WIND\n5 - SOLAR\n6 - WATER\n7 - COAL")]
   [SerializeField] private int[] elementsToLock;

   public Ease animationEase = Ease.InOutCubic;

   public bool expanded;
   public int tagsClosed;
   
   public static OverlaysDropdown Instance { get; private set; }

  private InputMap inputMap;
  private Action<InputAction.CallbackContext> showPowerAction;
  private Action<InputAction.CallbackContext> showCO2Action;
  private Action<InputAction.CallbackContext> showFinanceAction;
  private Action<InputAction.CallbackContext> collapseAllTagsAction;
  private Action<InputAction.CallbackContext> toggleDropdownAction;
  

  [SerializeField] private Sprite lockSprite;
  [SerializeField] private Sprite lockBackdropSprite;

  public bool PowerOpen { get; private set;  }
  public bool Co2Open { get; private set;  }
  public bool FinanceOpen { get; private set; }
  
   private float resetYPosition;

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
       globallyActiveTypes = new HashSet<TreeTagType>();
       
       // Hide every item at the start
       foreach (var element in elements)
       {
           element.transform.localScale = Vector3.zero;
       }

       PowerOpen = false;
       Co2Open = false;
       FinanceOpen = false;
   }

   private void OnEnable()
   {
        // Create a new map and subscribe every combo to it
        inputMap = new InputMap();
        inputMap.main.Enable();
        
        showPowerAction = ctx => { ToggleAllTagsWithType(TreeTagType.POWER); };
        showCO2Action = ctx => { ToggleAllTagsWithType(TreeTagType.CO2); };
        showFinanceAction = ctx => { ToggleAllTagsWithType(TreeTagType.FINANCE); };
        collapseAllTagsAction = ctx => { CollapseAllTags(); };
        toggleDropdownAction = ctx => { Toggle(); };

        inputMap.main.ShowPower.performed += showPowerAction;
        inputMap.main.ShowCO2.performed += showCO2Action;
        inputMap.main.ShowFinance.performed += showFinanceAction;
        inputMap.main.CollapseAllTags.performed += collapseAllTagsAction;
        inputMap.main.ToggleDropdown.performed += toggleDropdownAction;
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
       Button powerTags = elements[0].GetComponent<Button>();
       powerTags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.POWER);});
       
       Button co2Tags = elements[1].GetComponent<Button>();
       co2Tags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.CO2);});
       
       Button financeTags = elements[2].GetComponent<Button>();
       financeTags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.FINANCE);});
        
       // Skip one index for the "divider"
       
       Button sunOverlay = elements[4].GetComponent<Button>();
       sunOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.WIND);});
       
       Button windOverlay = elements[5].GetComponent<Button>();
       windOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.SUN);});

       Button waterOverlay = elements[6].GetComponent<Button>();
       waterOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.WATER);});
       
       Button coalOverlay = elements[7].GetComponent<Button>();
       coalOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.COAL);});
       
       resetYPosition = elements[0].transform.localPosition.y;

        if (BuilderInventory.Instance.isConstructionInventory)
        {
            LockElements(BuilderInventory.Instance.getConstructionSlotToUnlock());
            
        }
        LockElements(elementsToLock);
    }
    private void LockElements(int constructionSlotToUnlock)
    {
        // Use switch-case to lock specific elements
        switch (constructionSlotToUnlock)
        {
            case 0:
                LockElement(5);
                LockElement(6);
                LockElement(7);
                break;
            case 1:
                LockElement(4);
                LockElement(6);
                LockElement(7);
                break;
            case 2:
                LockElement(4);
                LockElement(5);
                LockElement(7);
                break;
            case 3:
                LockElement(4);
                LockElement(5);
                LockElement(6);
                break;
            default:
                break;
        }
    }

    private void LockElements(int[] e2Lock)
    {
        foreach (var value in e2Lock)
        {
            LockElement(value);
        }
    }
    private void LockElement(int index)
    {
        GameObject element = elements[index];

        if (element != null && lockSprite != null)
        {
            Image[] images = element.GetComponentsInChildren<Image>();
            Image backdrop;
            if (images[0].name.Equals("Backdrop"))
                backdrop = images[0];
            else
                backdrop = images[1];
            
            backdrop.sprite = lockBackdropSprite;
            element.GetComponent<Button>().interactable = false;
            GameObject lockImageObj = new GameObject("LockImage");
            Image lockImage = lockImageObj.AddComponent<Image>();

            lockImage.sprite = lockSprite;

            lockImageObj.transform.SetParent(element.transform, false);
            lockImageObj.transform.localPosition = Vector3.zero; 
            lockImage.rectTransform.sizeDelta = new Vector2(50, 50); 
        }
    
    }

    
    public async void Expand()
   {
       if (expanded)
           return;
       for (int index = 0; index < elements.Count; index++)
       {
           elements[index].GetComponent<Image>().DOFade(1f, 0.5f).SetEase(animationEase);
           elements[index].transform.DOScale(1f, 0.5f + 0.1f * index).SetEase(animationEase);
           float targetY = GetTargetYPosForTreeElement(index, false);
           elements[index].transform.DOLocalMoveY(targetY , 0.3f).SetEase(animationEase);
       }

       await toggleText.DOFade(0f, textAnimationTime).AsyncWaitForCompletion();
       toggleText.text = "Schilder";
       toggleText.DOFade(1f, textAnimationTime);
       expanded = true;
   }

   public async void Collapse()
   {
       if (!expanded)
           return;

       for (int index = 0; index < elements.Count; index++)
       {
           elements[index].GetComponent<Image>().DOFade(0f, 0.5f).SetEase(animationEase);
           elements[index].transform.DOScale(0f, 0.5f + 0.1f).SetEase(animationEase);
           float targetY = GetTargetYPosForTreeElement(index, true);
           elements[index].transform.DOLocalMoveY(targetY , 0.3f).SetEase(animationEase);
       }
       
       await toggleText.DOFade(0f, textAnimationTime).AsyncWaitForCompletion();
       toggleText.text = "Schilder & Karten";
       toggleText.DOFade(1f, textAnimationTime);
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
       if (collapsing)
           return resetYPosition;
       
       float yPos = elements[index].transform.localPosition.y;
       float height = elements[index].GetComponent<RectTransform>().rect.height;
       float targetY = (height *  1.5f * index);
       
       return yPos - targetY;
   }

   public void ToggleAllTagsWithType(TreeTagType type)
   {
       bool openTags = false;
       if (type == TreeTagType.POWER)
       {
           PowerOpen = !PowerOpen;
           openTags = PowerOpen;
       }
       else if (type == TreeTagType.CO2)
       {
           Co2Open = !Co2Open;
           openTags = Co2Open;
       }
       else if (type == TreeTagType.FINANCE)
       {
           FinanceOpen = !FinanceOpen;
           openTags = FinanceOpen;
       }
       foreach (var tagTree in allTags)
       {
           if (openTags)
            tagTree.OpenTag(type);
           else
               tagTree.CloseTag(type);
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