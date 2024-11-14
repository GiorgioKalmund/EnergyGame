using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    [SerializeField] private GameObject overlay;

    public OverlayToggle currentActiveToggle;

    public void SetOverlay(Sprite overlaySprite, OverlayToggle callerToggle)
    {
        overlay.SetActive(true);
        if (currentActiveToggle)
        {
            currentActiveToggle.ToggleOff(); // toggles currently active toggle off
        }
        // If we need more complex display types than sprites, we should consider simply using Gameobjects & Prefabs and simply disable / enable them.
        overlay.GetComponent<Image>().sprite = overlaySprite; // updates the overlay sprite
        currentActiveToggle = callerToggle; // sets new currently active toggle
    }

    public void ClearOverlay()
    {
        overlay.SetActive(false);
        currentActiveToggle = null;
    }
}
