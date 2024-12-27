using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public static GraphManager Instance { get; private set;}
    public int[,] Matrix { get; } = new int[100, 100];
    public Wandler[] wandlerArray = new Wandler[255];
    public Wandler[] Endpoints = new Wandler[100];
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
        return;
    }

    public void RemoveWandler(Wandler wandler){
        wandlerArray[wandler.InstanceID] = null;
        for(int i = 0; i < numOfWandler; i++){
            Matrix[wandler.InstanceID, i] = 0;
            Matrix[i, wandler.InstanceID] = 0;
        }
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
        for(int i  = 0; i < numOfEndpoints; i++){
            Endpoints[i].ComputeInput();
            Debug.Log(Endpoints[i] + " - " + i + " : " + Endpoints[i].getOutput());
        }
    }
}
