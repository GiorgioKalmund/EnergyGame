using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Managers")]
    [SerializeField] private OverlayManager _overlayManager;
    [SerializeField] private SelectionManager _selectionManager;
    
    [Header("Cursor")]
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;

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
        SetCursorTextureDefault();
    }

    void Update()
    {
        ChangeCursor();
    }
    private void ChangeCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorTextureDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetCursorTextureDefault();
        }
    }

    private void SetCursorTextureDefault()
    {
        Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
    }
    
    private void SetCursorTextureDown()
    {
        Cursor.SetCursor(cursorDownTexture, Vector2.zero, CursorMode.Auto);
    }
}
