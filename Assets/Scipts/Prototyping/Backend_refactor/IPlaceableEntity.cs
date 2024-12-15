public interface IPlaceableEntity{
    float Cost {
        get;

    }
    //money you get when selling = cost* sellPercentage
    float SellPercentage{
        get;
    }
    PlacementType MustPlaceOn{
        get;
    }
    void Sell();
    bool Place(TileData tile);



}