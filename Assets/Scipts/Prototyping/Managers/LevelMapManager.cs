using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapManager : MonoBehaviour
{
    public static  LevelMapManager Instance { get; private set; }

    [SerializeField] private int unlockUntilLevel = 1;
    
    public List<LevelMapMarker> markers;
    /**
     *  IMPORTANT: This needs to be identical to the real amount of markers present
     *  If not set correctly, the behaviour might not be as expected!
     */
    public int maxMarkerCount;
    public LevelMapMarker CurrentlySelectedMarker { get; set; }

    [Header("Path")]
    [SerializeField] private GameObject pathGameObject;
    [SerializeField] private GameObject pathParent;

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
        if (!pathParent)
        {
            Debug.LogError("No parent for path elements in Map Manager");
        }
    }

    private void Start()
    {
        StartCoroutine(WaitForAllMarkers());
    }

    public void AddMarker(LevelMapMarker marker)
    {
       markers.Add(marker);
    }

    private void LinkMarkers()
    {
        if (markers.Count != maxMarkerCount)
        {
            Debug.LogWarning("Actual marker count does not correspond to the value entered. Please modify 'maxMarkerCount' to be the amount of markers visible on the field");
        }
        Debug.Log("Linking "+markers.Count + " markers.");
        markers.Sort((marker0, marker1) => marker0.markerID.CompareTo(marker1.markerID));
        for (int index = 0; index < markers.Count; index++)
        {
            // Calculate backward, meaning that the first marker doesn't have a prev 
            if (index != 0)
            {
                markers[index].SetPrev(markers[index-1]);
            }
            
            // Calculate forward, meaning that the last marker doesn't have a next
            if (index != markers.Count - 1)
            {
                markers[index].SetNext(markers[index+1]);
            }
        }
    }
    /*
     *  IMPORTANT: This method uses 1-based indexing for consistency with level names
     */
    public void UnlockUntilMarkerId(int markerId)
    {
        if (markerId < 0)
        {
            Debug.LogWarning(markerId+" is not a valid level Index");
            return;
        }
        markers[markerId].Unlock();
    }

    public GameObject GetPathObject()
    {
        return pathGameObject;
    }

    public GameObject GetPathParent()
    {
        return pathParent;
    }
    
    private IEnumerator WaitForAllMarkers()
    {
        yield return new WaitUntil(() => markers.Count >= maxMarkerCount);

        LinkMarkers();
        UnlockUntilMarkerId(unlockUntilLevel - 1);
    } 
    
}