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
        if(uiButton.image){   
            originalColor = uiButton.targetGraphic.color;
        }
        if(GetComponent<RawImage>()){
            originalColor = GetComponent<RawImage>().color;
        }

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
        if(uiButton.image){   
            uiButton.image.color = originalColor;
        }
        if(GetComponent<RawImage>()){
            GetComponent<RawImage>().color = originalColor;
        }
        
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            // Change to blink color
            if(uiButton.image){   
                uiButton.image.color = blinkColor;
            }
            if(GetComponent<RawImage>()){
                GetComponent<RawImage>().color = blinkColor;
            }
            yield return new WaitForSeconds(blinkTime);

            // // Revert to original color
            if(uiButton.image){   
                uiButton.image.color = originalColor;
            }
            if(GetComponent<RawImage>()){
                GetComponent<RawImage>().color = originalColor;
            }
            yield return new WaitForSeconds(blinkTime);
        }
    }

    
}
