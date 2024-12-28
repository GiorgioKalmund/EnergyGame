using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(DialogueContainer))]
public class SpeechBubble : MonoBehaviour
{
    //This is the current string to indicate that something went wrong when getting the dialogue string 
    public static string INVALID_DIALOGUE = "NULL";
    public static float SPEECHBUBBLE_DURATION = 10f;
    [SerializeField] private TMP_Text textbox;
    [SerializeField] private DialogueContainer dialogueContainer;
    [SerializeField] private DialogueObjectSetting dialogueSetting;
    private bool isOpen = false;


    void OpenSpeechbubble()
    {
        string nextText = GetSpeechBubbleText();
        if (nextText.Equals(INVALID_DIALOGUE))
        {
            Debug.LogWarning("Invalid Dialogue String in Speechbubble.");
            return;
        }
        // only continue if dialogue is valid
        isOpen = true;
        textbox.text = nextText;
        //Put animation to open the speechbubble here
        StartCoroutine(CloseSpeechbubbleAfterTimeout(SPEECHBUBBLE_DURATION));
    }

    void CloseSpeechbubble()
    {
        if (!isOpen)
        {
            Debug.LogWarning("Tried to close Speechbubble that was not open.");
            return;
        }
        isOpen = false;
        textbox.text = "";
        //Put animation to close the speechbubble here

        //Avoids calling close twice
        StopAllCoroutines();
    }

    //Call this in the smiley button
    public void ToggleSpeechBubble()
    {
        if (!isOpen)
        {
            OpenSpeechbubble();

        }
        else
        {
            CloseSpeechbubble();
        }
    }
    private IEnumerator CloseSpeechbubbleAfterTimeout(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CloseSpeechbubble();
    }

    private string GetSpeechBubbleText()
    {
        switch (dialogueSetting)
        {
            case DialogueObjectSetting.LINEAR:
                return dialogueContainer.GetNextLineOfDialogue();
            case DialogueObjectSetting.FIRSTONLY:
                return dialogueContainer.GetFirstLine();
            case DialogueObjectSetting.RANDOM:
                return dialogueContainer.GetRandom();

        }
        return INVALID_DIALOGUE;
    }


}
enum DialogueObjectSetting
{
    LINEAR,
    FIRSTONLY,
    RANDOM
}
