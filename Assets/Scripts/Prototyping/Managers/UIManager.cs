using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Camera")] 
    public Camera sceneCamera;
    
    [Header("State")] 
    public UIState Mode = UIState.DEFAULT;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;
    
    [Header("Top Panel")]
    [SerializeField] private TMP_Text budgetText;
    [SerializeField] private TMP_Text currentEnvironmentText;
    [SerializeField] private TMP_Text fulfilledEndpointsText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Sprite nextLevelButtonSprite;
    
    [Header("Visual")]
    [SerializeField] private Image connectionActiveIndicatorImage;

    private InputMap inputMap;
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        {
            Instance = this;
        }
        
        Assert.IsNotNull(sceneCamera, "UIManager: sceneCamera not found! Aborting.");
    }

    private void OnEnable()
    {
        // TODO: Assign new Hotkeys if needed
        inputMap = new InputMap();
    }

    void Start()
    {
        SetCursorTextureDefault();
        
        // Set Quality Level to Ultra
        SetQualityLevel(5);
        
        // GridDataManager updates the completed endpoints text at the start to prevent race condition
        
        // Assign pause button functionality
        pauseButton.onClick.AddListener(delegate { SettingsManager.Instance.ToggleSettingsPanel(true); });
        
        // TODO: Assign next level button functionality & determine if we need to already unlock it (i.e. the level has already been completed at least once)
        nextLevelButton.onClick.AddListener(null);
        if (false)
        {
            UnlockNextLevelButton();
        }
    }

    void Update()
    {
        ChangeCursor();
    }

    public void RenderBudget()
    {
        budgetText.text = $"{BudgetManager.Instance.GetBudget():F2}€";
    }
    private void ChangeCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorTextureDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetCursorTextureDefault();
        }
    }

    private void SetCursorTextureDefault()
    {
        Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
    }
    
    private void SetCursorTextureDown()
    {
        Cursor.SetCursor(cursorDownTexture, Vector2.zero, CursorMode.Auto);
    } 

    public void UpdateCurrentEnvironmentalImpact()
    {
        currentEnvironmentText.text = $"{LevelManager.Instance.GetCurrentEnvironmentalImpact():F2} / {LevelManager.Instance.GetMaxEnvironmentalImpact()} CO2t";
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void ToggleConnectionModeIndicator(bool? enable)
    {
        if (enable != null)
        {
            connectionActiveIndicatorImage.DOFade(enable == true ? 1f : 0f, 0.5f);
            Mode = enable == true ? UIState.CONNECTING : UIState.DEFAULT; 
            return;
        }
        connectionActiveIndicatorImage.enabled = !connectionActiveIndicatorImage.enabled;
    }

    public void ActiveConnectingMode()
    {
        if (Mode != UIState.CONNECTING)
        {
            ToggleConnectionModeIndicator(true);
            if (BuilderInventory.Instance)
                BuilderInventory.Instance.HideInventory();
            else
                Debug.LogError("UIManager: No BuilderInventory found!");
        }
        else
        {
            Debug.LogError("UIManager: Already Connecting, should not be able to press this button!");
        }
    }

    public void DeactivateConnectingMode()
    {
        if (Mode == UIState.CONNECTING)
        {
            ToggleConnectionModeIndicator(false);
            if (BuilderInventory.Instance)
                BuilderInventory.Instance.ShowInventory();
            else
                Debug.LogError("UIManager: No BuilderInventory found!");
        }
        else
        {
            Debug.LogError("UIManager: Deactivation of Connecting called even though the state never was connecting in the first place!");
        }
        
    }

    public void SetEndpointsCompleted(int newValue)
    {
        if (LevelManager.Instance)
            fulfilledEndpointsText.text = $"{newValue} / {LevelManager.Instance.endpointsCount}";
        else
            Debug.LogError("GridDataManager: No LevelManager found!");
    }

    public void UnlockNextLevelButton()
    {
        nextLevelButton.interactable = true;
        nextLevelButton.image.sprite = nextLevelButtonSprite;
    }


}

public enum UIState 
{
    DEFAULT, CONNECTING, DESTROYING
}
