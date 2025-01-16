using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class FaceRenderer : MonoBehaviour
{
    public GameObject character;
    public RenderTexture texture;
    GameObject c;
    private Vector3 mousePos;
    
    void Start(){
        //transform.position += new Vector3(100000, 100000, 100000);
        texture.name = Random.Range(0,9999).ToString();
    }

    void Update(){
        mousePos = Input.mousePosition;
        //mousePos += new Vector3(100000, 100000, 100000);;
        mousePos.z -= 1000;

        c.transform.LookAt(mousePos);
        //c.transform.Rotate(0,90,0);
    }

    public void setTexture()
    {
        texture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        texture.Create();
        Camera cam = GetComponentInChildren<Camera>();
        cam.targetTexture = texture;
        cam.targetDisplay = -1;
        texture.Release();
    }

    public void loadCharacter(){
        Destroy(c);
        c = Instantiate(character, transform.position, Quaternion.identity);
        c.transform.SetParent(transform);
        c.transform.localPosition = new Vector3(0,0,0);
        c.transform.Rotate(0,180,0);
    }
}
