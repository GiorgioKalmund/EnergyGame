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
    public string displayName = "";

    [Header("Neighbours")] 
    [SerializeField] private LevelMapMarker prev;
    [SerializeField] private LevelMapMarker next;
    public List<LevelPathMarker> prevPathSteps;
    
    [Header("Visual")] 
    [SerializeField] private Sprite lockedImage;
    [SerializeField] private GameObject lockedImageLock;
    [SerializeField] private Sprite unlockedImage;
    [SerializeField] private TMP_Text displayText;
    
    [SerializeField] private Image backdropImage;
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
    
    private SFX sfx;
    
    private void Start()
    {
        LevelMapManager.Instance.AddMarker(this);
        toggleButton = GetComponent<Button>();
        if (!toggleButton)
        {
            gameObject.AddComponent<Button>();
            Debug.LogWarning(name +" didn't have a button component, added one.");
        }

        unlocked = false;
        if (!backdropImage)
            Debug.Log("No backdrop!");
        backdropImage.sprite = lockedImage;


        // Apply the function to the toggle button
        ClosePopup();
        popup.transform.localScale = Vector3.zero;
        toggleButton.onClick.AddListener(TogglePopup);
        toggleButton.interactable = false; 


        // Apply function to the popup button 
        popup.SetActive(true);
        popupDescriptionText.text = displayName; 
        popupButton = popup.GetComponentInChildren<Button>();
        popupButton.onClick.AddListener(delegate{SceneManager.LoadScene(linkedSceneName);});
        displayText.text = "";
        
        // Set scale, so hover animations work properly afterwards
        gameObject.transform.DOScale(1f, 0.1f);

        DOTween.KillAll();
        DOTween.defaultRecyclable = true;
    }

    private void OnEnable()
    {
        sfx = GameObject.FindWithTag("SFX").GetComponent<SFX>();
    }

    private void OpenPopup(){
        if (popupOpen)
            return;
        
        if (LevelMapManager.Instance.CurrentlySelectedMarker)
            LevelMapManager.Instance.CurrentlySelectedMarker.ClosePopup();
        
        popupOpen = true;
        
        // Scale back to regular size once popup is open
        gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutElastic);
        
        if (!popup.transform)
            Debug.LogError(name + ": No Transform found for Popup");
        
        popupButton.interactable = false;
        popup.transform.DOScale(1f, popupTime).SetRecyclable();
        
        float yPos = popup.transform.localPosition.y;
        popup.transform.DOLocalMoveY(yPos + popupYOffset, popupTime).SetEase(popupEase).SetRecyclable();
        popup.transform.DOShakeRotation(shakeTime, shakeStrength).SetRecyclable();

        //Move to the top of the hierarchy, avoiding overshadowing
        transform.SetAsLastSibling();
        LevelMapManager.Instance.CurrentlySelectedMarker = this;
        
        popupButton.interactable = true;
        sfx.LevelSelect();
    }
    public void ClosePopup(){
        if (!popupOpen)
            return;
        
        popupOpen = false;
        if (popup.transform)
            popup.transform.DOScale(0.0f, popupTime).SetRecyclable();
         
        float yPos = popup.transform.localPosition.y;
        
        if (popup.transform)
            popup.transform.DOLocalMoveY(yPos - popupYOffset,popupTime).SetEase(popupEase).SetRecyclable();
        
        sfx.LevelUnselect();
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
        lockedImageLock.SetActive(false);
        displayText.text = $"{(markerID + 1)}";
        backdropImage.sprite = unlockedImage;
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
        backdropImage.sprite = lockedImage;
        lockedImageLock.SetActive(true);
        displayText.text = "";
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
        int steps = Mathf.CeilToInt(distance.magnitude / (Screen.width / 9.6f));
        float stepLength = 1f / (steps + 1);
        
        for (int index = 0; index < steps; index++)
        {
            GameObject pathStep = Instantiate(LevelMapManager.Instance.GetPathObject(), next.gameObject.transform.position, Quaternion.identity);
            pathStep.gameObject.transform.SetParent(LevelMapManager.Instance.GetPathParent().transform);
            float scale = LevelMapManager.Instance.pathScale;
            pathStep.transform.localScale = new Vector3(scale, scale, scale);
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
        
        if (gameObject.transform != null)
            gameObject.transform.DOScale(1.1f, 0.2f).SetEase(Ease.InOutElastic);
    }
    
    public void OnPointerExit(PointerEventData data)
    {
        if (gameObject.transform != null)
            gameObject.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutElastic);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}