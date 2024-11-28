using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Scipts.Prototyping.TestScipt_Lin_;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class GridDataManager : MonoBehaviour
{
    [Header("Textures")]
    [SerializeField] public Texture2D mapTexture;
    [SerializeField] [CanBeNull] private Texture2D sunTexture;
    [SerializeField] [CanBeNull] private Texture2D windTexture;
    [SerializeField] [CanBeNull] private Texture2D waterTexture;
    [SerializeField] [CanBeNull] private Texture2D displayTexture;
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
    private int textureWidth;
    private int textureHeight;

    private TileData[,] gridData;

    private void Awake()
    {
        textureWidth = mapTexture.width;
        textureHeight = mapTexture.height;

        mapOverlay.GetComponent<Renderer>().material.mainTexture = displayTexture;
        
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
            { color9, PlacementType.Endpoint}  // Enpoints 
        };

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
                        sunlight = sunTexture.GetPixel(x, y).a;
                    }
                    if (windTexture)
                    {
                        windSpeed = windTexture.GetPixel(x, y).a;
                    }
                    if (waterTexture)
                    {
                        waterSpeed = waterTexture.GetPixel(x, y).a;
                    }

                    //map each block with placementType
                    placementMappings.TryGetValue(pixelColor, out PlacementType placementType);
                    
                    TileData tileData = new TileData(sunlight, windSpeed, waterSpeed, placementType, new Vector2(x, y), null);

                    
                    if (instance.GetComponent<ProducerDescriptor>())
                    {
                        tileData.setCurrentBuilding(instance.GetComponent<ProducerDescriptor>());
                    }
                   
                    //assign tileDataInformation to each block
                    TileDataWrapper wrapper = instance.AddComponent<TileDataWrapper>();
                    wrapper.tileData = tileData;

                }
                else
                {
                    Debug.Log("Could not find appropriate map entry for "+pixelColor);
                }
            }
        }
    }
}

[System.Serializable]
public class TileData
{
    float sunlightHours;
    public float windSpeed;
    public float waterSpeed;
    public PlacementType placementType;
    [CanBeNull] public ISelectableEntity currentBuilding;
    public Vector2 coords;

    public TileData(float sunlight, float wind, float water, PlacementType type, Vector2 coords, ProducerDescriptor currentBuilding)
    {
        sunlightHours = sunlight;
        windSpeed = wind;
        waterSpeed = water;
        placementType = type;
        this.coords = coords;
        this.currentBuilding = currentBuilding;
    }
    public void setPlacementType(PlacementType type)
    {
        placementType = type;
    }

    public void setCurrentBuilding(ISelectableEntity building)
    {
        this.currentBuilding = building;
    }
}