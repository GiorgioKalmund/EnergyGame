using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class Cutscene_Manager : MonoBehaviour
{
    public SpeechBubble character1Bubble; 
    public SpeechBubble character2Bubble; 
    public bool isDialogueActive = false; 
    public bool isCharacter1Speaking = true;

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
    public async void ShowNextDialogue()
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

            await character1Bubble.OpenSpeechbubble();
        }
        else
        {
            if (!character2Bubble.DialogueContainer.HasNextLine())
            {
                EndDialogue();
                return;
            }

            await character2Bubble.OpenSpeechbubble();
        }

        // Alternate the speaker for the next dialogue
        isCharacter1Speaking = !isCharacter1Speaking;
        ShowNextDialogue();
    }

    private async void EndDialogue()
    {
        isDialogueActive = false;

        // Close both speech bubbles
        await character1Bubble.CloseSpeechbubble();
        await character2Bubble.CloseSpeechbubble();
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space)) // Change KeyCode as needed
        {
            ShowNextDialogue();
        }
    }
}
