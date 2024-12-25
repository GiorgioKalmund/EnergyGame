using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using DG.Tweening;
public class LevelMapMarker : MonoBehaviour
{
    public bool unlocked { get; private set; }
    public int markerID;

    [Header("Neighbours")] 
    [SerializeField] private LevelMapMarker prev;
    [SerializeField] private LevelMapMarker next;
    [FormerlySerializedAs("prevPath")] public List<LevelPathMarker> prevPathSteps;
    
    [Header("Visual")] 
    [SerializeField] private Sprite lockedImage;
    [SerializeField] private Sprite unlockedImage;
    [SerializeField] private TMP_Text displayText;

    private Image _spriteImage;
    private Button toggleButton;
    private float stepLength;

    [Header("Popup")]
    [SerializeField] private GameObject popup;
    private Button popupButton;
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
        stepLength = 1f / (LevelMapManager.Instance.GetPathSteps() + 1);


        // Apply the function to the toggle button
        ClosePopup();
        popup.transform.localScale = Vector3.zero;
        toggleButton.onClick.AddListener(TogglePopup);
        toggleButton.interactable = false; 


        // Apply function to the popup button 
        popupButton = popup.GetComponentInChildren<Button>();
        popupButton.onClick.AddListener(delegate{GameManager.LoadSceneByIdAsync(markerID+1);});
        displayText.text = $"{(markerID + 1)}";
    }

    private void OpenPopup(){
        if (popupOpen)
            return;
        popupOpen = true;
        popup.transform.DOScale(1f, 0.5f);
        float yPos = popup.transform.localPosition.y;
        popup.transform.DOLocalMoveY(yPos + 100, 0.5f);
        transform.SetAsLastSibling();
    }
    private void ClosePopup(){
        if (!popupOpen)
            return;
        popupOpen = false;
        popup.transform.DOScale(0.1f, 0.5f);
        float yPos = popup.transform.localPosition.y;
        popup.transform.DOLocalMoveY(yPos - 100, 0.5f);
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
    
    public void SetNext(LevelMapMarker next)
    {
        this.next = next;
        for (int index = 0; index < LevelMapManager.Instance.GetPathSteps(); index++)
        {
            GameObject pathStep = Instantiate(LevelMapManager.Instance.GetPathObject(), next.gameObject.transform.position, Quaternion.identity);
            pathStep.gameObject.transform.SetParent(LevelMapManager.Instance.GetPathParent().transform);
            pathStep.transform.localScale = new Vector3(1f, 1f, 1f);
            next.prevPathSteps.Add(pathStep.GetComponent<LevelPathMarker>());
        
            Vector3 distance = next.gameObject.transform.position - gameObject.transform.position;
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
}