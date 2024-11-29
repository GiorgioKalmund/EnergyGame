using UnityEngine;
using System;
using UnityEngine.Serialization;
using Scipts.Prototyping.TestScipt_Lin_;
using Unity.XR.OpenVR;
using UnityEditor;
using UnityEngine.WSA;

[RequireComponent(typeof(BoxCollider))]
public class ProducerDescriptor : MonoBehaviour, ISelectableEntity
{
    [Header("Info")] 
    public String buildingName;
    [SerializeField] private PlacementType placement;
    [SerializeField] private float cost;
    public float maxProduction;
    [SerializeField] private float currentProduction;
    public float environmentalImpact;
    public bool placed = false;
    public bool selected = false;
    public int id;
    
    [SerializeField] private GameObject selectionIndicator = null;
    [SerializeField] private Sprite imageSprite;
    public bool isOnLeftHalfOfScreen;
    public TileData tileOn;
    
    public void Start()
    {
        id = LevelController.Instance.nextID;
        LevelController.Instance.nextID += 1;
    }

    public void Awake()
    {
        if (buildingName == "")
        {
            buildingName = gameObject.name;
        }

        isOnLeftHalfOfScreen = IsOnLeftHalfOfTheScreen();
    }

    public bool isPlaced()
    {
        return placed == true;
    }

    public bool Place(TileData tile)
    {
        // if we do not have enough budget, cancel placement
        if (!BudgetManager.Instance.UseBudget(cost))
        {
            return false;
        }
        placed = true;
        SetTile(tile);
        LevelController.Instance.AddEnvironmentalImpact(environmentalImpact);
        isOnLeftHalfOfScreen = IsOnLeftHalfOfTheScreen();
        return true;
    }
    public void Sell()
    {
       BudgetManager.Instance.Sell(cost);
       tileOn.Reset();
       LevelController.Instance.ReduceProduce(currentProduction);
       Destroy();
       PlacementSystem.Instance.HideCable();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void SetTile(TileData tile)
    {
        this.tileOn = tile;
    }

    public void Select()
    {
        //Debug.Log("Selected "+ this.buildingName);
        selectionIndicator.SetActive(true);
        selected = true;
    }

    public void Deselect()
    {
        //Debug.Log("Deselected "+ this.buildingName);
        selectionIndicator.SetActive(false);
        selected = false;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public float GetMaxProduction()
    {
        return this.maxProduction;
    }
    
    public float GetCurrentProduction()
    {
        return this.currentProduction;
    }

    public void SetProduction(float newProductionValue)
    {
        this.currentProduction = newProductionValue;
    }
    public float GetCost()
    {
        return this.cost;
    }

    public Sprite GetSprite()
    {
        return imageSprite;
    }

    public int GetID()
    {
        return id;
    }

    public bool IsOnLeftHalfOfTheScreen()
    {
        float screenWidth = Screen.width;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return (screenPosition.x >= screenWidth / 2);
    }

    public PlacementType GetPlacementType()
    {
        return this.placement;
    }

    public string GetName()
    {
        return buildingName;
    }

}


