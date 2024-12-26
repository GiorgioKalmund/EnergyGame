using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Drag this onto the object that should have dialogue functionality and drag the corresponding dialogue Scriptable Object into the field.
public class DialogueContainer : MonoBehaviour
{
    [SerializeField] public Dialogue Dialogue;
    private int dialogueCounter = 0;

    
    //Use this to get the next line of dialogue
    //TODO: Add magic numbers to do functionality between dialogue text
    public String GetNextLineOfDialogue(){
        if(dialogueCounter < Dialogue.DialogueText.Length){
            return Dialogue.DialogueText[dialogueCounter++];

        } else {
            return "";
        }
    }
}
