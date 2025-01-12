using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Speechbubbles")]
    [SerializeField] private SpeechBubble budget;
    [SerializeField] private SpeechBubble pollution;
    [SerializeField] private SpeechBubble builder;
    [SerializeField] private SpeechBubble endpoints;
    [SerializeField] private SpeechBubble quijote;

    [Header("Dialogues")]
    [SerializeField] private Dialogue budgetDialogue;
    [SerializeField] private Dialogue pollutionDialogue;
    [SerializeField] private Dialogue builderDialogue;
    [SerializeField] private Dialogue endpointsDialogue;
    [SerializeField] private Dialogue quijoteDialogue;
    
    [Header("DialogueSetting")]
    [SerializeField] private DialogueObjectSetting budgetDialogueSetting;
    [SerializeField] private DialogueObjectSetting pollutionDialogueSetting;
    [SerializeField] private DialogueObjectSetting builderDialogueSetting;
    [SerializeField] private DialogueObjectSetting endpointsDialogueSetting;
    [SerializeField] private DialogueObjectSetting quijoteDialogueSetting;
    

    void Start()
    {
        //budget.DialogueContainer.Dialogue = budgetDialogue;
        budget.dialogueSetting = budgetDialogueSetting;

        pollution.DialogueContainer.Dialogue = pollutionDialogue;
        pollution.dialogueSetting = pollutionDialogueSetting;

        /* builder.DialogueContainer.Dialogue = builderDialogue;
        builder.dialogueSetting = builderDialogueSetting; */

        endpoints.DialogueContainer.Dialogue = endpointsDialogue;
        endpoints.dialogueSetting = endpointsDialogueSetting;

        quijote.DialogueContainer.Dialogue = quijoteDialogue;
        quijote.dialogueSetting = quijoteDialogueSetting;
    }

    // Update is called once per frame
    void Update()
    {
        //budget.DialogueContainer.Dialogue = budgetDialogue;
        budget.dialogueSetting = budgetDialogueSetting;

        pollution.DialogueContainer.Dialogue = pollutionDialogue;
        pollution.dialogueSetting = pollutionDialogueSetting;

        /* builder.DialogueContainer.Dialogue = builderDialogue;
        builder.dialogueSetting = builderDialogueSetting; */

        endpoints.DialogueContainer.Dialogue = endpointsDialogue;
        endpoints.dialogueSetting = endpointsDialogueSetting;

        quijote.DialogueContainer.Dialogue = quijoteDialogue;
        quijote.dialogueSetting = quijoteDialogueSetting;
    }
    public void OpenSpeechbubbleBudgetExceeded(){
        budget.OpenSpeechbubble();
    }
    public void OpenSpeechbubblePollutionExceeded(){
        pollution.OpenSpeechbubble();
    }

    
}
