using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionCircle : MonoBehaviour
{
    [SerializeField] Sprite regularSprite;
    [SerializeField] Sprite collidingSprite;

    private SpriteRenderer _spriteRenderer;

    public bool colliding = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_spriteRenderer != null && regularSprite != null)
        {
            _spriteRenderer.sprite = regularSprite;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _spriteRenderer.sprite = collidingSprite;
        colliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _spriteRenderer.sprite = regularSprite;
        colliding = false;
    }

    public void HideSprite()
    {
        _spriteRenderer.enabled = false;
    }

    public void ShowSprite()
    {
        _spriteRenderer.enabled = true;
    }
}
