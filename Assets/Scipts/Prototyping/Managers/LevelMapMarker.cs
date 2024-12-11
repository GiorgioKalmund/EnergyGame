using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelMapMarker : MonoBehaviour
{
    public bool unlocked { get; private set; }
    public int id;

    [Header("Neighbours")] 
    [SerializeField] private LevelMapMarker prev;
    [SerializeField] private LevelMapMarker next; 
    
    [Header("Sprites")] 
    [SerializeField] private Sprite lockedImage;
    [SerializeField] private Sprite unlockedImage;

    private Image _spriteImage;
    private Button _button;

    private LineRenderer _connectingLine;

    private void OnEnable()
    {
        LevelMapManager.Instance.AddMarker(this);
        _spriteImage = GetComponent<Image>();
        _button = GetComponent<Button>();
        if (!_button)
        {
            gameObject.AddComponent<Button>();
            Debug.LogWarning(name +" didn't have a button component, added one.");
        }
        _button.onClick.AddListener(ToggleLock);

        _connectingLine = gameObject.AddComponent<LineRenderer>();
        _connectingLine.startWidth= 0.1f;
        _connectingLine.endWidth = 0.1f;
        _connectingLine.positionCount = 2;
        _connectingLine.SetPosition(0, gameObject.transform.position);
        _connectingLine.SetPosition(1, gameObject.transform.position);

        unlocked = false;
        _spriteImage.sprite = lockedImage;
    }

    public void Unlock()
    {
        if (prev)
        {
            prev.Unlock();
        }
        unlocked = true;
        _spriteImage.sprite = unlockedImage;
    }

    public void Lock()
    {
        if (next)
        {
            next.Lock();
        }
        unlocked = false;
        _spriteImage.sprite = lockedImage;
    }

    public void ToggleLock()
    {
        if (unlocked)
        {
            Lock();
        }
        else
        {
            Unlock();
        }
    }

    public LevelMapMarker GetNext()
    {
        return next;
    }
    
    public void SetNext(LevelMapMarker next)
    {
        this.next = next;
        _connectingLine.SetPosition(1, next.gameObject.transform.position);
    }

    public LevelMapMarker GetPrev()
    {
        return prev;
    }
    
    public void SetPrev(LevelMapMarker prev)
    {
        this.prev = prev;
    }
}