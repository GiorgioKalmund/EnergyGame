using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Speechbubbles")]
    [SerializeField] private SpeechBubble budget;
    [SerializeField] private SpeechBubble pollution;
    [SerializeField] private SpeechBubble builder;
    [SerializeField] private SpeechBubble power1;
    [SerializeField] private SpeechBubble quijote;

    [Header("Dialogues")]
    [SerializeField] private Dialogue budgetDialogue;
    [SerializeField] private Dialogue pollutionDialogue;
    [SerializeField] private Dialogue builderDialogue;
    [SerializeField] private Dialogue power1Dialogue;
    [SerializeField] private Dialogue quijoteDialogue;
    
    [Header("DialogueSetting")]
    [SerializeField] private DialogueObjectSetting budgetDialogueSetting;
    [SerializeField] private DialogueObjectSetting pollutionDialogueSetting;
    [SerializeField] private DialogueObjectSetting builderDialogueSetting;
    [SerializeField] private DialogueObjectSetting power1DialogueSetting;
    [SerializeField] private DialogueObjectSetting quijoteDialogueSetting;
    

    void Start()
    {
        budget.DialogueContainer.Dialogue = budgetDialogue;
        budget.dialogueSetting = budgetDialogueSetting;

        pollution.DialogueContainer.Dialogue = pollutionDialogue;
        pollution.dialogueSetting = pollutionDialogueSetting;

        builder.DialogueContainer.Dialogue = builderDialogue;
        builder.dialogueSetting = builderDialogueSetting;

        power1.DialogueContainer.Dialogue = power1Dialogue;
        power1.dialogueSetting = power1DialogueSetting;

        quijote.DialogueContainer.Dialogue = quijoteDialogue;
        quijote.dialogueSetting = quijoteDialogueSetting;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenSpeechbubbleBudgetExceeded(){
        budget.OpenSpeechbubble();
    }
    public void OpenSpeechbubblePollutionExceeded(){
        pollution.OpenSpeechbubble();
    }

    
}
