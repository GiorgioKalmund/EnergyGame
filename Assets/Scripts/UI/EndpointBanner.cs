using UnityEngine;
using TMPro;

public class EndpointBanner : MonoBehaviour
{
    public GameObject endpointGameObject;

    [SerializeField] private TMP_Text text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!endpointGameObject)
            return;
        
        Vector3 screenPoint = UIManager.Instance.sceneCamera.WorldToScreenPoint(endpointGameObject.transform.position);
        transform.position = new Vector3(screenPoint.x, transform.position.y, transform.position.z);
    }

    public void SetEndpoint(GameObject endpoint)
    {
        endpointGameObject = endpoint;
    }

    public void UpdateText(float value, float demand)
    {
        text.text = $"{value:F0}MW / {demand:F0}MW";
    }
}
