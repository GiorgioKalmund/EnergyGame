using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class BuildingDesriptor : MonoBehaviour
{
    public String name;
    [SerializeField]
    private PlacementType placement;

    [SerializeField] private float cost;
    [SerializeField] private bool placed = false;
    private BoxCollider _collider;
    

    public PlacementType Placement
    {
        get => placement;
        set => placement = value;
    }

    public void Awake()
    {
        if (name == "")
        {
            name = gameObject.name;
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

    }

    public void Sell()
    {
        
    }
}

public enum PlacementType
{
    Default, Water
}

