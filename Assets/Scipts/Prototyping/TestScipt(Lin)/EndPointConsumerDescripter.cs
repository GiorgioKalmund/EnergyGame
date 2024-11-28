using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scipts.Prototyping.TestScipt_Lin_;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class EndPointConsumerDescripter : MonoBehaviour, ISelectableEntity
{
    [Header("Info")]
    public String buildingName;
    public float consumed;
    [SerializeField] private float demand;
    public bool completed;
    [SerializeField] private bool selected = false;
    public Vector2 coords;
    public int id;
    public bool isOnLeftHalfOfScreen;

    [SerializeField] public List<ConsumerDescriptor> consumer = new List<ConsumerDescriptor>();

    [Header("Extra")]
    private BoxCollider _collider;
    [SerializeField] private GameObject selectionIndicator = null;
    [SerializeField] private Sprite imageSprite;

    public void Start()
    {
        id = LevelController.Instance.nextID;
        LevelController.Instance.nextID += 1;

        // TODO: Probably not the best way, but suffices for prototyping
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
        {
            // Debug.Log("HIT: "+hit.transform.gameObject.name);
            TileDataWrapper tileDataWrapper = hit.transform.gameObject.GetComponent<TileDataWrapper>();
            if (tileDataWrapper)
            {
                tileDataWrapper.tileData.setCurrentBuilding(this);
                coords = tileDataWrapper.tileData.coords;
            }
            else
            {
                Debug.Log("Hit object isn't a tile!");
            }
        }
        else
        {
            Debug.LogError("Consumer Descriptor Initialization Raycast missed!");
        }
        InitializeConsumers();

    }
    //Schritt 1: schauen nur nach nearBy objecte mit ConsumerDescriptor
    //Schritt 2: Für jedes nearBy in der list mache wir das geliche nochmal
    private void InitializeConsumers()
    {
        // mark object damit nicht zwei mal gezählt wird
        HashSet<GameObject> visited = new HashSet<GameObject>(); 
        Queue<GameObject> toProcess = new Queue<GameObject>();

        // Start with the current gameObject's nearby objects
        FindNearbyConsumers(transform.position, toProcess, visited);

        while (toProcess.Count > 0)
        {
            GameObject current = toProcess.Dequeue();
            ConsumerDescriptor descriptor = current.GetComponent<ConsumerDescriptor>();
            
            if (descriptor != null && !consumer.Contains(descriptor))
            {
                consumer.Add(descriptor);
                // Look for nearby objects of the current object
                FindNearbyConsumers(current.transform.position, toProcess, visited);
            }
        }
    }

    private void FindNearbyConsumers(Vector3 position, Queue<GameObject> toProcess, HashSet<GameObject> visited)
    {
        //1.00005 units dis
        Collider[] nearbyColliders = Physics.OverlapSphere(position, 1.5f); 

        foreach (Collider collider in nearbyColliders)
        {
            GameObject obj = collider.gameObject;

            if (!visited.Contains(obj) && obj != this.gameObject) // Avoid adding the current object
            {
                visited.Add(obj); // Mark as visited
                toProcess.Enqueue(obj); 
            }
        }
    }



    public void Select()
    {
        Debug.Log("Selected " + this.buildingName);
        selectionIndicator.SetActive(true);
        selected = true;
    }

    public void Deselect()
    {
        Debug.Log("Deselected " + this.buildingName);
        selectionIndicator.SetActive(false);
        selected = false;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public Sprite GetSprite()
    {
        return imageSprite;
    }

    public string GetName()
    {
        return buildingName;
    }

    public int GetID()
    {
        return id;
    }

    public float GetDemand()
    {
        return demand;
    }

    public float GetConsumed()
    {
        return consumed;
    }

    public bool IsCompleted()
    {
        completed = consumed >= demand;
        return completed;
    }

    public bool IsOnLeftHalfOfTheScreen()
    {
        float screenWidth = Screen.width;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return (screenPosition.x >= screenWidth / 2);
    }


}