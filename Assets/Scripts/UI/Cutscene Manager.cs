using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Cutscene_Manager : MonoBehaviour
{
    [SerializeField] public string NextScene;
    [SerializeField] public GameObject skipButton;
    [System.Serializable]
    public class DialogueTurn
    {
        public SpeechBubble speaker; 
    }

    public List<DialogueTurn> dialogueSequence; 
    public bool isDialogueActive = false;

    private int currentTurnIndex = 0;

    private void Awake()
    {
        skipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        skipButton.SetActive(false);
    }

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

        ShowNextDialogue();
    }


    void Start()
    {
        StartDialogue();
    }

    public void SkipCurrentLine()
    {
        SpeechBubble currentSpeaker = dialogueSequence[currentTurnIndex].speaker;
        currentSpeaker.CloseSpeechBubbleInstantly();
    }



    public async void ShowNextDialogue()
    {
        if (!isDialogueActive)
        {
            EndDialogue();
            return;
        }

        if (currentTurnIndex >= dialogueSequence.Count)
        {
            EndDialogue();
            return;
        }

        DialogueTurn currentTurn = dialogueSequence[currentTurnIndex];
        SpeechBubble currentSpeaker = currentTurn.speaker;

        // Check if the current speaker has dialogue left
        if (!currentSpeaker.DialogueContainer.HasNextLine())
        {
            EndDialogue();
            return;
        }

        await currentSpeaker.OpenSpeechbubble();
        currentTurnIndex++;
        //currentSpeaker.CloseSpeechBubbleInstantly();

        ShowNextDialogue();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        //CloseAllSpeechBubbles();

        //change skip button into start
        if (skipButton != null)
        {
            skipButton.SetActive(true);
        }
    }

    public void nextScene()
    {
        SceneManager.LoadScene(NextScene);
    }


    public void CloseAllSpeechBubbles()
    {
        foreach (var turn in dialogueSequence)
        {
            turn.speaker.transform.localScale = Vector3.zero; 
            turn.speaker.isOpen = false;   
        }
    }
}
