using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scipts.Prototyping.TestScipt_Lin_;
using UnityEngine;
using UnityEngine.Serialization;

public class GridDataManager : MonoBehaviour
{
    [SerializeField] public Texture2D mapTexture; // Assign your input texture here
    [SerializeField] [CanBeNull] public Texture2D sunTexture;
    [SerializeField] [CanBeNull] public Texture2D windTexture;
    [SerializeField] [CanBeNull] public Texture2D waterTexture;

    private Dictionary<Color, GameObject> ColorToGameObjectMap;
    [SerializeField] private GameObject whiteGameObject; 
    [SerializeField] private GameObject blackGameObject;

    [Header("Grid")] 
    [SerializeField] private Grid grid;
    private int textureWidth;
    private int textureHeight;

    const float cellWidth = 1f;
    const float cellHeight = 1f;

    private TileData[,] gridData;

    [SerializeField] private Transform parent;

    void Start()
    {
        InitializeGridData();
    }

    private void Awake()
    {
        textureWidth = mapTexture.width;
        textureHeight = mapTexture.height;
        ColorToGameObjectMap = new Dictionary<Color, GameObject>();
        ColorToGameObjectMap[Color.white] = whiteGameObject;
        ColorToGameObjectMap[Color.black] = blackGameObject;
        parent.transform.position = new Vector3((int)textureHeight / 2f, 1f, (int)textureWidth / 2f);
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
                gridData[x, y] = new TileData(sunlight, windSpeed, waterSpeed);
                // Map color channels to your values
                GameObject objectToInstantiate = ColorToGameObjectMap[pixelColor];
                GameObject instance = Instantiate(objectToInstantiate, new Vector3(x, 1, y), Quaternion.identity);
                instance.transform.parent = parent;
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

    public TileData(float sunlight, float wind, float water)
    {
        sunlightHours = sunlight;
        windSpeed = wind;
        waterSpeed = water;
        placementType = PlacementType.Default;
        // BuidlingDescriptor: currentBuilding
        // PlacementType: type
    }
}