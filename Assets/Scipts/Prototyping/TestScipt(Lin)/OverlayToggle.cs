using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OverlayToggle : MonoBehaviour
{
    public bool isOn = false;
    [Header("Sprites")]
    [SerializeField] private Sprite toggleOffSprite;
    [SerializeField] private Sprite toggleOnSprite;
    [FormerlySerializedAs("overlayImage")]
    [Header("Overlay")]
    [SerializeField] private Sprite overlaySprite;
    [SerializeField] private OverlayManager manager;

    private Image _imageComponent;

    private void Start()
    {
        _imageComponent = GetComponent<Image>();

        // The manager could also be retrieved via getting the Parent, however this limits hierarchial flexibility.
        // Although then no manual setting of it for every toggle is needed.
        // manager = GetComponentInParent<OverlayManager>();
    }

    // Called by the Button Component on the UI Element
    public void Toggle()
    {
        isOn = !isOn;
        _imageComponent.sprite = isOn ? toggleOnSprite : toggleOffSprite;

        if (isOn)
        {
            manager.SetOverlay(overlaySprite, this);
        }
        else
        {
            manager.ClearOverlay();
        }
        Debug.Log(isOn);
    }

    public void ToggleOff()
    {
        this.isOn = false;
        _imageComponent.sprite = isOn ? toggleOnSprite : toggleOffSprite;

    }

    public void ToggleOn()
    {
        this.isOn =  true;
        _imageComponent.sprite = isOn ? toggleOnSprite : toggleOffSprite;
    }

    public bool IsOn()
    {
        return this.isOn;
    }
}
