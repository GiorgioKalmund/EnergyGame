using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionManager : MonoBehaviour
{
    private ISelectableEntity _currentlySelected = null;
    
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
        if (_currentlySelected != null)
        {
            _currentlySelected.Deselect();
            if (newSelection.GetID() == _currentlySelected.GetID()) 
            {
                ClearSelection();
                return;
            }
        }
        
        // TODO: Adapt to new TagSelectionTree
        
    }

    public void ClearSelection()
    {
        if (_currentlySelected != null)
        {
            // Debug.Log("Deselecting "+_currentlySelected.GetID());
            _currentlySelected.Deselect();
        }
        _currentlySelected = null;
        // TODO: Adapt to new TagSelectionTree
    }
    
      public void CheckForSelection()
        {
            if (InputManager.IsPointOverUI())
            {
                return;
            }
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
            {
                // Debug.Log("Found selectable Object");
                TileData selectedTileData = hit.transform.gameObject.GetComponent<TileDataWrapper>().tileData;
                ISelectableEntity entity = selectedTileData.currentBuilding;
                // Debug.Log("Clicked on tile: "+selectedTileData.coords);
                if (entity != null && !entity.IsSelected())
                {
                    // Debug.Log("Selected "+entity.GetName() + " on "+selectedTileData.coords);
                    Select(entity);
                }
                else
                {
                    // Debug.Log("Cleared Selection");
                    ClearSelection();
                }
            }
        }
}
