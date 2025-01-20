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

   public bool placed = true;
   
   public Vector3 startPos;
   public Vector3 endPos;

   public TagSelectionTree tagTree;

   [Header("Destructor")] 
   [SerializeField] private GameObject destructorButton;

   private void Awake()
   {
       /* startPos = gameObject.transform.position;
       startPos.y += 0.3f;
       endPos = PlacementManager.Instance.GetCellIndicatorTransform().position;
       endPos.y += 0.3f; */
   }

   private void Start()
   {
       _lineRenderer = GetComponent<LineRenderer>();
       _lineRenderer.positionCount = lineVertexCount;
       lineFunctionDivisor = 0.9f * lineVertexCount;
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
       
       // Place it 50% of the way towards the end position
       Vector3 newPos = startPos + (endPos - startPos) * 0.5f;
       newPos.y += 0.5f;
       tagTree.transform.parent.gameObject.transform.position = newPos;
       
       GraphManager.Instance.calculateAll();
   }

   public void DrawCable()
   {
       if (!placed)
       {
           endPos = PlacementManager.Instance.GetCellIndicatorTransform().position;
           endPos.y = PlacementManager.Instance.cellIndicatorPlacementY + ConnectCableMode.Instance.cableYOffset;
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

           Vector3 dir = UIManager.Instance.sceneCamera.transform.position - pointToDraw;
           if (index == lineVertexCount / 2)
               destructorButton.transform.position = pointToDraw + Vector3.Normalize(dir) * 0.3f;
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
