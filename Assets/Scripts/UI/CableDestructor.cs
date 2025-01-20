using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

public class CableDestructor : MonoBehaviour
{
    [SerializeField] private GameObject destructor;
    public float animationTime = 0.3f;
    private void Update()
    {
        transform.LookAt(UIManager.Instance.sceneCamera.transform);
        transform.Rotate(0, 180, 0);
    }

    private void Start()
    {
        UIManager.Instance.allDestructors.Add(this);
        destructor.transform.localScale = Vector3.zero;
    }

    public void Destroy()
    {
        GetComponentInParent<ProducerDescriptor>().Sell();
    }

    public void Activate()
    {
        destructor.transform.DOScale(Vector3.one, animationTime);
    }

    public void Deactivate()
    {
        destructor.transform.DOScale(Vector3.zero, animationTime);
    }
    
    
}
