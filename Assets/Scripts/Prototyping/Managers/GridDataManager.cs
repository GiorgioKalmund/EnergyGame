using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class GridDataManager : MonoBehaviour
{
    [Header("Textures")]
    [SerializeField] public Texture2D mapTexture;
    [SerializeField] [CanBeNull] public Texture2D sunTexture;
    [SerializeField] [CanBeNull] public Texture2D windTexture;
    [SerializeField] [CanBeNull] public Texture2D waterTexture;
    [SerializeField] [CanBeNull] public Texture2D coalTexture;
    [SerializeField] private GameObject mapOverlay;

    [Header("Tiles")]
    [SerializeField] private Color color1;
    [SerializeField] private GameObject prefab1;
    [SerializeField] private Color color2;
    [SerializeField] private GameObject prefab2;
    [SerializeField] private Color color3;
    [SerializeField] private GameObject prefab3;
    [SerializeField] private Color color4;
    [SerializeField] private GameObject prefab4;
    [SerializeField] private Color color5;
    [SerializeField] private GameObject prefab5;
    [SerializeField] private Color color6;
    [SerializeField] private GameObject prefab6;
    [SerializeField] private Color color7;
    [SerializeField] private GameObject prefab7;
    [SerializeField] private Color color8;
    [SerializeField] private GameObject prefab8;
    [SerializeField] private Color color9;
    [SerializeField] private GameObject prefab9;
    
    [Header("Map")]
    private Dictionary<Color, GameObject> ColorToGameObjectMap;
    private Dictionary<Color, PlacementType> placementMappings;

    [Header("Grid")] 
    [SerializeField] private Grid grid;
    [SerializeField] private Transform tilesCenter;
    private static int textureWidth;
    private static int textureHeight;

    private Color clearColor = Color.clear;
    [Header("Overlay Colours")] 
    public float strength1;
    public float strength2;
    public float strength3;
    public float strength4;
    [Header("WIND")] 
    [SerializeField] private Color wind1 = Color.white;
    [SerializeField] private Color wind2 = Color.white;
    [SerializeField] private Color wind3 = Color.white;
    [SerializeField] private Color wind4 = Color.white;

    [Header("SUN")] 
    [SerializeField] private Color sun1 = Color.white;
    [SerializeField] private Color sun2 = Color.white;
    [SerializeField] private Color sun3 = Color.white;
    [SerializeField] private Color sun4 = Color.white;
    
    [Header("WATER")] 
    [SerializeField] private Color water1 = Color.white;
    [SerializeField] private Color water2 = Color.white;
    [SerializeField] private Color water3 = Color.white;
    [SerializeField] private Color water4 = Color.white;

    [Header("COAL")]
    [SerializeField] private Color coal1 = Color.white;

    [SerializeField] private GameObject endpointPrefab;
   
    /// <summary>
    /// 
    /// First index: x 
    /// Second index: y
    /// Third index: 0 for tile, 1 for building
    /// </summary>
    public static GameObject[,,] GridData = new GameObject[255,255,2];

    public static GridDataManager Instance;

    private void Awake()
    {
        textureWidth = mapTexture.width;
        textureHeight = mapTexture.height;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Renderer r = mapOverlay.GetComponent<Renderer>();
        r.GetPropertyBlock(block);
        block.SetTexture("_Texture", mapTexture);
        r.SetPropertyBlock(block);
        
        ColorToGameObjectMap = new Dictionary<Color, GameObject>
        {
            { color1, prefab1 },
            { color2, prefab2 },
            { color3, prefab3 },
            { color4, prefab4 },
            { color5, prefab5 },
            { color6, prefab6 },
            { color7, prefab7 },
            { color8, prefab8 },
            { color9, prefab9 }
        };
        placementMappings = new Dictionary<Color, PlacementType>
        {
            { color1, PlacementType.Blocked},  // Commercial
            { color2, PlacementType.Default }, // Default
            { color3, PlacementType.Blocked }, // Forest
            { color4, PlacementType.Blocked }, // Railroad
            { color5, PlacementType.Blocked},  // Residential
            { color6, PlacementType.Shore },   // Shore
            { color7, PlacementType.Blocked }, // Street
            { color8, PlacementType.Water },   // Water
            { color9, PlacementType.Endpoint}  // Endpoints 
        };

        if (Instance && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        tilesCenter.transform.position = new Vector3((int)textureHeight / 2f, 1f, (int)textureWidth / 2f);
        GenerateMap();

        //SetUp for Grid default (1x1)
        grid.cellSize = new Vector3(1, 1, 1);
        grid.transform.position = new Vector3((int)textureHeight , 1f, (int)textureWidth );
        
        Debug.Log($"Grid setup complete. Center position : {tilesCenter.position}, Grid position: {grid.transform.position}");
    }

    private void GenerateMap()
    {
        float sunlight = 0;
        float waterSpeed = 0; 
        float windSpeed = 0;
        float coalAmount= 0;
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                Color pixelColor = mapTexture.GetPixel(x, y);
                // Check if the color matches a prefab
                if (ColorToGameObjectMap.TryGetValue(pixelColor, out GameObject prefab))
                {
                    // Instantiate the prefab
                    Vector3 position = new Vector3(x, 1, y);
                    //get each GameObject
                    GameObject instance = Instantiate(prefab, position, Quaternion.identity);
                    instance.transform.parent = tilesCenter;
                    instance.name = $"{x},{y}";

                    if (sunTexture)
                    {
                        var color  = sunTexture.GetPixel(x, y);
                        if (color.Equals(sun1))
                            sunlight = strength1;
                        else if (color.Equals(sun2))
                            sunlight = strength2;
                        else if (color.Equals(sun3))
                            sunlight = strength3;
                        else if (color.Equals(sun4))
                            sunlight = strength4;
                        else sunlight = 0f;
                    }
                    if (waterTexture)
                    {
                        var color  = waterTexture.GetPixel(x, y);
                        if (color.Equals(water1))
                            waterSpeed = strength1;
                        else if (color.Equals(water2))
                            waterSpeed = strength2;
                        else if (color.Equals(water3))
                            waterSpeed = strength3;
                        else if (color.Equals(water4))
                            waterSpeed = strength4;
                        else waterSpeed = 0f; 
                    }
                    if (windTexture)
                    {
                        var color  = windTexture.GetPixel(x, y);
                        if (color.Equals(wind1))
                            windSpeed = strength1;
                        else if (color.Equals(wind2))
                            windSpeed = strength2;
                        else if (color.Equals(wind3))
                            windSpeed = strength3;
                        else if (color.Equals(wind4))
                            windSpeed = strength4;
                        else windSpeed = 0f;
                    }
                    if (coalTexture)
                    {
                        coalAmount = coalTexture.GetPixel(x, y).a;
                    }

                    //map each block with placementType
                    placementMappings.TryGetValue(pixelColor, out PlacementType placementType);

                    if (placementType == PlacementType.Endpoint)
                    {
                        GameObject endpoint = Instantiate(endpointPrefab,new Vector3(x,PlacementManager.Instance.cellIndicatorPlacementY - 0.02f,y),Quaternion.identity);
                        if (LevelManager.Instance)
                            endpoint.GetComponentInChildren<Wandler>().endpointDemand = LevelManager.Instance.endpointDemands[LevelManager.Instance.endpointsCount++];
                        else
                            Debug.LogError("GridDataManager: No LevelManager found!");
                        GridData[x,y,1] = endpoint;
                    }
                    
                    TileData tileData = new TileData(sunlight, windSpeed, waterSpeed, coalAmount, placementType, new Vector2(x, y), null);
                    
                    if (instance.GetComponent<ProducerDescriptor>())
                    {
                        tileData.SetCurrentBuilding(instance.GetComponent<ProducerDescriptor>());
                    }
                   
                    //assign tileDataInformation to each block
                    TileDataWrapper wrapper = instance.AddComponent<TileDataWrapper>();
                    wrapper.tileData = tileData;

                    
                    GridData[x,y,0] = instance;
                }
                else
                {
                    Debug.LogError("Could not find appropriate map entry for "+ pixelColor.ToHexString());
                }
            }
        }

        if (UIManager.Instance)
        {
            UIManager.Instance.SetEndpointsCompleted(0);
        }
    }
    public static Vector3Int ConvertGridPosToArrayPos(Vector3Int pos){
        int tmp = pos.y;

        pos.y = pos.z;
        pos.z = tmp;

        pos.x = textureWidth + pos.x;
        pos.y = textureHeight + pos.y;

        /* pos.x -= 1;
        pos.y -= 1; */
        
        return pos;
    }
    /// <summary>
    /// Inserts gameobject into GridData array
    /// </summary>
    /// <param name="pos">Position in Array</param>
    /// <param name="gameObject">Object to put into array</param>
    public static void SetGridDataAtPos(Vector3Int pos, GameObject gameObject){
        GridData[pos.x,pos.y,pos.z] = gameObject;
    }
    /// <summary>
    ///  Gets the gameobject at the position specified by pos
    /// </summary>
    /// <param name="pos">Position in Array</param>
    /// <returns>The gameobject at Position or null</returns>
    public static GameObject GetGridDataAtPos(Vector3Int pos){
         
        return GridData[Math.Clamp(pos.x,0,textureWidth-1),Math.Clamp(pos.y,0,textureHeight-1),pos.z];
    }
    public bool OverlayTexturesAllExistent()
    {
        return windTexture && sunTexture && coalTexture && waterTexture;
    }
    public static Vector3Int GetArrayPositionAtMousePosition(){
        Grid grid = PlacementManager.Instance.Grid;
        Vector3 mousePos = InputManager.Instance.GetMousePositionInWorldSpace();
        Vector3Int gridPosition = grid.WorldToCell(mousePos + new Vector3(0.5f, 0, 0.5f));
        Vector3Int arrPosition = ConvertGridPosToArrayPos(gridPosition);
        return arrPosition;
    }

    public List<Color> GetWindColors()
    {
        return new List<Color>() { wind1, wind2, wind3, wind4 };
    }
    public List<Color> GetSunColors()
    {
        return new List<Color>() { sun1, sun2, sun3, sun4};
    }
    public List<Color> GetWaterColors()
    {
        return new List<Color>() { water1, water2, water3, water4 };
    }
    // TODO: If ever more coal colors are added, this has to be changed
    public List<Color> GetCoalColors()
    {
        return new List<Color>() { coal1, coal1, coal1, coal1 };
    }

    public float[] GetIntensities()
    {
        return new[] { strength1, strength2, strength3, strength4 };
    }
}

[System.Serializable]
public class TileData
{
    public float sunlightHours;
    public float windSpeed;
    public float waterSpeed;
    public float coalAmount;
    private PlacementType defaultPlacementType;
    public PlacementType currentPlacementType;
    [CanBeNull] public ISelectableEntity currentBuilding;
    public Vector2 coords;

    public TileData(float sunlight, float wind, float water,float coal, PlacementType type, Vector2 coords, ProducerDescriptor currentBuilding)
    {
        sunlightHours = sunlight;
        windSpeed = wind;
        waterSpeed = water;
        coalAmount = coal;
        defaultPlacementType = type;
        currentPlacementType = type;
        this.coords = coords;
        this.currentBuilding = currentBuilding;
    }
    public void setPlacementType(PlacementType type)
    {
        currentPlacementType = type;
    }

    public PlacementType GetCurrentPlacementType()
    {
        return currentPlacementType;
    }

    public void SetCurrentBuilding(ISelectableEntity building)
    {
        this.currentBuilding = building;
    }

    public void Reset()
    {
        currentBuilding = null;
        currentPlacementType = defaultPlacementType;
    }

}