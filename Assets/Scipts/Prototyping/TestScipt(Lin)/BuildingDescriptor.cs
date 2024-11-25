using UnityEngine;
using System;
using UnityEngine.Serialization;
using Scipts.Prototyping.TestScipt_Lin_;
using Unity.XR.OpenVR;
using UnityEditor;

[RequireComponent(typeof(BoxCollider))]
public class BuildingDescriptor : MonoBehaviour
{
    public String buildingName;
    [SerializeField] private PlacementType placement;
    [SerializeField] private float cost;
    [SerializeField] private float production; 
    [SerializeField] private bool placed = false;
    [SerializeField] private bool selected = false;
    public int id;
    
    private BoxCollider _collider;
    [SerializeField] private GameObject selectionIndicator = null;
    [SerializeField] private Sprite imageSprite;
    public bool isOnLeftHalfOfScreen;
    
    public PlacementType Placement
    {
        get => placement;
        set => placement = value;
    }

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
        _collider = GetComponent<BoxCollider>();
        _collider.enabled = false;
        CheckForHalfPositionOnScreen();
    }

    public bool isPlaced()
    {
        return placed == true;
    }

    public void Place()
    {
        placed = true;
        _collider.enabled = true;
        CheckForHalfPositionOnScreen();
    }
    public void Sell()
    {
        
    }

    public void Select()
    {
            Debug.Log("Selected "+ this.buildingName);
            selectionIndicator.SetActive(true);
            selected = true;
    }

    public void Deselect()
    {
        Debug.Log("Deselected "+ this.buildingName);
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

    public float GetProduction()
    {
        return this.production;
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

    private void CheckForHalfPositionOnScreen()
    {
        float screenWidth = Screen.width;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.x >= screenWidth / 2)
        {
            isOnLeftHalfOfScreen = false;
        }
        else
        {
            isOnLeftHalfOfScreen = true;
        }
    }

    public PlacementType GetPlacementType()
    {
        return this.placement;
    }

}


