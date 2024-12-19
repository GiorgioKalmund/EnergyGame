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

    public void ComputeInput(){
        float inputAmt = 0f;
        for(int i = 0; i<graphManager.Matrix.Length;i++){
            if(graphManager.Matrix[i,InstanceID] == 1){
                inputAmt += graphManager.wandlerArray[i].getOutput();
            }


        }
        input.Amount = inputAmt;

    }
    public float getOutput(){
        ComputeInput();
        int numOfChildren = 0;
        for(int i = 0; i<graphManager.Matrix.Length;i++){
            if(graphManager.Matrix[InstanceID,i] == 1){
                numOfChildren++;
            }
        }
        return input.Amount * efficiency / numOfChildren;
    }
}
