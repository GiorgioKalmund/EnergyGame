using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class FaceRenderer : MonoBehaviour
{
    public GameObject character;
    public RenderTexture texture;
    GameObject c;
    private Transform mousePos;
    public GameObject empty;
    public GameObject lookAt;
    public Vector3 relativePos;
    public int off;
    
    void Start(){
        //transform.position += new Vector3(100000, 100000, 100000);
        texture.name = Random.Range(0,9999).ToString();
        empty = new GameObject();
        mousePos = empty.transform;
    }

    void Update(){
        if(lookAt == null){
            //mousePos += new Vector3(100000, 100000, 100000);
            //mousePos.position = new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height, -1000);
            //mousePos.position *= GetComponentInParent<Canvas>().
            mousePos.position = Input.mousePosition;
            mousePos.position += new Vector3(0,0,-1000);

            c.transform.LookAt(mousePos.TransformPoint(transform.position - relativePos));
            //c.transform.Rotate(0,90,0);
        }
        else{
            mousePos.position = lookAt.transform.position*off;
            c.transform.DOLookAt(mousePos.position + transform.position - relativePos, 0.5f);
        }
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
