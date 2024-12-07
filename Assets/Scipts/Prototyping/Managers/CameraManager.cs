using System;
using System.Data.Common;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class CameraManager : MonoBehaviour
{
   public static CameraManager Instance { get; private set; }
   
   [Header("Camera")]
   [SerializeField] private GameObject currentCamera;
   
   [Header("Targets")]
   public Transform targetTransform;
   private GameObject originalCameraPosition;
   private void Awake()
   {
      if (Instance && Instance != this)
      {
         Destroy(this);
      }
      else
      {
         Instance = this;
      }

      originalCameraPosition = new GameObject();
      originalCameraPosition.transform.position = currentCamera.transform.position;
      originalCameraPosition.transform.rotation = currentCamera.transform.rotation;
   }

   public void MoveCameraToTargetTransform()
   {
      currentCamera.transform.DOMove(targetTransform.position, 1f);
      currentCamera.transform.DORotateQuaternion(targetTransform.rotation, 1f);
   }

   public void ResetCameraTransform()
   {
      // Debug.Log("Resetting Camera to "+originalCameraPosition.position);
      currentCamera.transform.DOMove(originalCameraPosition.transform.position, 1f);
      currentCamera.transform.DORotateQuaternion(originalCameraPosition.transform.rotation, 1f);
   }
}