using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Wandler : MonoBehaviour
{
    public GraphManager graphManager;
    public int InstanceID;
    [SerializeField]
    private int generating;
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

        input = new EnergyChunk();
        output = new EnergyChunk();

        if(onStartConnectTo != null){
            addInputWandler(onStartConnectTo);
        }
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
        return input.Amount * efficiency / (numOfChildren == 0 ? 1 : numOfChildren);
    }
}
