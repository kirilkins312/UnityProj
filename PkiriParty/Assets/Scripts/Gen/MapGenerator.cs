using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    public bool useFalloff; // Toggle to enable/disable falloff map
    float[,] falloffMap;

    void Start()
    {
        GenerateMap();
    }

    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    public void GenerateMap()
    {
        // Generate the base noise map
        float[,] noiseMap = Noise.GenerateNoiseMap(
            mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        // Load the river heightmap from your BMP file (this assumes the BMP is in Resources folder)
        Texture2D riverTexture = Resources.Load<Texture2D>("RiverHeightmap");  // Ensure the BMP is placed in the Resources folder
        if (riverTexture == null)
        {
            Debug.LogError("RiverHeightmap not found in Resources.");
            return;
        }

        // Convert river texture to heightmap
        float[,] riverHeightmap = LoadHeightmapFromBMP(riverTexture, mapChunkSize, mapChunkSize);

        // Scale and overlay river heightmap
        float[,] scaledRiverHeightmap = ScaleRiverHeightmap(riverHeightmap, 0.6f);
        noiseMap = OverlayRiverHeightmap(noiseMap, scaledRiverHeightmap, 0.8f);


        // Apply falloff map if enabled
        if (useFalloff)
        {
            float falloffStrength = 0.5f; // Adjust this value between 0 and 1 to control falloff influence
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y] * falloffStrength);
                }
            }
        }





        // Generate the color map
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        // Display the map based on the selected draw mode
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            MeshFilter meshFilter = display.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                meshFilter.sharedMesh.Clear();
            }

            display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize)
            );
        }
    }

    public static float[,] ScaleRiverHeightmap(float[,] riverHeightmap, float scalingFactor)
    {
        int width = riverHeightmap.GetLength(0);
        int height = riverHeightmap.GetLength(1);

        float[,] scaledRiverHeightmap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                scaledRiverHeightmap[x, y] = riverHeightmap[x, y] * scalingFactor;
            }
        }

        return scaledRiverHeightmap;
    }

    public static float[,] LoadHeightmapFromBMP(Texture2D texture, int targetWidth, int targetHeight)
    {
        int width = texture.width;
        int height = texture.height;
        float[,] heightmap = new float[targetWidth, targetHeight];

        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                int texX = Mathf.FloorToInt((float)x / targetWidth * width);
                int texY = Mathf.FloorToInt((float)y / targetHeight * height);

                heightmap[x, y] = texture.GetPixel(texX, texY).grayscale;
            }
        }

        return heightmap;
    }

    public static float[,] OverlayRiverHeightmap(float[,] terrainHeightmap, float[,] riverHeightmap, float riverStrength)
    {
        int width = terrainHeightmap.GetLength(0);
        int height = terrainHeightmap.GetLength(1);

        float[,] blendedHeightmap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                blendedHeightmap[x, y] = Mathf.Lerp(terrainHeightmap[x, y], riverHeightmap[x, y], riverStrength);
            }
        }

        return blendedHeightmap;
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }

        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);

        if (Application.isPlaying)
        {
            GenerateMap();
        }
    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}