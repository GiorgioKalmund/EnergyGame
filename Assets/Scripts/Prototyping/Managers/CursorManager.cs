using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private Texture2D cursorDefaultTexture;
    [SerializeField] private Texture2D cursorDownTexture;
    [SerializeField] private Vector2 hotspot = new Vector2(5,2);


    private void Start()
    {
        Cursor.SetCursor(cursorDefaultTexture, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        ChangeCursor();
    }

    private void ChangeCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetDownCursor();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetDefaultCursor();
        }
    }

   private void SetDefaultCursor()
    {
        Cursor.SetCursor(cursorDefaultTexture, hotspot, CursorMode.Auto);
    } 
    
    private void SetDownCursor()
    {
        Cursor.SetCursor(cursorDownTexture,hotspot, CursorMode.Auto);
    }
}