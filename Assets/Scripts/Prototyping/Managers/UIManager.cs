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
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Rendering;

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
    [SerializeField] private Texture2D cursorExplosiveTexture;
    [SerializeField] private Texture2D cursorExplosiveDownTexture;
    
    private Texture2D currentCursorTexture;
    private Texture2D currentCursorDownTexture;
    
    [Header("Top Panel")]
    [SerializeField] private TMP_Text budgetText;
    [SerializeField] private TMP_Text currentEnvironmentText;
    [SerializeField] private TMP_Text fulfilledEndpointsText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Sprite nextLevelButtonSprite;
    public Color insufficientBudgetColor;
    
    [Header("Visual")]
    [SerializeField] private Image connectionActiveIndicatorImage;
    [SerializeField] private Image destructionActiveIndicatorImage;

    [Header("Overlays")] 
    [SerializeField] private Material overlayMaterial;

    public bool overlayOpen;
    public OverlayType overlayOpenType;

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
        
        DOTween.SetTweensCapacity(200, 50); 
        DOTween.defaultAutoKill = true;
    }

    private void OnEnable()
    {
        // TODO: Assign new Hotkeys if needed with InputMap
    }

    void Start()
    {
        SetDefaultCursorTextures(); 
        
        // Set Quality Level to Ultra
        SetQualityLevel(5);
        
        // GridDataManager updates the completed endpoints text on Start 
        
        // Assign pause button functionality
        pauseButton.onClick.AddListener(delegate { SettingsManager.Instance.ToggleSettingsPanel(true); });
        
        // TODO: Assign next level button functionality & determine if we need to already unlock it (i.e. the level has already been completed at least once)
        nextLevelButton.onClick.AddListener(GoToNextLevel);
        if (false)
        {
            UnlockNextLevelButton();
        }
        
        // Check if overlay material / object is present
        if (!overlayMaterial)
            throw new Exception("UIManager: No Overlay Material detected!");
        else
            HideOverlay();

    }

    void Update()
    {
        ChangeCursor();
    }

    public void RenderBudget()
    {
        UpdateBudgetColor();
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
            SetCursorTexture();
        }
    }

    private void SetCursorTexture()
    {
        Cursor.SetCursor(currentCursorTexture, Vector2.zero, CursorMode.Auto);
    }
    
    private void SetCursorTextureDown()
    {
        Cursor.SetCursor(currentCursorDownTexture, Vector2.zero, CursorMode.Auto);
    }

    private void SetDefaultCursorTextures()
    {
        Debug.Log("Set default cursor!");
        currentCursorTexture = cursorDefaultTexture;
        currentCursorDownTexture = cursorDownTexture;
        Cursor.SetCursor(currentCursorTexture, Vector2.zero, CursorMode.Auto);
    } 
    
    private void SetDestructionCursorTextures()
    {
        currentCursorTexture = cursorExplosiveTexture;
        currentCursorDownTexture = cursorExplosiveDownTexture;
        Cursor.SetCursor(currentCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void UpdateCurrentEnvironmentalImpact()
    {
        if (LevelManager.Instance && currentEnvironmentText)
            currentEnvironmentText.text = $"{LevelManager.Instance.GetCurrentEnvironmentalImpact():F2} / {LevelManager.Instance.GetMaxEnvironmentalImpact():F2} CO2t";
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
            return;
        }
        connectionActiveIndicatorImage.enabled = !connectionActiveIndicatorImage.enabled;
    }

    public void ActivateConnectingMode()
    {
        ResetMode(); 
        if (Mode != UIState.CONNECTING)
        {
            Mode = UIState.CONNECTING;
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
    public void ToggleConnectingMode(){
        if(Mode == UIState.CONNECTING){
            DeactivateConnectingMode();
        } else{
            ActivateConnectingMode();
        }
    }
    public void DeactivateConnectingMode()
    {
        if (Mode == UIState.CONNECTING)
        {
            Mode = UIState.DEFAULT;
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

    public void ActivateDestructionMode()
    {
        Mode = UIState.DESTROYING;
    }

    public void ToggleDestructionMode()
    {
        if (Mode == UIState.CONNECTING)
            ResetMode();
        
        // If the current mode is Destroying, we go back to default, otherwise switch to destroying
        Mode = Mode == UIState.DESTROYING ? UIState.DEFAULT : UIState.DESTROYING;
        bool enable = Mode == UIState.DESTROYING;
        destructionActiveIndicatorImage.DOFade(enable ? 1f : 0f, 0.5f);
        if (BuilderInventory.Instance)
        {
            if (BuilderInventory.Instance.Expanded())
            {
                BuilderInventory.Instance.HideInventory();
            }
            else
            {
                BuilderInventory.Instance.ShowInventory();
            }
        }

        if (enable)
            SetDestructionCursorTextures();
        else
            SetDefaultCursorTextures();
            
    }

    public void ActivateDefaultMode()
    {
        
        Mode = UIState.DEFAULT;
    }

    public void ResetMode()
    {
        if (Mode == UIState.CONNECTING)
        {
            DeactivateConnectingMode();
        } else if (Mode == UIState.DESTROYING)
        {
            ToggleDestructionMode();
        }
        ActivateDefaultMode();
        SetDefaultCursorTextures();
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

    public void UpdateBudgetColor()
    {
        if (BudgetManager.Instance.GetBudget() < 0)
        {
            budgetText.color = insufficientBudgetColor;
        }
        else
        {
            budgetText.color = Color.white;
        }
    }

    public void GoToNextLevel()
    {
        
    }

    public void ShowOverlay(OverlayType type)
    {
        if (!GridDataManager.Instance || !GridDataManager.Instance.OverlayTexturesAllExistent())
            Debug.LogError("UIManager: Needs GridDataManger Instance and overlay textures to show overlay!");
        
        overlayMaterial.color = Color.white;
        switch (type)
        {
            case OverlayType.WIND:
                overlayMaterial.mainTexture = GridDataManager.Instance.windTexture;
                break;
            case OverlayType.WATER:
                overlayMaterial.mainTexture = GridDataManager.Instance.waterTexture;
                break;
            case OverlayType.SUN:
                overlayMaterial.mainTexture = GridDataManager.Instance.sunTexture;
                break;
            case OverlayType.COAL:
                overlayMaterial.mainTexture = GridDataManager.Instance.coalTexture;
                break;
        }
       overlayOpen = true;
       overlayOpenType = type;
    }

    public void HideOverlay()
    {
       overlayMaterial.color = Color.clear;
       overlayOpen = false;
    }

    public void ToggleOverlay(OverlayType type)
    {
       if (overlayOpen && overlayOpenType == type)
           HideOverlay();
       else
           ShowOverlay(type);
    }


}

public enum UIState 
{
    DEFAULT, CONNECTING, DESTROYING
}

public enum OverlayType
{
    WIND, WATER, SUN, COAL
}
