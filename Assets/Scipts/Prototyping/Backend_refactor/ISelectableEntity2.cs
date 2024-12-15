public interface ISelectableEntity2{

    bool isSelected{
        get;
        set;
    }
    string name {
        get;
        
    }
    void Select();
    void Deselect();



}