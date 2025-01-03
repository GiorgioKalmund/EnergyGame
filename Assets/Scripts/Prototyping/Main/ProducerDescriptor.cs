using UnityEngine;
using System.Collections.Generic;

public class ProducerDescriptor : MonoBehaviour, ISelectableEntity
{
    [Header("Info")] 
    public string buildingName;
    public PowerPlantType powerPlantType;
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
    [SerializeField] private Sprite imageSprite;
    
    [Header("Tile")]
    public TileData tileOn;

    public List<PowerCable> connectedCables;
    
    [Header("Tag")] 
    [SerializeField] private TagSelectionTree tagTree;
    
    public void Awake()
    {
        if (buildingName == "")
        {
            buildingName = gameObject.name;
        }

    }
    
    public void Start()
    {
        id = LevelManager.Instance.nextID;
        LevelManager.Instance.nextID += 1;
        connectedCables = new List<PowerCable>();
         
        tagTree.Setup(this);
    }

    public bool isPlaced()
    {
        return placed == true;
    }

    public void Place(TileData tile)
    {
        // if we do not have enough budget, cancel placement
        BudgetManager.Instance.UseBudget(cost);
        placed = true;
        SetTile(tile);
        LevelManager.Instance.AddEnvironmentalImpact(environmentalImpact);
    }
    public void Sell()
    {
       BudgetManager.Instance.Sell(cost);
       tileOn.Reset();
       LevelManager.Instance.ReduceProduce(currentProduction);
       LevelManager.Instance.ReduceEnvironmentalImpact(environmentalImpact);
       foreach (PowerCable cable in connectedCables)
       {
           cable.Sell();
       }
       
       if (UIManager.Instance.Mode == UIState.DESTROYING) 
           UIManager.Instance.ToggleDestructionMode();
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

    public void ToggleSelection()
    {
        if (selected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    public void Select()
    {
        if (!placed)
            return;
        
        UpdateProductionTag();
        if (tagTree)
        {
            tagTree.ExpandTree(new List<TreeTagType>() { TreeTagType.POWER , TreeTagType.CO2, TreeTagType.FINANCE});
        }
        selected = true;
    }

    public void Deselect()
    {
        if (!selected)
            return;
        
        if (tagTree)
        {
            tagTree.CollapseTree();
        }
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
        currentProduction = newProductionValue;
        UpdateProductionTag();
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
   public void ToggleTag(int combination)
    {
        tagTree.ToggleTreeCombination(combination);
    }

    public void CloseTag()
    {
        tagTree.CollapseTree();
    }

    public void UpdateProductionTag()
    {
        tagTree.SetProductionText(currentProduction);
    }

  
}


