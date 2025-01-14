using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Smiley : MonoBehaviour
{
    [Header("Character")]
    [SerializeField]
    private Smileys character;
    [SerializeField]
    private Expression expression;
    [SerializeField]
    private GameObject[] models;
    [Header("Rendering")]
    [SerializeField]
    private GameObject faceRenderer;
    private RenderTexture tex;
    private Expression prevExpression;
    private FaceRenderer model;

    private int insted = 0;
    void Start(){
        RawImage img = GetComponentInChildren<RawImage>();
        faceRenderer = Instantiate(faceRenderer, GetComponent<RectTransform>().transform.position, Quaternion.identity);
        faceRenderer.GetComponent<FaceRenderer>().setTexture();
        model = faceRenderer.GetComponent<FaceRenderer>();
        tex = faceRenderer.GetComponent<FaceRenderer>().texture;
        if(tex == null){
            Debug.Log("No texture");
        }
        img.texture = tex;
        prevExpression = expression;

        switch(character){
            case Smileys.Baumeister:
                switch(expression){
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
                switch(expression){
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
            switch(expression){            
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
            switch(expression){
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
            switch(expression){
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

        if(expression != prevExpression){
            Debug.Log("Switched from "+prevExpression+" to "+expression);
            switch(character){
            case Smileys.Baumeister:
                switch(expression){
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
                switch(expression){
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
            switch(expression){            
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
            switch(expression){
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
            switch(expression){
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

        prevExpression = expression;
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
            faceRenderer.transform.position = fourCornersArray[0];
        }
    }
}

enum Smileys{
    Baumeister,
    Greta,
    Don,
    Monopoly,
    Bürgermeisterin
}

enum Expression{
    Frown,
    Neutral,
    Smile
}
