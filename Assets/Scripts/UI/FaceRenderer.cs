using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FaceRenderer : MonoBehaviour
{
    public GameObject character;
    public RenderTexture texture;
    GameObject c;
    
    void Start(){
        transform.position = new Vector3(100000 + UnityEngine.Random.Range(-1000, 1000), 100000 + UnityEngine.Random.Range(-1000, 1000),100000 + UnityEngine.Random.Range(-1000, 1000));
        texture.name = UnityEngine.Random.Range(0,9999).ToString();
    }

    public void setTexture()
    {
        texture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        texture.Create();
        GetComponentInChildren<Camera>().targetTexture = texture;
        GetComponentInChildren<Camera>().targetDisplay = -1;
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
