using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TagSelectionElement : MonoBehaviour
{
  public Ease animationEase = Ease.InOutCubic;
  public float animationDuration = 0.3f;
  private bool open;

  private void Awake()
  {
    transform.localScale = Vector3.zero;
    open = false;
  }

  public void Open()
  {
    if (open)
      return;
    
    transform.DOScale(1f, animationDuration).SetEase(animationEase).SetRecyclable();
    GetComponent<Image>().DOFade(1f, animationDuration).SetEase(animationEase).SetRecyclable();
    open = true;
  }

  public void Close()
  {
    if (!open)
      return;
    
    transform.DOScale(0f, animationDuration).SetEase(animationEase).SetRecyclable();
    GetComponent<Image>().DOFade(0f, animationDuration).SetEase(animationEase).SetRecyclable();
    open = false;
  }

  public void Toggle()
  {
    if (open)
      Close();
    else
      Open();
  }

  public bool IsOpen()
  {
    return open;
  }

  private void OnDestroy()
  {
    DOTween.KillAll();
  }
}