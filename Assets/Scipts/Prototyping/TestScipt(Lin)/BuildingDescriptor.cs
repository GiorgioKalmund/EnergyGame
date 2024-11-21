using UnityEngine;
using System;
using UnityEngine.Serialization;
using Scipts.Prototyping.TestScipt_Lin_;

[RequireComponent(typeof(BoxCollider))]
public class BuildingDesriptor : MonoBehaviour
{
    public String buildngName;
    [SerializeField]
    private PlacementType placement;

    [SerializeField] private float cost;
    [SerializeField] public float production; 
    [SerializeField] private bool placed = false;
    [SerializeField] private bool selected = false;
    
    private BoxCollider _collider;
    [SerializeField] private GameObject selectionIndicator = null;

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

    public void Select()
    {
            Debug.Log("Selected "+ this.buildngName);
            selectionIndicator.SetActive(true);
            selected = true;
    }

    public void Deselect()
    {
        Debug.Log("Deselected "+ this.buildngName);
        selectionIndicator.SetActive(false);
        selected = false;
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

    public bool IsSelected()
    {
        return selected;
    }

}


