using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionPanelObjectGameObject;
    private BuildingDescriptor _currentlySelected = null;
    private SelectionPanel _selectionPanel;
    
    public static SelectionManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton
       if (Instance && Instance != this)
       {
           Destroy(this);
       }
       else
       {
           Instance = this;
       }
       _selectionPanel = selectionPanelObjectGameObject.GetComponent<SelectionPanel>();
       selectionPanelObjectGameObject.SetActive(false);
    }

    public void Select(BuildingDescriptor newSelection)
    {
        if (_currentlySelected)
        {
            _currentlySelected.Deselect();
            if (newSelection.GetID() == _currentlySelected.GetID()) 
            {
                ClearSelection();
                return;
            }
        }
        newSelection.Select();
        _selectionPanel.SetPanel(newSelection);
        _currentlySelected = newSelection;
        selectionPanelObjectGameObject.SetActive(true);
    }

    public void ClearSelection()
    {
        _currentlySelected = null;
        selectionPanelObjectGameObject.SetActive(false);
    }
}
