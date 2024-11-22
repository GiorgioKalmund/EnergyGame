using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GridDataManager : MonoBehaviour
{
    [SerializeField]
    public Texture2D inputTexture; // Assign your input texture here


    public int gridWidth;
    public int gridHeight;

    private TileData[,] gridData;



    void Start()
    {
        InitializeGridData();

    }

    void InitializeGridData()
    {
        if (inputTexture == null)
        {
            Debug.LogError("Input texture is missing!");
            return;
        }

        gridData = new TileData[gridWidth, gridHeight];

        float cellWidth = inputTexture.width / (float)gridWidth;
        float cellHeight = inputTexture.height / (float)gridHeight;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calculate the corresponding pixel position
                int pixelX = Mathf.FloorToInt(x * cellWidth);
                int pixelY = Mathf.FloorToInt(y * cellHeight);

                Color pixelColor = inputTexture.GetPixel(pixelX, pixelY);

                // Map color channels to your values
                float sunlight = pixelColor.r; 
                float windSpeed = pixelColor.g; 
                float waterSpeed = pixelColor.b; 

                gridData[x, y] = new TileData(sunlight, windSpeed, waterSpeed);
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

    public TileData(float sunlight, float wind, float water)
    {
        sunlightHours = sunlight;
        windSpeed = wind;
        waterSpeed = water;
    }
}