using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelPathMarker : MonoBehaviour
{

    public bool unlocked;
    
    [Header("Sprites")] 
    [SerializeField] private Sprite lockedImage;
    [SerializeField] private Sprite unlockedImage;

    private Image _spriteImage;
    private void Awake()
    {
        _spriteImage = GetComponent<Image>();
        Lock();
    }
     public void Unlock()
    {
        unlocked = true;
        _spriteImage.sprite = unlockedImage;
    }

    public void Lock()
    {
        unlocked = false;
        _spriteImage.sprite = lockedImage;
    }
}