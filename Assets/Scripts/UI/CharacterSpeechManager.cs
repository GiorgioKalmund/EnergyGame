using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CharacterSpeechManager : MonoBehaviour
{
    [Header("Characters")] 
    [SerializeField] private SpeechBubble endpointsBubble;
    [SerializeField] private SpeechBubble co2Bubble;
    [SerializeField] private SpeechBubble financeBubble;
    [SerializeField] private SpeechBubble donBubble;
    [SerializeField] private SpeechBubble builderBubble;
    public static CharacterSpeechManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
        
        if (!endpointsBubble || !co2Bubble || !financeBubble || !donBubble)
            Debug.LogError("CharacterSpeechManager: Missing at least one bubble reference!");
    }

    void Start(){
        StartCoroutine(StartFirstBuilderDialogue(5));
    }

    public void EndpointsBubbleAction(SpeechBubbleAction action)
    {
        if (action == SpeechBubbleAction.OPEN)
            endpointsBubble.OpenSpeechbubble();
        else if (action == SpeechBubbleAction.CLOSE)
            endpointsBubble.CloseSpeechbubble();
        else
            endpointsBubble.ToggleSpeechBubble();
    }
    
    public void Co2BubbleAction(SpeechBubbleAction action)
    {
        if (action == SpeechBubbleAction.OPEN)
            co2Bubble.OpenSpeechbubble();
        else if (action == SpeechBubbleAction.CLOSE)
            co2Bubble.CloseSpeechbubble();
        else
            co2Bubble.ToggleSpeechBubble();
    }
    
    public void FinanceBubbleAction(SpeechBubbleAction action)
    {
        if (action == SpeechBubbleAction.OPEN)
            financeBubble.OpenSpeechbubble();
        else if (action == SpeechBubbleAction.CLOSE)
            financeBubble.CloseSpeechbubble();
        else
            financeBubble.ToggleSpeechBubble();
    }
    
    public void DonBubbleAction(SpeechBubbleAction action)
    {
        if (action == SpeechBubbleAction.OPEN)
            donBubble.OpenSpeechbubble();
        else if (action == SpeechBubbleAction.CLOSE)
            donBubble.CloseSpeechbubble();
        else
            donBubble.ToggleSpeechBubble();
    }

    public void BuilderBubbleAction(SpeechBubbleAction action){
        if(action == SpeechBubbleAction.OPEN){
            builderBubble.OpenSpeechbubble();
        } else if(action == SpeechBubbleAction.CLOSE){
            builderBubble.CloseSpeechbubble();
        } else{
            builderBubble.ToggleSpeechBubble();
        }
    }

    private IEnumerator StartFirstBuilderDialogue(int seconds){
        yield return new WaitForSecondsRealtime(seconds);
        BuilderBubbleAction(SpeechBubbleAction.OPEN);
    }

}

    public enum SpeechBubbleAction{
    OPEN, CLOSE, TOGGLE
}