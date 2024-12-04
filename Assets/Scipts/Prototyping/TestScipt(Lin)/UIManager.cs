using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // [Header("Managers")]
    // [SerializeField] private OverlayManager _overlayManager;
    // [SerializeField] private SelectionManager _selectionManager;
    
    [Header("Cursor")]
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;
    
    [Header("Power")]
    [SerializeField] private TMP_Text demandText;
    [SerializeField] private TMP_Text currentProductionText;
    
    [Header("Budget")]
    [SerializeField] private TMP_Text budgetText;

    [Header("Enviroment")]
    [SerializeField] private TMP_Text Max_Enviroment;
    [SerializeField] private TMP_Text Current_Enviroment;
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

    void Start()
    {
        SetCursorTextureDefault();
        UpdateDemandText();
        UpdateMaxEnvironmentalImpact();
        Debug.LogWarning("Quality level:"+QualitySettings.GetQualityLevel());
    }

    void Update()
    {
        ChangeCursor();
        if ((Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Backspace)) && SelectionManager.Instance.PanelIsOpen())
        {
            SelectionManager.Instance.TriggerSellAction();
        }
    }

    public void RenderBudget()
    {
        budgetText.text = $"{BudgetManager.Instance.GetBudget()}€";
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
    public void UpdateDemandText()
    { 
        if (demandText) 
        { 
            demandText.text = $"Demand: {LevelController.Instance.GetCurrentDemand():F2} MW"; 
        }
    }
    public void UpdateCurrentProductionText()
    { 
        if (currentProductionText) 
        { 
            currentProductionText.text = $"Current Production: {LevelController.Instance.GetCurrentProduction():F2} MW"; 
        }
    }


    public void UpdateMaxEnvironmentalImpact()
    { 
        if (Max_Enviroment) 
        {
            Max_Enviroment.text = $"{LevelController.Instance.GetMaxEnvironmentalImpact():F2} CO2";
        }
    }

    
    public void UpdateCurrentEnvironmentalImpact()
    {
        if (Current_Enviroment)
        {
            Current_Enviroment.text = $"{LevelController.Instance.GetCurrentEnvironmentalImpact():F2} CO2";
        }
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }


}
