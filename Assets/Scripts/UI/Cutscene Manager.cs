using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Cutscene_Manager : MonoBehaviour
{
    [SerializeField] public string NextScene;
    [SerializeField] public GameObject skipButton;
    [Serializable]
    public class DialogueTurn
    {
        public SpeechBubble speaker; 
    }

    SpeechBubble currentSpeaker;

    public List<DialogueTurn> dialogueSequence; 
    public bool isDialogueActive = false;

    private int currentTurnIndex = 0;

    SpeechBubble _currentSpeaker = null;
    
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
        skipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        skipButton.SetActive(false);

        
    }

    void Update(){
        if(currentSpeaker && !currentSpeaker.isOpen){
            ShowNextDialogue();
        }
    }

    public void SkipCurrentLine()
    {
        if(currentTurnIndex >= dialogueSequence.Count) return; 
        SpeechBubble currentSpeaker = dialogueSequence[currentTurnIndex].speaker;
        currentSpeaker.CloseSpeechBubbleInstantly();
    }

    void Update(){

        if(_currentSpeaker && !_currentSpeaker.isOpen && isDialogueActive){
        ShowNextDialogue();
        }
    }

    

    void ShowNextDialogue(){
        if(!isDialogueActive || currentTurnIndex >= dialogueSequence.Count){
            EndDialogue();
            return;
        }
        
        DialogueTurn currentTurn = dialogueSequence[currentTurnIndex];
        _currentSpeaker = currentTurn.speaker;

        if(!_currentSpeaker.DialogueContainer.HasNextLine()){
            EndDialogue();
            return;
        }
        _currentSpeaker.OpenSpeechbubble();
        currentTurnIndex++;
        
        
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
