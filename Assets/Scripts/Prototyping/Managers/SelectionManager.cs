using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionManager : MonoBehaviour
{
    private ISelectableEntity _currentlySelected;
    
    private Camera mainCamera;
    
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
    }

    private void Start()
    {
        mainCamera = UIManager.Instance.sceneCamera;
    }

    public void Select(ISelectableEntity newSelection)
    {
        if (newSelection.IsSelected())
        {
            newSelection.Deselect();
            return;
        }
        
        if (_currentlySelected != null)
        {
            _currentlySelected.Deselect();
            if (newSelection.GetID() == _currentlySelected.GetID()) 
            {
                Debug.Log("Identical!");
                ClearSelection();
                return;
            }
        }
        
        newSelection.Select();
        _currentlySelected = newSelection;
    }

    public void ClearSelection()
    {
        if (_currentlySelected != null)
        {
            //Debug.Log("Deselecting "+ _currentlySelected.GetID());
            _currentlySelected.Deselect();
            _currentlySelected = null;
        }
        
    }
}
