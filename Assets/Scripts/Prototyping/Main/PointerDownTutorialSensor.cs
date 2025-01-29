using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerDownTutorialSensor : TutorialSensor, IPointerDownHandler
{
    private Color blinkColor = Color.yellow;
    [SerializeField]
    private float blinkTime = 0.5f;
    private Button uiButton;
    private Color originalColor;
    public override void Enable()
    {
        uiButton = GetComponentInChildren<Button>();
        originalColor = uiButton.targetGraphic.color;

        Debug.Log("Pointer down has been enabled");

        startBlinking();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnContinueTutorial();
        Debug.Log("Ich habe worden gedrückt");

        stopBlinking();
    }

    public void startBlinking(){
        StartCoroutine(BlinkEffect());
    }

    public void stopBlinking(){
        StopAllCoroutines();
        uiButton.image.color = originalColor;
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            // Change to blink color
            uiButton.image.color = blinkColor;
            yield return new WaitForSeconds(blinkTime);

            // // Revert to original color
            uiButton.image.color = originalColor;
            yield return new WaitForSeconds(blinkTime);
        }
    }

    
}
