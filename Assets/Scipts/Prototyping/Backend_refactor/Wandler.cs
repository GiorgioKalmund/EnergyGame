using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandler : MonoBehaviour
{
    public GraphManager graphManager;
    public int InstanceID;
    [SerializeField]
    private EnergyChunk input;
    [SerializeField]
    public EnergyChunk output;
    [SerializeField]
    private float efficiency;

    public Wandler onStartConnectTo;
    
    // Start is called before the first frame update
    void Start()
    {
        graphManager = GraphManager.Instance;
        InstanceID = graphManager.numOfWandler;
        graphManager.InsertNew(this);

        if(onStartConnectTo != null){
            addInputWandler(onStartConnectTo);
        }
        graphManager.printMatrix();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addInputWandler(Wandler wandler){
        graphManager.ConnectWandler(wandler, this);
    }

    public void addOutputWandler(Wandler wandler){
        graphManager.ConnectWandler(this, wandler);
    }
}
