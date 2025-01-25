using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldAutofocus : MonoBehaviour
{
    [SerializeField]
    private GameObject pivot;
    [SerializeField]
    private DepthOfField dof;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Volume>().profile.TryGet<DepthOfField>(out dof);
    }

    // Update is called once per frame
    void Update()
    {
        //float dist = Vector3.Magnitude(Camera.main.transform.position - pivot.transform.position);  
        //dof.focusDistance = new MinFloatParameter(dist, 0, true);
    }
}
