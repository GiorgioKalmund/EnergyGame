using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Cutscene_Manager : MonoBehaviour
{
    public SpeechBubble character1Bubble; 
    public SpeechBubble character2Bubble; 
    public bool isDialogueActive = false; 
    private bool isCharacter1Speaking = true;

    public void StartDialogue()
    {
        if (isDialogueActive)
        {     
            return;
        }

        isDialogueActive = true;
        isCharacter1Speaking = true; 
        ShowNextDialogue();
    }

    void Start()
    {
        StartDialogue();
    }
    public void ShowNextDialogue()
    {
        if (!isDialogueActive)
        {
            return;
        }

        // Alternate between characters
        if (isCharacter1Speaking)
        {
            if (!character1Bubble.DialogueContainer.HasNextLine())
            {
                EndDialogue();
                return;
            }

            character1Bubble.OpenSpeechbubble();
        }
        else
        {
            if (!character2Bubble.DialogueContainer.HasNextLine())
            {
                EndDialogue();
                return;
            }

            character2Bubble.OpenSpeechbubble();
        }

        // Alternate the speaker for the next dialogue
        isCharacter1Speaking = !isCharacter1Speaking;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        // Close both speech bubbles
        character1Bubble.CloseSpeechBubbleInstantly();
        character2Bubble.CloseSpeechBubbleInstantly();

    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space)) // Change KeyCode as needed
        {
            ShowNextDialogue();
        }
    }
}
