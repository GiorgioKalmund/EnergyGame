using System;
using UnityEngine;

public class LevelMapManager : MonoBehaviour
{
    public LevelMapManager Instane { get; private set; }
    
    private void Awake()
    {
        if (Instane && Instane != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instane = this;
        }
    }
}