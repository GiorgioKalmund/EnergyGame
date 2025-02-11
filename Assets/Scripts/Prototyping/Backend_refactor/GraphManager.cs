using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set;}
    public int[,] Matrix { get; } = new int[255, 255];
    public Wandler[] wandlerArray = new Wandler[255];
    public Wandler[] Endpoints = new Wandler[15];
    public int numOfEndpoints = 0;
    public int numOfWandler = 0;

    public void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void InsertNew(Wandler wandler){
        wandlerArray[numOfWandler++] = wandler;
        return;
    }

    public void ConnectWandler(Wandler from, Wandler to){
        Debug.Log("Connecting "+from.InstanceID+ " with "+to.InstanceID);
        Matrix[from.InstanceID, to.InstanceID] = 1;
        calculateAll();
        return;
    }

    public void DisconnectWandler(Wandler from, Wandler to){
        Debug.Log("Connecting "+from.InstanceID+ " with "+to.InstanceID);
        Matrix[from.InstanceID, to.InstanceID] = 0;
        calculateAll();
        return;
    }

    public void RemoveWandler(Wandler wandler){
        wandlerArray[wandler.InstanceID] = null;
        for(int i = 0; i < numOfWandler; i++){
            Matrix[wandler.InstanceID, i] = 0;
            Matrix[i, wandler.InstanceID] = 0;
        }
        
        Destroy(wandler.gameObject);
        calculateAll();
    }

    [ContextMenu("Print Adjacency Matrix")]
    public void printMatrix(){
        string temp = "";
        for(int i = 0; i < numOfWandler;i++){
            for(int j = 0;j<numOfWandler;j++){
                temp += Matrix[j,i];
            }
            Debug.Log(temp + "\n");
            temp = "";
        }
        return;
    }

    [ContextMenu("Calculate Energy Grid")]
    public void calculateAll(){
        //Debug.LogError("ALL DIRECTIONS WILL BE RECALCULATED");
        calcDistance();
        recalcDirection();
        //Debug.LogError("EVERYTHING IS BEING CALCULATED");
        float total = 0;
        for(int i  = 0; i < numOfEndpoints; i++){
            float output = Endpoints[i].getOutput();
            total += output;
        }
        
        // TODO: Split up into different endpoints and assign each of them their correct (new) value
        
        // TODO: 
    }

    public void calcDistance(){
        for(int i  = 0; i < numOfWandler; i++){
            if(wandlerArray[i]){
                wandlerArray[i].distance = -1;
            }
            
        }
        for(int i  = 0; i < numOfEndpoints; i++){
            Endpoints[i].distance = 0;
            Endpoints[i].calcDistance();
        }
    }

    public void recalcDirection(){
        setVisitedFalse();
        for(int i  = 0; i < numOfEndpoints; i++){
            Endpoints[i].recalcDirection();
        }
        setVisitedFalse();
    }

    private void setVisitedFalse(){
        for(int i  = 0; i < numOfWandler; i++){
            if(wandlerArray[i]){
                wandlerArray[i].visited = false;
            }
        }
    }
}
