using System;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    public OverlayToggle currentActiveToggle;
    
    public static OverlayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void DetermineNewActiveToggle(OverlayToggle callerToggle)
    {
        if (currentActiveToggle)
        {
            currentActiveToggle.ToggleOff();
        }
        if (currentActiveToggle != callerToggle)
        {
            currentActiveToggle = callerToggle;
            currentActiveToggle.ToggleOn();
        }
        else
        {
            currentActiveToggle = null;
        }
    }


    public void ClearOverlay()
    {
        currentActiveToggle = null;
    }
}
