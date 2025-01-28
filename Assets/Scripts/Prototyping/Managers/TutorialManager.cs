using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialSensor[] _sensors;
    [SerializeField] private SpeechBubble _tutSpeechbubble;
    private int _currentSensor = 0;
    private bool _hasEnabledNextSensor = false;
    void OnEnable(){
        TutorialSensor.continueTutorial += DisableInterruped;
    }
    void OnDisable(){
        TutorialSensor.continueTutorial -= DisableInterruped;
    }
    void Start(){
        
    }
    void Update(){
        if(!_tutSpeechbubble.isOpen && _tutSpeechbubble.DialogueContainer.HasNextLine() && !_tutSpeechbubble.IsInterruped){
            _tutSpeechbubble.OpenSpeechbubble();
        }

        if(_tutSpeechbubble.IsInterruped && !_hasEnabledNextSensor){
            _sensors[_currentSensor].enabled = true;
            _sensors[_currentSensor].Enable();
            _currentSensor++;
            _hasEnabledNextSensor = true;
        }

    }
    
    void DisableInterruped(){
        _tutSpeechbubble.IsInterruped = false;
        _hasEnabledNextSensor = false;
    }

}