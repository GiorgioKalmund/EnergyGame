using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scipts.Prototyping.TestScipt_Lin_;
using UnityEngine;
using UnityEngine.Serialization;

public class GridDataManager : MonoBehaviour
{
    [Header("Textures")]
    [SerializeField] public Texture2D mapTexture; // Assign your input texture here
    [SerializeField] [CanBeNull] public Texture2D sunTexture;
    [SerializeField] [CanBeNull] public Texture2D windTexture;
    [SerializeField] [CanBeNull] public Texture2D waterTexture;
    
    [Header("Map")]
    [SerializeField] private Color colorOne;
    [SerializeField] private GameObject firstGameObject;
    [SerializeField] private Color colorTwo;
    [SerializeField] private GameObject secondGameObject;
    [SerializeField] private Color colorThree;
    [SerializeField] private GameObject thirdGameObject;
    private Dictionary<Color, GameObject> ColorToGameObjectMap;

    [Header("Grid")] 
    [SerializeField] private Grid grid;
    [SerializeField] private Transform tilesCenter;
    private int textureWidth;
    private int textureHeight;

    private TileData[,] gridData;

    void Start()
    {
        InitializeGridData();
    }

    private void Awake()
    {
        textureWidth = mapTexture.width;
        textureHeight = mapTexture.height;
        ColorToGameObjectMap = new Dictionary<Color, GameObject>();
        // Better naming scheme for colors and objects?
        ColorToGameObjectMap[colorOne] = firstGameObject;
        ColorToGameObjectMap[colorTwo] = secondGameObject;
        ColorToGameObjectMap[colorThree] = thirdGameObject;
        tilesCenter.transform.position = new Vector3((int)textureHeight / 2f, 1f, (int)textureWidth / 2f);
    }

    void InitializeGridData()
    {
        if (mapTexture == null)
        {
            Debug.LogError("Input texture is missing!");
            return;
        }

        gridData = new TileData[textureWidth, textureHeight];

        for (int x = 0; x < textureHeight; x++)
        {
            for (int y = 0; y < textureWidth; y++)
            {

                Color pixelColor = mapTexture.GetPixel(x, y);
                float sunlight = 1f;
                float windSpeed = 1f; 
                float waterSpeed = 1f; 
                PlacementType placementType = PlacementType.Blocked; 
                if (sunTexture)
                {
                    sunlight = sunTexture.GetPixel(x, y).a;
                    placementType = PlacementType.Default;
                }
                if (windTexture)
                {
                    windSpeed = windTexture.GetPixel(x, y).a;
                    placementType = PlacementType.Default;
                }
                if (waterTexture)
                {
                    waterSpeed = waterTexture.GetPixel(x, y).a;
                    placementType = PlacementType.Water;
                }
                // Define Grid Data Object with corresponding stats
                gridData[x, y] = new TileData(sunlight, windSpeed, waterSpeed, placementType);
                // Map color channels to your values
                GameObject objectToInstantiate = ColorToGameObjectMap[pixelColor];
                GameObject instance = Instantiate(objectToInstantiate, new Vector3(x, 1, y), Quaternion.identity);
                instance.transform.parent = tilesCenter;
            }
        }

        Debug.Log("Grid data initialized!");
    }
    public TileData[,] GetGridData()
    {
        return gridData;
    }
}

[System.Serializable]
public class TileData
{
    public float sunlightHours;
    public float windSpeed;
    public float waterSpeed;
    public PlacementType placementType;

    public TileData(float sunlight, float wind, float water, PlacementType type)
    {
        sunlightHours = sunlight;
        windSpeed = wind;
        waterSpeed = water;
        placementType = type;
        // BuidlingDescriptor: currentBuilding
    }
}