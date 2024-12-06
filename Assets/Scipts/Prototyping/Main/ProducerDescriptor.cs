using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Unity.XR.OpenVR;
using UnityEditor;
//using UnityEngine.WSA;

[RequireComponent(typeof(BoxCollider))]
public class ProducerDescriptor : MonoBehaviour, ISelectableEntity
{
    [Header("Info")] 
    public String buildingName;
    [SerializeField] private PlacementType placement;
    [SerializeField] private float cost;
    public float maxProduction;
    [SerializeField] private float currentProduction;
    [SerializeField] public float environmentalImpact;
    public bool placed = false;
    public bool selected = false;
    [Header("IDs")]
    [Tooltip("Global ID, unique across all instances of this type.")]
    public int id;
    [Tooltip("ID it is associated with inside the database")]
    public int instanceId;
    
    [Header("UI & UX")]
    [SerializeField] private GameObject selectionIndicator = null;
    [SerializeField] private Sprite imageSprite;
    public bool isOnLeftHalfOfScreen;
    
    [Header("Tile")]
    public TileData tileOn;

    public List<PowerCable> connectedCables;
    
    
    

    public void Awake()
    {
        if (buildingName == "")
        {
            buildingName = gameObject.name;
        }

        isOnLeftHalfOfScreen = IsOnLeftHalfOfTheScreen();
    }
    
    public void Start()
    {
        id = LevelManager.Instance.nextID;
        LevelManager.Instance.nextID += 1;
        connectedCables = new List<PowerCable>();
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
        LevelManager.Instance.AddEnvironmentalImpact(environmentalImpact);
        isOnLeftHalfOfScreen = IsOnLeftHalfOfTheScreen();
        return true;
    }
    public void Sell()
    {
       BudgetManager.Instance.Sell(cost);
       tileOn.Reset();
       LevelManager.Instance.ReduceProduce(currentProduction);
       LevelManager.Instance.ReduceEnvironmentalImpact(environmentalImpact);
       InventoryManager.Instance.UpdateInventorySlots();
       foreach (PowerCable cable in connectedCables)
       {
           cable.Sell();
       }
       Destroy();
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

    public int GetDBInstanceID()
    {
        return instanceId;
    }

    public float GetEnvironmentalImpact()
    {
        return environmentalImpact;
    }

    public void SetDBInstanceID(int dbInstanceId)
    {
        this.instanceId = dbInstanceId;
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

    public void AddCable(PowerCable newCable)
    {
        connectedCables.Add(newCable);
    }

}


