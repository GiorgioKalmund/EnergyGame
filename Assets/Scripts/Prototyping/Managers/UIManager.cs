using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Cursor")]
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;
    
    [Header("Budget")]
    [SerializeField] private TMP_Text budgetText;

    [Header("Enviroment")]
    [SerializeField] private TMP_Text currentEnvironmentText;

    [Header("Connection Indicator")] 
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
    }

    private void OnEnable()
    {
        inputMap = new InputMap();
    }

    void Start()
    {
        SetCursorTextureDefault();
        
        // Set Quality Level to Ultra
        SetQualityLevel(5);
    }

    void Update()
    {
        ChangeCursor();
        
        //TODO: Look at new selling hotkey
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
        if (currentEnvironmentText)
        {
            currentEnvironmentText.text = $"{LevelManager.Instance.GetCurrentEnvironmentalImpact():F2} CO2t";
        }
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void ToggleConnectionModeIndicator(bool? enable)
    {
        if (enable != null)
        {
            connectionActiveIndicatorImage.enabled = enable.Value;
            return;
        }
        connectionActiveIndicatorImage.enabled = !connectionActiveIndicatorImage.enabled;
    }


}
