using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapManager : MonoBehaviour
{
    public static  LevelMapManager Instance { get; private set; }

    public List<LevelMapMarker> markers;
    public int maxMarkerCount;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        markers = new List<LevelMapMarker>();
    }

    private void Start()
    {
        // TODO: Implement saving for current progression state
        while (markers.Count != maxMarkerCount)
        {
            
        }
        LinkMarkers();
    }

    public void AddMarker(LevelMapMarker marker)
    {
       markers.Add(marker);
    }

    private void LinkMarkers()
    {
        Debug.Log("Linking "+markers.Count + " markers.");
        markers.Sort((marker0, marker1) => marker0.id.CompareTo(marker1.id));
        for (int index = 0; index < markers.Count; index++)
        {
            if (index != 0)
            {
                Debug.Log("Set prev for "+index);
                markers[index].SetPrev(markers[index-1]);
            }

            if (index != markers.Count - 1)
            {
                Debug.Log("Set next for "+index);
                markers[index].SetNext(markers[index+1]);
            }
        }
    }
}