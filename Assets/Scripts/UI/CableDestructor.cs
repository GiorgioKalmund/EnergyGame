using System;
using System.ComponentModel;
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
        GetComponentInParent<ProducerDescriptor>().Sell(true);
        //GameObject.Destroy(this);
    }

    public void Activate()
    {
        LineRenderer renderer = GetComponentInParent<LineRenderer>();
        if(renderer?.enabled==false){
            return;
        } else if(renderer?.enabled == true){
            destructor.transform.DOScale(Vector3.one, animationTime);
        } else{
            destructor.transform.DOScale(Vector3.one, animationTime);
        }
        
    }

    public void Deactivate()
    {
        destructor.transform.DOScale(Vector3.zero, animationTime);
    }

    private void OnDestroy()
    {
        DOTween.Kill(destructor.transform);
        UIManager.Instance.allDestructors.Remove(this);
    }
}
