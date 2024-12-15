using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public int[,] Matrix { get; } = new int[100, 100];
    private Wandler[] wandlerArray = new Wandler[100];
    private int numOfWandler = 0;
    
    public void InsertNew(Wandler wandler){
        wandlerArray[numOfWandler++] = wandler;


    }
}
