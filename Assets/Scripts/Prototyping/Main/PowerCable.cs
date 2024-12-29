using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PowerCable : MonoBehaviour
{
   private LineRenderer _lineRenderer;
   [SerializeField] private int lineVertexCount = 10;
   [SerializeField] private float lineFunctionDivisor = 6;

   public bool placed = false;
   
   private Vector3 startPos;
   private Vector3 endPos;

   // reference for endpoint?

   private void Awake()
   {
       startPos = gameObject.transform.position;
       startPos.y += 0.3f;
       endPos = PlacementManager.Instance.GetCellIndicatorTransform().position;
       endPos.y += 0.3f;
   }

   private void Start()
   {
       _lineRenderer = GetComponent<LineRenderer>();
       _lineRenderer.positionCount = lineVertexCount;
       lineFunctionDivisor = 0.64f * lineVertexCount;
       PowerCableManager.Instance.AddCable(this);
   }

   void Update() {
       if (!placed)
       {
           DrawCable();
       }
   }

   public void Place()
   {
       placed = true;
       GraphManager.Instance.calculateAll();
   }

   void DrawCable()
   {
       if (!placed)
       {
           endPos = PlacementManager.Instance.GetCellIndicatorTransform().position;
           endPos.y += 0.3f;
       }
       
       Vector3 direction = endPos - startPos;
       _lineRenderer.SetPosition(0, startPos);
       _lineRenderer.SetPosition(lineVertexCount - 1 , endPos);
       int half = lineVertexCount / 2;
       float subtraction = ApplyCableFunction(half / lineFunctionDivisor);
       for (int index = 1; index < lineVertexCount - 1; index++)
       {
           Vector3 pointToDraw = startPos + (index * direction / lineVertexCount);
           pointToDraw.y += ApplyCableFunction((index - half) / lineFunctionDivisor) - subtraction;
           _lineRenderer.SetPosition(index, pointToDraw);
       }
   }
   
   float ApplyCableFunction(float x)
   {
       return Mathf.Sqrt(Mathf.Pow(x, 2f) + 1);
   }

   public Vector3 GetStartPosition()
   {
       return startPos;
   }

   public bool IsEqualTo(Vector3 position)
   {
       float xDelta = Mathf.Abs(startPos.x - position.x);
       float zDelta = Mathf.Abs(startPos.z - position.z);

       float precision = 0.01f;
       
       return xDelta <= precision && zDelta <= precision;
   }

   public void Sell()
   {
       Destroy(gameObject);
   }

   
}
