using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Wandler : MonoBehaviour
{
    [Header("Wandler")]
    public GraphManager graphManager;
    public int InstanceID;
    [SerializeField]
    public float generating;
    [SerializeField]
    private EnergyChunk input;
    [SerializeField]
    public EnergyChunk output;
    [SerializeField]
    public float efficiency;
    public Wandler onStartConnectTo;

    [Header("Tag Tree")]
    public TagSelectionTree tagTree;

    [Header("Endpoint")]
    [SerializeField]
    private bool Endpoint;
    private bool EndpointCompleted;
    [SerializeField] TagSelectionTree endpointTree = null;
    public int endpointDemand;
    public int distance = -1;
    public bool visited = false;
    public int overflowDistance = 10000000;

    // Start is called before the first frame update
    void Start()
    {
        graphManager = GraphManager.Instance;
        InstanceID = graphManager.numOfWandler;
        graphManager.InsertNew(this);
        if(Endpoint){
            graphManager.Endpoints[graphManager.numOfEndpoints++] = this;
        }

        input = new EnergyChunk();
        output = new EnergyChunk();


        if(onStartConnectTo != null){
            addInputWandler(onStartConnectTo);
        }
        
        if (Endpoint)
            UpdateEndpointText(0);
        
        //graphManager.printMatrix();
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

    public void ComputeInput(){
        float inputAmt = generating;
        //Debug.Log("Now calculating Input for: "+InstanceID);
        for(int i = 0; i<graphManager.Matrix.GetLength(0);i++){
            if(graphManager.Matrix[i,InstanceID] == 1){
                inputAmt += graphManager.wandlerArray[i].getOutput();
            }
        }
        input.Amount = inputAmt;
        //Debug.Log(InstanceID + " has an Input of: " + input.Amount);
        
        return;
    }
    public float getOutput(){
        ComputeInput();
        //Debug.Log("Now Calculating Output for: "+ InstanceID);
        int numOfChildren = 0;
        for(int i = 0; i<graphManager.Matrix.GetLength(0);i++){
            if(graphManager.Matrix[InstanceID,i] == 1){
                numOfChildren++;
            }
        }
        output.Amount = input.Amount * efficiency;
        //Debug.Log(InstanceID + " has an Output of:" + output.Amount);
        float result;
        if (Endpoint)
        {
            //Output overflow
            result = Math.Max(0, input.Amount* efficiency - endpointDemand) / (numOfChildren == 0 ? 1 : numOfChildren);
        }
        else
        {
            result = input.Amount * efficiency / (numOfChildren == 0 ? 1 : numOfChildren);
        }
        UpdateEndpointText(input.Amount *efficiency);
        if(tagTree&&generating==0)
            tagTree.SetProductionText(result);
        return result;
    }

    public void calcDistance(){
            for(int i = 0; i<graphManager.numOfWandler;i++){
                if(graphManager.Matrix[InstanceID,i] == 1 || graphManager.Matrix[i,InstanceID] == 1){
                    if(graphManager.wandlerArray[i].distance == -1 || graphManager.wandlerArray[i].distance > distance+1){
                        if(graphManager.wandlerArray[i].Endpoint == false){
                            graphManager.wandlerArray[i].distance = distance+1;
                        }
                        else{
                            graphManager.wandlerArray[i].distance = 0;
                        }
                        if(graphManager.Matrix[i,InstanceID] == 1 && graphManager.wandlerArray[i].Endpoint){
                            return;
                        }
                        graphManager.wandlerArray[i].calcDistance();
                    }
                }
            }
            //if(tagTree)
                //tagTree.SetProductionText(distance);
    }

    public void recalcDirection(){
        visited = true;
            for(int i = 0; i<graphManager.numOfWandler;i++){
                if(graphManager.Matrix[InstanceID,i] == 1 || graphManager.Matrix[i,InstanceID] == 1){
                    if(graphManager.wandlerArray[i].visited == false && graphManager.wandlerArray[i].Endpoint == false && Endpoint == false){
                        if(graphManager.wandlerArray[i].distance > distance){
                            graphManager.Matrix[InstanceID, i] = 0;
                            graphManager.Matrix[i, InstanceID] = 1;
                            graphManager.wandlerArray[i].recalcDirection();
                        }
                        else if(graphManager.wandlerArray[i].distance < distance && graphManager.wandlerArray[i].Endpoint == false && Endpoint == false){
                            graphManager.Matrix[InstanceID, i] = 1;
                            graphManager.Matrix[i, InstanceID] = 0;
                            graphManager.wandlerArray[i].recalcDirection();
                        }
                    }
                }
            }
    }

    public void UpdateEndpointText(float value)
    {
       if (!Endpoint){
           
       }
       else{
       
       endpointTree.SetEndpointProductionText(value, endpointDemand);
        
       if (value > endpointDemand && !EndpointCompleted)
       {
           LevelManager.Instance.CompleteEndpoint();
           
           EndpointCompleted = true;
       }
       if(EndpointCompleted && value < endpointDemand){
            LevelManager.Instance.UncompleteEndpoint();
            EndpointCompleted = false;
       }
       
       // TODO: Update texts of all connected cables
       }
    }
}
