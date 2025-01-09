using UnityEngine;
using TMPro;

public class EndpointBanner : MonoBehaviour
{
    public GameObject endpointGameObject;
    public float x, y;

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

    public void SetEndpoint(GameObject endpoint, float x, float y)
    {
        endpointGameObject = endpoint;
        this.x = x;
        this.y = y;
    }

    public void UpdateText(float value)
    {
        text.text = $"{value}MW";
    }
}
