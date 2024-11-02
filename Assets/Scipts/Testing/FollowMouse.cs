using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{

    [SerializeField] bool following = true;
    [SerializeField] CollisionCircle collision;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !collision.colliding)
        {
            following = false;
            collision.HideSprite();
        }

        if (following)
        {
            SetPositionUnderCursor();
        }
    }

    void SetPositionUnderCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
    }
}
