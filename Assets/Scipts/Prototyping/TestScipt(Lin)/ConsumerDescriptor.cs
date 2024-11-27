using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scipts.Prototyping.TestScipt_Lin_;

public class ConsumerDescriptor : MonoBehaviour, SelectableEntity
{
   [Header("Info")] 
   public String buildingName;
   public float consumed;
   [SerializeField] private float demand;
   public bool completed;
   [SerializeField] private bool selected = false;
   public Vector2 coords;
   public int id;
   public bool isOnLeftHalfOfScreen;
   
   [Header("Extra")]
   private BoxCollider _collider;
   [SerializeField] private GameObject selectionIndicator = null;
   [SerializeField] private Sprite imageSprite;
   
   public void Start()
   {
       id = LevelController.Instance.nextID;
       LevelController.Instance.nextID += 1;
       
       // TODO: Probably not the best way, but suffices for prototyping
       RaycastHit hit;
       if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
       {
          // Debug.Log("HIT: "+hit.transform.gameObject.name);
          TileDataWrapper tileDataWrapper = hit.transform.gameObject.GetComponent<TileDataWrapper>();
          if (tileDataWrapper)
          {
             tileDataWrapper.tileData.setCurrentBuilding(this);
             coords = tileDataWrapper.tileData.coords;
          }
          else
          {
             Debug.Log("Hit object isn't a tile!");
          }
       }
       else
       {
          Debug.LogError("Consumer Descriptor Initialization Raycast missed!");
       }
   }

   public void Select()
   {
         Debug.Log("Selected "+ this.buildingName);
         selectionIndicator.SetActive(true);
         selected = true;
   }

   public void Deselect()
   {
        Debug.Log("Deselected "+ this.buildingName);
        selectionIndicator.SetActive(false);
        selected = false;
   }

   public bool IsSelected()
   {
      return selected;
   }

   public Sprite GetSprite()
   {
      return imageSprite;
   }

   public string GetName()
   {
      return buildingName;
   }

   public int GetID()
   {
      return id;
   }

   public float GetDemand()
   {
      return demand;
   }
   
   public float GetConsumed()
   {
      return consumed;
   }
   
   public bool IsCompleted()
   {
      completed = consumed >= demand;
      return completed;
   }
   
   public bool IsOnLeftHalfOfTheScreen()
   {
      float screenWidth = Screen.width;
      Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
      return (screenPosition.x >= screenWidth / 2);
   } 
   
   
}
