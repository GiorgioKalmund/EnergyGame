using UnityEngine;
public class PlaceableEntity : MonoBehaviour{


    public float Cost;
    //money you get when selling = cost* sellPercentage
    public float SellPercentage;
    public PlacementType MustPlaceOn;
    public bool IsPlaced = false;
    public void Sell(){
        BudgetManager.Instance.Sell(Cost);
        //Rest der alten Sell Function kann hier schlecht implementiert werden weil
        //sonst cross referenzen gebraucht werden und der ganze Sinn vom neuen System war modularität.

    }
    bool Place(TileData tile){

        if(!BudgetManager.Instance.UseBudget(Cost)){
            return false;
        }
        IsPlaced = true;
        //Add environmental impact here
        return true;



    }


}
