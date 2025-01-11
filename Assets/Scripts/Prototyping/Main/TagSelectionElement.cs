using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TagSelectionElement : MonoBehaviour
{
  public Ease animationEase = Ease.InOutCubic;
  public float animationDuration = 0.3f;
  private bool open;
  private bool active = true;
  public TreeTagType type;

  private void Awake()
  {
    transform.localScale = Vector3.zero;
    open = false;
  }

  public void Open()
  {
    if (open || !active)
      return;
    
    transform.DOScale(1f, animationDuration).SetEase(animationEase).SetRecyclable();
    GetComponent<Image>().DOFade(1f, animationDuration).SetEase(animationEase).SetRecyclable();
    OverlaysDropdown.Instance.globallyActiveTypes.Add(type);
    open = true;
  }

  public void Close()
  {
    if (!open)
      return;
    
    transform.DOScale(0f, animationDuration).SetEase(animationEase).SetRecyclable();
    GetComponent<Image>().DOFade(0f, animationDuration).SetEase(animationEase).SetRecyclable();
    OverlaysDropdown.Instance.globallyActiveTypes.Remove(type);
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

  public void Deactivate()
  {
    active = false;
  }
  
  public void Activate()
  {
    active = false;
  }

  public void SetActive(bool activate)
  {
    active = activate;
  }
  
  private void OnDestroy()
  {
    DOTween.Kill(transform);
    DOTween.Kill(GetComponent<Image>());
  }
}