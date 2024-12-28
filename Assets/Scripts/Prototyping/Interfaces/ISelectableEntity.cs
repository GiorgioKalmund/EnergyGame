using UnityEngine;
using UnityEngine.UI;
public interface ISelectableEntity
{
    public void Select();
    public void Deselect();

    public bool IsSelected();

    public bool IsOnLeftHalfOfTheScreen();

    public int GetID();

    public Sprite GetSprite();

    public string GetName();

    public void OpenTag(int combination);
    public void CloseTag();

}