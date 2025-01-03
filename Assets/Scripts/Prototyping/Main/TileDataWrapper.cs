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
            ISelectableEntity descriptor = tileData.currentBuilding;
            if (descriptor as ProducerDescriptor)
            { 
                ProducerDescriptor producer = (ProducerDescriptor)descriptor;
                 
                if (producer)
                    producer.Sell();
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
