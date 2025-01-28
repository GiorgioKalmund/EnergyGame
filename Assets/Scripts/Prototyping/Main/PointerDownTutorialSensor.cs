using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownTutorialSensor : TutorialSensor, IPointerDownHandler
{
    public override void Enable()
    {
        Debug.Log("Pointer down has been enabled");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnContinueTutorial();
        Debug.Log("Ich habe worden gedrückt");
    }

    
}
