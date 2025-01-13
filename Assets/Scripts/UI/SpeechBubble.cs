using System;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(DialogueContainer))]
public class SpeechBubble : MonoBehaviour
{
    //This is the current string to indicate that something went wrong when getting the dialogue string 
    public static string INVALID_DIALOGUE = "NULL";
    public static float SPEECHBUBBLE_DURATION = 10f;
    [SerializeField] private TMP_Text textbox;
    [SerializeField] public DialogueContainer DialogueContainer;
    [SerializeField] public DialogueObjectSetting dialogueSetting;
    public bool isOpen = false;
    [SerializeField] private float animationTime = 0.3f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void OpenSpeechbubble(int index = -1)
    {
        string nextText = GetSpeechBubbleText(index);
        if (nextText.Equals(INVALID_DIALOGUE))
        {
            Debug.LogWarning("Invalid Dialogue String in Speechbubble.");
            return;
        }
        transform.DOScale(1f, animationTime);
        // only continue if dialogue is valid
        isOpen = true;
        textbox.text = nextText;
        //Put animation to open the speechbubble here
        CloseSpeechbubble();
    }

    public async void CloseSpeechbubble()
    {
        if (!isOpen)
        {
            Debug.LogWarning("Tried to close Speechbubble that was not open.");
            return;
        }
        await transform.DOScale(0f, animationTime).SetDelay(10).AsyncWaitForCompletion();
        isOpen = false;
        textbox.text = "";
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

    private string GetSpeechBubbleText(int index = -1) //optional
    {
        switch (dialogueSetting)
        {
            case DialogueObjectSetting.LINEAR:
                return DialogueContainer.GetNextLineOfDialogue();
            case DialogueObjectSetting.FIRSTONLY:
                return DialogueContainer.GetFirstLine();
            case DialogueObjectSetting.RANDOM:
                return DialogueContainer.GetRandom();
            case DialogueObjectSetting.INDEX:
                if(index != -1) return DialogueContainer.GetAtIndex(index);
                break;
        }
        return INVALID_DIALOGUE;
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
public enum DialogueObjectSetting
{
    LINEAR,
    FIRSTONLY,
    RANDOM,
    INDEX
}
