using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandler : MonoBehaviour
{
    private GraphManager graphManager;
    public int InstanceID;
    [SerializeField]
    private EnergyChunk input;
    [SerializeField]
    public EnergyChunk output;
    [SerializeField]
    private float efficiency;
    
    // Start is called before the first frame update
    void Start()
    {
        graphManager = FindObjectOfType<GraphManager>();
        InstanceID = graphManager.numOfWandler;
        graphManager.InsertNew(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
