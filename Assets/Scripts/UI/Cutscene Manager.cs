using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

public class Cutscene_Manager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueTurn
    {
        public SpeechBubble speaker; // The character speaking
    }

    public List<DialogueTurn> dialogueSequence; 
    public bool isDialogueActive = false;

    private int currentTurnIndex = 0;


    public void StartDialogue()
    {
        if (isDialogueActive)
        {
            return;
        }

        if (dialogueSequence == null || dialogueSequence.Count == 0)
        {
            Debug.LogWarning("Dialogue sequence is empty!");
            return;
        }

        isDialogueActive = true;
        currentTurnIndex = 0;

        StartCoroutine(DialogueRoutine());
    }


    void Start()
    {
        StartDialogue();
    }


    private System.Collections.IEnumerator DialogueRoutine()
    {
        while (isDialogueActive)
        {
            if (currentTurnIndex >= dialogueSequence.Count)
            {
                EndDialogue();
                yield break; // Exit the coroutine
            }

            DialogueTurn currentTurn = dialogueSequence[currentTurnIndex];
            SpeechBubble currentSpeaker = currentTurn.speaker;

            // Check if the current speaker has dialogue left
            if (!currentSpeaker.DialogueContainer.HasNextLine())
            {
                Debug.LogWarning($"{currentSpeaker.name} has no more lines to speak.");
                EndDialogue();
                yield break;
            }

            // Show the next line for the current speaker
            yield return currentSpeaker.OpenSpeechbubble();

            // Move to the next turn
            currentTurnIndex++;

            // Wait for user input to continue
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }

    private async void EndDialogue()
    {
        isDialogueActive = false;

        // Close all speech bubbles instantly
        foreach (var turn in dialogueSequence)
        {
            turn.speaker.CloseSpeechBubbleInstantly();
        }
    }

    
}
