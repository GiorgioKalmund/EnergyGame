using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class ConsumerDescriptor : MonoBehaviour, ISelectableEntity
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

   [FormerlySerializedAs("tag")]
   [Header("Tag")] 
   [SerializeField] private TagSelectionTree tagTree;
   
   public void Start()
   {
       id = LevelManager.Instance.nextID;
       LevelManager.Instance.nextID += 1;
       
       // TODO: Probably not the best way, but suffices for prototyping
       RaycastHit hit;
       if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
       {
          // Debug.Log("HIT: "+hit.transform.gameObject.name);
          TileDataWrapper tileDataWrapper = hit.transform.gameObject.GetComponent<TileDataWrapper>();
          if (tileDataWrapper)
          {
             tileDataWrapper.tileData.SetCurrentBuilding(this);
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
  public void ToggleSelection()
  {
        if (selected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
  }
   public void Select()
   {
         selectionIndicator.SetActive(true);
         selected = true;
   }

   public void Deselect()
   {
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


   public void ToggleTag(int combination)
   {
      tagTree.ToggleTreeCombination(combination);
   }

   public void CloseTag()
   {
      tagTree.CollapseTree();
   }
   
}
