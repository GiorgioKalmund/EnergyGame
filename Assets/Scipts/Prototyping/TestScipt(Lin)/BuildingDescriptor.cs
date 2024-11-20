using UnityEngine;
using System;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider))]
public class BuildingDesriptor : MonoBehaviour
{
    public String buildngName;
    [SerializeField]
    private PlacementType placement;

    [SerializeField] private float cost;
    [SerializeField] public float production; 
    [SerializeField] private bool placed = false;
    private BoxCollider _collider;
    

    public PlacementType Placement
    {
        get => placement;
        set => placement = value;
    }

    public void Awake()
    {
        if (buildngName == "")
        {
            buildngName = gameObject.name;
        }

        _collider = GetComponent<BoxCollider>();
        _collider.enabled = false;
    }

    public bool isPlaced()
    {
        return placed == true;
    }

    public void Place()
    {
        placed = true;
        _collider.enabled = true;
        Debug.Log($"{buildngName} added {production}");
        //LevelController.Instance.AddProduce(production);
    }
    public void Sell()
    {
        
    }



}

public enum PlacementType
{
    Default, Water
}

