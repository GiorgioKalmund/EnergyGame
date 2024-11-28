using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionPanelObjectGameObject;
    private ISelectableEntity _currentlySelected = null;
    private SelectionPanel _selectionPanel;
    [SerializeField]
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
       _selectionPanel = selectionPanelObjectGameObject.GetComponent<SelectionPanel>();
       selectionPanelObjectGameObject.SetActive(false);
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
        newSelection.Select();
        _selectionPanel.SetPanel(newSelection);
        _currentlySelected = newSelection;
        selectionPanelObjectGameObject.SetActive(true);
        RectTransform rect = selectionPanelObjectGameObject.GetComponent<RectTransform>();
        if (!newSelection.IsOnLeftHalfOfTheScreen())
        {
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
        }

        rect.anchoredPosition = new Vector2(0f, -80f);
    }

    public void ClearSelection()
    {
        if (_currentlySelected != null)
        {
            // Debug.Log("Deselecting "+_currentlySelected.GetID());
            _currentlySelected.Deselect();
        }
        _currentlySelected = null;
        selectionPanelObjectGameObject.SetActive(false);
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
