using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class TileDataWrapper : MonoBehaviour
{
    public TileData tileData;

    private void OnMouseDown()
    {
        if (InputManager.IsPointOverUI())
            return;
        
        if (UIManager.Instance.Mode == UIState.DEFAULT)
        {
            if (SelectionManager.Instance && tileData.currentBuilding != null)
                SelectionManager.Instance.Select(tileData.currentBuilding);
            
        } else if (UIManager.Instance.Mode == UIState.DESTROYING)
        {
            /* ISelectableEntity descriptor = tileData.currentBuilding;
            if (descriptor as ProducerDescriptor)
            { 
                ProducerDescriptor producer = (ProducerDescriptor)descriptor;
                 
                if (producer)
                    producer.Sell();
            } */

            Grid grid = PlacementManager.Instance.Grid;
            Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
            Vector3Int gridPosition = grid.WorldToCell(mousePos + new Vector3(0.5f, 0, 0.5f));
            Vector3Int arrPosition = GridDataManager.ConvertGridPosToArrayPos(gridPosition);
            arrPosition.z = 1;
            GameObject toDelete;
            if(toDelete = GridDataManager.GetGridDataAtPos(arrPosition)){
                if(toDelete.CompareTag("Endpoint")){
                    return;
                }
                toDelete.GetComponent<ProducerDescriptor>().Sell();
            }

        }
    }

    private void OnMouseUp()
    {
         if (InputManager.IsPointOverUI())
            return;
         
         if (SelectionManager.Instance && tileData.currentBuilding == null)
             SelectionManager.Instance.ClearSelection();
    }
}
