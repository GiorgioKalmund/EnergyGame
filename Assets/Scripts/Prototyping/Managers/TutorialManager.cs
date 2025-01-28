using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialSensor[] _sensors;
    [SerializeField] private SpeechBubble _tutSpeechbubble;
    [SerializeField] private int tutorialLevel;
    private int _currentSensor = 0;
    private bool _hasEnabledNextSensor = false;
    private bool _hasOpenedSpeechbubble = false;
    void OnEnable(){
        TutorialSensor.continueTutorial += DisableInterruped;
    }
    void OnDisable(){
        TutorialSensor.continueTutorial -= DisableInterruped;
    }
    void Start(){
        
    }
    void Update(){
        if(!_tutSpeechbubble.isOpen && _tutSpeechbubble.DialogueContainer.HasNextLine() && !_tutSpeechbubble.IsInterruped && !_hasOpenedSpeechbubble){
            _tutSpeechbubble.OpenSpeechbubble();
            Debug.Log("Opening Speechbubble");
            _hasOpenedSpeechbubble = true;
        }
        else{
            _hasOpenedSpeechbubble = false;
        }

        if(_tutSpeechbubble.IsInterruped && !_hasEnabledNextSensor){
            _sensors[_currentSensor].enabled = true;
            _sensors[_currentSensor].Enable();
            _currentSensor++;
            _hasEnabledNextSensor = true;
            switch(tutorialLevel){
                case 1:
                    switch(_currentSensor){
                        case 1:
                            OverlaysDropdown.Instance.Expand();
                            break;
                        case 2:
                            OverlaysDropdown.Instance.Expand();
                            break;
                        case 3:
                            OverlaysDropdown.Instance.Expand();
                            break;
                        case 4:
                            OverlaysDropdown.Instance.CollapseAllTags();
                            BuilderInventory.Instance.ShowInventory();
                            break;
                    }
                    break;
            }
        }

    }
    
    void DisableInterruped(){
        _tutSpeechbubble.isOpen = false;
        _tutSpeechbubble.OpenSpeechbubble();
        _tutSpeechbubble.IsInterruped = false;
        _hasEnabledNextSensor = false;
        
    }

}