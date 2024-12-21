public enum PlacementType
{
    // Default: Land towers can be placed on this tile
    // Water: Water towers can be placed on this tile
    // Blocked: No towers can be placed on this tile
    // Shore: Towers between Water and Default can be placed on this tile
    // Endpoint: Connection point for cities and similar to receive power 
    Default, Water, Blocked, Shore, Endpoint
}