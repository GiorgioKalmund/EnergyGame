using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OverlayToggle : MonoBehaviour
{
    public bool isOn = false;
    [Header("Sprites")]
    [SerializeField] private Sprite toggleOffSprite;
    [SerializeField] private Sprite toggleOnSprite;
    [Header("Overlay")]
    [SerializeField] private GameObject overlayGameObject;

    private Image _imageComponent;

    private void Start()
    {
        _imageComponent = GetComponent<Image>();
        overlayGameObject.SetActive(false);

        // The manager could also be retrieved via getting the Parent, however this limits hierarchial flexibility.
        // Although then no manual setting of it for every toggle is needed.
        // manager = GetComponentInParent<OverlayManager>();
    }

    public void Toggle()
    {
        OverlayManager.Instance.DetermineNewActiveToggle(this);
    }

    public void ToggleOff()
    {
        this.isOn = false;
        _imageComponent.sprite = isOn ? toggleOnSprite : toggleOffSprite;
        overlayGameObject.SetActive(false);
        CameraManager.Instance.ResetCameraTransform();
    }

    public void ToggleOn()
    {
        this.isOn =  true;
        _imageComponent.sprite = isOn ? toggleOnSprite : toggleOffSprite;
        overlayGameObject.SetActive(true);
        CameraManager.Instance.MoveCameraToTargetTransform();
    }

    public bool IsOn()
    {
        return this.isOn;
    }
}
