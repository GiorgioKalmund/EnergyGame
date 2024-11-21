using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private Image imageElement;
    [SerializeField] private TMP_Text displayNameElement;
    [SerializeField] private TMP_Text infoTextElement;
    [SerializeField] private TMP_Text productionTextElement;
    [SerializeField] private TMP_Text costTextElement;
    [Header("Building")] 
    [SerializeField] private BuildingDescriptor currentBuilding;
    
    public void SetPanel(BuildingDescriptor buildingDescriptor)
    {
       imageElement.sprite = buildingDescriptor.GetSprite();
       displayNameElement.text = buildingDescriptor.buildingName;
       infoTextElement.text = "Some random info text might appear here.";
       productionTextElement.text = $"Production {buildingDescriptor.GetProduction()} MW";
       costTextElement.text = $"Cost {buildingDescriptor.GetCost()} €";
       currentBuilding = buildingDescriptor;
    }

    public void Close()
    {
       SelectionManager.Instance.ClearSelection(); 
    }
}
