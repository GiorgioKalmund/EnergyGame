using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public Transform cableAnchor;

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
        
        
        //maxProduction = GetComponent<Wandler>().generating;
        if(tagTree) tagTree.Setup(this);
        
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
        UpdateProductionTag();
    }
    public void Sell()
    {
        BudgetManager.Instance.Sell(cost);
        tileOn.Reset();

        LevelManager.Instance.ReduceEnvironmentalImpact(environmentalImpact);

        GraphManager g = GraphManager.Instance;
        Wandler w = GetComponentInChildren<Wandler>();
        List<Wandler> neighbors = new();
        for (int i = 0; i < g.numOfWandler; i++)
        {
            if(g.Matrix[w.InstanceID, i] != 0 || g.Matrix[i, w.InstanceID] != 0){
                neighbors.Add(g.wandlerArray[i]);
                
            }
        }

        for(int j = 0; j < neighbors.Count; j++){
            Wandler neighbor = neighbors.ElementAt(j);
            //Destroy only surrounding power cables not power plants 
            if(neighbor.gameObject.GetComponentInChildren<PowerCable>()){
                g.RemoveWandler(neighbor);
                // Destroy(neighbor.gameObject);
            }
        }

        GraphManager.Instance.RemoveWandler(GetComponent<Wandler>());
        
        if (UIManager.Instance.Mode == UIState.DESTROYING)
            UIManager.Instance.ToggleDestructionMode();

        if (BuilderInventory.Instance)
            BuilderInventory.Instance.AddSlotCapacity(1, instanceId);

        //Debug.Log(buildingName +  " " + placement + " got destroyed, " + " on " + tileOn.coords + " " + tileOn.currentPlacementType);
        tileOn.currentPlacementType = this.placement;
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
            tagTree.ExpandTreeSilently();
        }
        selected = true;
    }

    public void Deselect()
    {
        if (!selected)
            return;

        if (tagTree)
        {
            tagTree.CollapseTreeSilently();
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

    public void CloseTag()
    {
        tagTree.CollapseTree();
    }

    public void UpdateProductionTag()
    {
        return;
    }


}


