using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Cutscene_Manager : MonoBehaviour
{
    [SerializeField] public int NextScene;
    [System.Serializable]
    public class DialogueTurn
    {
        public SpeechBubble speaker; 
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
            //dislogEnd exit
            if (currentTurnIndex >= dialogueSequence.Count)
            {
                EndDialogue();          
                yield break;
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

            yield return currentSpeaker.OpenSpeechbubble();
            // Move to the next turn
            currentTurnIndex++;

            // Wait for user input to continue
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            currentSpeaker.CloseSpeechBubbleInstantly();

        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;

        // Close all speech bubbles completely
        foreach (var turn in dialogueSequence)
        {
            turn.speaker.CloseSpeechBubbleInstantly(); 
        }
        
        // TODO: Probably not instant switch (ask Design)
        SceneManager.LoadScene(NextScene);
    }
}
