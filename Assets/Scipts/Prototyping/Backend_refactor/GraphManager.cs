using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public int[,] Matrix { get; } = new int[100, 100];
    public Wandler[] wandlerArray = new Wandler[100];
    public int numOfWandler = 0;
    
    public void InsertNew(Wandler wandler){
        wandlerArray[numOfWandler++] = wandler;
    }

    public void ConnectWandler(Wandler from, Wandler to){
        Matrix[from.InstanceID, to.InstanceID] = 1;
    }

    public void RemoveWandler(Wandler wandler){
        wandlerArray[wandler.InstanceID] = null;
        for(int i = 0; i < numOfWandler; i++){
            Matrix[wandler.InstanceID, i] = 0;
            Matrix[i, wandler.InstanceID] = 0;
        }
    }

    public void printMatrix(){
        string temp = "";
        for(int i = 0; i < numOfWandler;i++){
            for(int j = 0;j<numOfWandler;j++){
                temp += Matrix[j,i];
            }
            Debug.Log(temp + "\n");
            temp = "";
        }
    }
}
