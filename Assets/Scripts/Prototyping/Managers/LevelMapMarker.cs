using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelMapMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool unlocked { get; private set; }
    public int markerID;
    public string linkedSceneName = "";

    [Header("Neighbours")] 
    [SerializeField] private LevelMapMarker prev;
    [SerializeField] private LevelMapMarker next;
    public List<LevelPathMarker> prevPathSteps;
    
    [Header("Visual")] 
    [SerializeField] private Sprite lockedImage;
    [SerializeField] private Sprite unlockedImage;
    [SerializeField] private TMP_Text displayText;

    private Image _spriteImage;
    private Button toggleButton;

    [Header("Popup")]
    [SerializeField] private GameObject popup;

    [SerializeField] private float popupTime = 0.33f;
    [SerializeField] private float popupYOffset = 100;
    [SerializeField] private float shakeTime = 0.5f;
    [SerializeField] private float shakeStrength = 5f;
    public Ease popupEase = Ease.InOutCubic;
    private Button popupButton;
    [SerializeField] private TMP_Text popupDescriptionText;
    private bool popupOpen;
    
    private void Start()
    {
        LevelMapManager.Instance.AddMarker(this);
        _spriteImage = GetComponent<Image>();
        toggleButton = GetComponent<Button>();
        if (!toggleButton)
        {
            gameObject.AddComponent<Button>();
            Debug.LogWarning(name +" didn't have a button component, added one.");
        }

        unlocked = false;
        _spriteImage.sprite = lockedImage;


        // Apply the function to the toggle button
        ClosePopup();
        popup.transform.localScale = Vector3.zero;
        toggleButton.onClick.AddListener(TogglePopup);
        toggleButton.interactable = false; 


        // Apply function to the popup button 
        popup.SetActive(true);
        popupDescriptionText.text = linkedSceneName;
        popupButton.onClick.AddListener(delegate{SceneManager.LoadScene(linkedSceneName);});
        displayText.text = $"{(markerID + 1)}";
        
        // Set scale, so hover animations work properly afterwards
        gameObject.transform.DOScale(1f, 0.1f);
    }

    private void OpenPopup(){
        if (popupOpen)
            return;
        if (LevelMapManager.Instance.CurrentlySelectedMarker)
        {
            LevelMapManager.Instance.CurrentlySelectedMarker.ClosePopup();
        }
        popupOpen = true;
        
        // Scale back to regular size once popup is open
        gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutElastic);
        
        popup.transform.DOScale(1f, popupTime);
        float yPos = popup.transform.localPosition.y;
        popup.transform.DOLocalMoveY(yPos + popupYOffset, popupTime).SetEase(popupEase);
        popup.transform.DOShakeRotation(shakeTime, shakeStrength);
        
        //Move to the top of the hierarchy, avoiding overshadowing
        transform.SetAsLastSibling();
        LevelMapManager.Instance.CurrentlySelectedMarker = this;
    }
    public void ClosePopup(){
        if (!popupOpen)
            return;
        
        popupOpen = false;
        
        popup.transform.DOScale(0.0f, popupTime);
        float yPos = popup.transform.localPosition.y;
        popup.transform.DOLocalMoveY(yPos - popupYOffset,popupTime).SetEase(popupEase);
    }

    private void TogglePopup()
    {
        if (popupOpen)
        {
            ClosePopup();
            return;
        }
        OpenPopup();
    }


    public void Unlock()
    {
        toggleButton.interactable = true; 
        if (prev)
        {
            prev.Unlock();
        }

        foreach (LevelPathMarker step in prevPathSteps)
        {
            step.Unlock();
        }
        
        unlocked = true;
        _spriteImage.sprite = unlockedImage;
    }

    public void Lock()
    {
        toggleButton.interactable = false; 
        if (next)
        {
            next.Lock();
        }
        foreach (LevelPathMarker step in prevPathSteps)
        {
            step.Lock();
        }
        unlocked = false;
        _spriteImage.sprite = lockedImage;
        popupButton.onClick.RemoveAllListeners();
    }

    public void ToggleLock()
    {
        if (unlocked)
        {
            Lock();
        }
        else
        {
            Unlock();
        }
    }

    public LevelMapMarker GetNext()
    {
        return next;
    }
    
    /**
     *  Connect current marker to next
     *
     *  Calculate "bread crumb" (LevelPathMarker) distance and count, then place them accordingly 
     */
    public void SetNext(LevelMapMarker next)
    {
        this.next = next;
        Vector3 distance = next.gameObject.transform.position - gameObject.transform.position;
        int steps = Mathf.CeilToInt(distance.magnitude / 200f);
        float stepLength = 1f / (steps + 1);
        
        for (int index = 0; index < steps; index++)
        {
            GameObject pathStep = Instantiate(LevelMapManager.Instance.GetPathObject(), next.gameObject.transform.position, Quaternion.identity);
            pathStep.gameObject.transform.SetParent(LevelMapManager.Instance.GetPathParent().transform);
            pathStep.transform.localScale = new Vector3(1f, 1f, 1f);
            next.prevPathSteps.Add(pathStep.GetComponent<LevelPathMarker>());
        
            pathStep.transform.position -= distance * stepLength * (index + 1);
        }
    }

    public LevelMapMarker GetPrev()
    {
        return prev;
    }
    
    public void SetPrev(LevelMapMarker prev)
    {
        this.prev = prev;
    }
    
   public void OnPointerEnter(PointerEventData data)
    {
        if (!unlocked || popupOpen)
        {
            return;
        }
        gameObject.transform.DOScale(1.1f, 0.2f).SetEase(Ease.InOutElastic);
    }
    
    public void OnPointerExit(PointerEventData data)
    {
        gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutElastic);
    }

}