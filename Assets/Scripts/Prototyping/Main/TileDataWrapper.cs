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
         
         if (SelectionManager.Instance && tileData.currentBuilding != null)
             SelectionManager.Instance.Select(tileData.currentBuilding);
    }

    private void OnMouseUp()
    {
         if (InputManager.IsPointOverUI())
            return;
         
         if (SelectionManager.Instance && tileData.currentBuilding == null)
             SelectionManager.Instance.ClearSelection();
    }
}
