using System.Text.RegularExpressions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Smiley : MonoBehaviour
{
    [Header("Character")]
    [SerializeField]
    private Smileys character;
    
    public Expression Expression;
    [SerializeField]
    private GameObject[] models;
    [Header("Rendering")]
    [SerializeField]
    public GameObject FaceRenderer;
    private RenderTexture tex;
    private Expression prevExpression;
    private FaceRenderer model;
    private int off = 100;

    private int insted = 0;
    private RawImage img;
    void Start(){
        img = GetComponentInChildren<RawImage>();
        FaceRenderer = Instantiate(FaceRenderer, GetComponent<RectTransform>().transform.position, Quaternion.identity);
        off = off+Random.Range(1,900);
        FaceRenderer.transform.position *= off;
        //faceRenderer.transform.SetParent(Camera.main.transform);
        FaceRenderer.GetComponent<FaceRenderer>().setTexture();
        model = FaceRenderer.GetComponent<FaceRenderer>();
        model.off = off;
        tex = FaceRenderer.GetComponent<FaceRenderer>().texture;
        if(tex == null){
            Debug.Log("No texture");
        }
        if(Regex.Match(SceneManager.GetActiveScene().name,@"B[0-9]_C").Success){
            img.enabled = false;
        }
        img.texture = tex;
        prevExpression = Expression;
        
        switch(character){
            case Smileys.Baumeister:
                switch(Expression){
                    case Expression.Frown:
                    case Expression.Smile:
                    case Expression.Neutral:
                        model.character = models[0];
                        break;
                    default:
                        Debug.Log("Error with Expression selection in UI Smileys");
                        break;
                }
                break;
            case Smileys.Bürgermeisterin:
                switch(Expression){
                        case Expression.Frown:
                            model.character = models[1];
                            break;
                        case Expression.Neutral:
                            model.character = models[2];
                            break;
                        case Expression.Smile:
                            model.character = models[3];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            case Smileys.Don:
            switch(Expression){            
                        case Expression.Neutral:
                        case Expression.Smile:
                        case Expression.Frown:
                            model.character = models[4];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            case Smileys.Greta:
            switch(Expression){
                        case Expression.Frown:
                            model.character = models[5];
                            break;
                        case Expression.Neutral:
                            model.character = models[6];
                            break;
                        case Expression.Smile:
                            model.character = models[7];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            case Smileys.Monopoly:
            switch(Expression){
                        case Expression.Frown:
                            model.character = models[8];
                            break;
                        case Expression.Neutral:
                            model.character = models[9];
                            break;
                        case Expression.Smile:
                            model.character = models[10];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            default:
                Debug.Log("Error with character selection in ui Smileys");
                break;
        }
        model.loadCharacter();
    }

    void Update(){

        if(Expression != prevExpression){
            Debug.Log("Switched from "+prevExpression+" to "+Expression);
            switch(character){
            case Smileys.Baumeister:
                switch(Expression){
                    case Expression.Frown:
                    case Expression.Smile:
                    case Expression.Neutral:
                        model.character = models[0];
                        break;
                    default:
                        Debug.Log("Error with Expression selection in UI Smileys");
                        break;
                }
                break;
            case Smileys.Bürgermeisterin:
                switch(Expression){
                        case Expression.Frown:
                            model.character = models[1];
                            break;
                        case Expression.Neutral:
                            model.character = models[2];
                            break;
                        case Expression.Smile:
                            model.character = models[3];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            case Smileys.Don:
            switch(Expression){            
                        case Expression.Neutral:
                        case Expression.Smile:
                        case Expression.Frown:
                            model.character = models[4];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            case Smileys.Greta:
            switch(Expression){
                        case Expression.Frown:
                            model.character = models[5];
                            break;
                        case Expression.Neutral:
                            model.character = models[6];
                            break;
                        case Expression.Smile:
                            model.character = models[7];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            case Smileys.Monopoly:
            switch(Expression){
                        case Expression.Frown:
                            model.character = models[8];
                            break;
                        case Expression.Neutral:
                            model.character = models[9];
                            break;
                        case Expression.Smile:
                            model.character = models[10];
                            break;
                        default:
                            Debug.Log("Error with Expression selection in UI Smileys");
                            break;
                    }
                break;
            default:
                Debug.Log("Error with character selection in ui Smileys");
                break;
        }
        model.loadCharacter();
        }

        prevExpression = Expression;
    }
    public void SetRenderTextureActive(bool isActive){
        img.enabled = isActive;
    }
    public GameObject GetFaceRenderer(){
        return FaceRenderer;
    }
    public RawImage GetImg(){
        return img;
    }

    void LateUpdate()
    {
        // Frame magic
        if (insted < 3)
            insted++;
        if (insted == 3)
        {
            Vector3[] fourCornersArray = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(fourCornersArray);
            if(GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera){
                model.relativePos = Camera.main.WorldToScreenPoint(fourCornersArray[0]);
            }
            else{
                model.relativePos = fourCornersArray[0];
            }
            insted = 4;
        }
    }

    public void cutsceneLookAt(GameObject o = null){
        model.lookAt = o;
    }
}

enum Smileys{
    Baumeister,
    Greta,
    Don,
    Monopoly,
    Bürgermeisterin
}

public enum Expression{
    Frown,
    Neutral,
    Smile
}
