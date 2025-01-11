using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// from https://discussions.unity.com/t/fps-counter/683389/4
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsText;
    [SerializeField] private float _hudRefreshRate = 1f;

    private float _timer;

    private void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = "FPS: " + fps;
            _timer = Time.unscaledTime + _hudRefreshRate;
            
            if (fps < 30)
                _fpsText.color = Color.red;
            else if (fps < 65)
                _fpsText.color = new Color(255, 165, 0);
            else
                _fpsText.color = Color.green; 
                
        }
    }
}
