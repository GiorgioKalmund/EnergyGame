using UnityEngine;

public class SelectableEntity : MonoBehaviour{

    public bool IsSelected {get; set;}
    //Put here not in Wandler etc. because it is only used for GUI atm
    public string Name {get; private set;}

    //White outline
    [SerializeField] private GameObject selectedHighlight = null;
    //Sprite in GUI
    [SerializeField] private Sprite guiSprite;

    void Awake(){
        if(Name == ""){
            Name = gameObject.name;
        }
    }
    void Select(){
        IsSelected= true;
        selectedHighlight.SetActive(true);
    }
    void Deselect(){
        IsSelected = false;
        selectedHighlight.SetActive(false);
    }



}