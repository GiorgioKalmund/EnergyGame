using UnityEngine;
using UnityEngine.UI;
public interface ISelectableEntity
{
    public void ToggleSelection();
    public void Select();
    public void Deselect();

    public bool IsSelected();

    public bool IsOnLeftHalfOfTheScreen();

    public int GetID();

    public Sprite GetSprite();

    public string GetName();

    public void ToggleTag(int combination);
    public void CloseTag();

}