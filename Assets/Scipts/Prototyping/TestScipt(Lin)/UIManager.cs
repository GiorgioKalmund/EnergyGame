using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private OverlayManager _overlayManager;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }

        {
            Instance = this;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
