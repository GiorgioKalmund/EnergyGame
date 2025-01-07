using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ScalingElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float animTime = 0.33f;
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, animTime).SetRecyclable();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, animTime).SetRecyclable();
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}