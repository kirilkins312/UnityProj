using UnityEngine;
using System.Collections;

public static class Noise
{

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float[,] riverMask = null, float riverStrength = 1f)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        // Blend the river mask into the noise map if provided
        if (riverMask != null)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.Lerp(noiseMap[x, y], 0, riverMask[x, y] * riverStrength);
                }
            }
        }

        return noiseMap;
    }
    public static float[,] GenerateRiverMask(int mapWidth, int mapHeight, int riverWidth)
    {
        float[,] riverMask = new float[mapWidth, mapHeight];

        int centerX = mapWidth / 2; // Example: vertical river at the center
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float distanceFromCenter = Mathf.Abs(x - centerX);
                riverMask[x, y] = Mathf.Clamp01(1 - (distanceFromCenter / riverWidth));
            }
        }

        return riverMask;
    }

    public static float[,] GenerateWavyRiverMask(int mapWidth, int mapHeight, float noiseScale, int riverWidth)
    {
        float[,] riverMask = new float[mapWidth, mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            float riverCenter = Mathf.PerlinNoise(y / noiseScale, 0) * mapWidth;

            for (int x = 0; x < mapWidth; x++)
            {
                // Adjust the river width by extending the falloff range
                float distanceFromCenter = Mathf.Abs(x - riverCenter) / riverWidth;

                // Invert and clamp the distance to create the mask
                riverMask[x, y] = Mathf.Clamp01(1 - distanceFromCenter); // Wider river with a gentler drop-off
            }
        }

        return riverMask;
    }


    public static float[,] AddRiverWithFalloff(float[,] noiseMap, float[,] riverMask, float riverStrength, float falloffScale, float widthMultiplier)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        float[,] modifiedNoiseMap = new float[mapWidth, mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // Apply a falloff function to the river mask
                float distance = riverMask[x, y];

                // Adjust the falloff to account for the width multiplier
                float falloff = Mathf.Pow(1 - (distance / widthMultiplier), falloffScale);
                float adjustedRiverMask = Mathf.Clamp01(distance * falloff);

                // Blend the river and the noise map
                modifiedNoiseMap[x, y] = Mathf.Lerp(noiseMap[x, y], 0, adjustedRiverMask * riverStrength);
            }
        }

        return modifiedNoiseMap;
    }



}