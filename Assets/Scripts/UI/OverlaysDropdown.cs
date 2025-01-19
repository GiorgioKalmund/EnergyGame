    using System;
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
       Button powerTags = elements[0].AddComponent<Button>();
       powerTags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.POWER);});
       
       Button co2Tags = elements[1].AddComponent<Button>();
       co2Tags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.CO2);});
       
       Button financeTags = elements[2].AddComponent<Button>();
       financeTags.onClick.AddListener(delegate{ToggleAllTagsWithType(TreeTagType.FINANCE);});
        
       // Skip one index for the "divider"
       
       Button sunOverlay = elements[4].AddComponent<Button>();
       sunOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.WIND);});
       
       Button windOverlay = elements[5].AddComponent<Button>();
       windOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.SUN);});

       Button waterOverlay = elements[6].AddComponent<Button>();
       waterOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.WATER);});
       
       Button coalOverlay = elements[7].AddComponent<Button>();
       coalOverlay.onClick.AddListener(delegate{UIManager.Instance.ToggleOverlay(OverlayType.COAL);});
       
       resetYPosition = elements[0].transform.localPosition.y;
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
       toggleText.text = "Tags";
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
       toggleText.text = "Tags & Overlays";
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