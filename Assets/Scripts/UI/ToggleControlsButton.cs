using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToggleControlsButton : MonoBehaviour
{
   [SerializeField] private GameObject panel;
   private bool panelActive;
   public float animationTime;
   private void Start()
   {
      GetComponent<Button>().onClick.AddListener(TogglePanel);
      panel.transform.localScale = Vector3.zero;
   }

   public void TogglePanel()
   {
      if (!panelActive)
         panel.transform.DOScale(1f, animationTime).SetEase(Ease.InOutSine);
      else
         panel.transform.DOScale(0f, animationTime).SetEase(Ease.InOutSine);

      panelActive = !panelActive;
   }
}