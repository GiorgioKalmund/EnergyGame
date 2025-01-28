using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialSensor : MonoBehaviour
{
    void Start(){
        this.enabled = false;
    }
    public static event Action continueTutorial;
    protected virtual void OnContinueTutorial(){
        continueTutorial?.Invoke();
        this.enabled =false;
    }
    public abstract void Enable();

}
