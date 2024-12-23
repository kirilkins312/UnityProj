using System.Collections.Generic;
using UnityEngine;

public class PlacementGenerator : MonoBehaviour
{
    public GameObject treePrefab;  // The tree prefab to instantiate
    public MeshCollider meshCollider; // Reference to the mesh collider
    public float radius = 2.0f;  // Minimum spacing between points
    public int numSamplesBeforeRejection = 30;  // Controls the density of the points
    public int maxPoints = 100;  // Maximum number of points to generate
    public float seaLevel = 0f;  // Minimum height for tree placement

    private List<Vector3> points; // Store generated points

    private void Start()
    {
        ClearTrees();
        GeneratePoints();
    }

    public void GeneratePoints()
    {
        if (treePrefab == null || meshCollider == null)
        {
            Debug.LogWarning("Tree prefab or mesh collider is not assigned!");
            return;
        }

        points = GeneratePoissonPoints();

        foreach (var point in points)
        {
            Instantiate(treePrefab, point, Quaternion.identity, transform);
        }
    }

    private List<Vector3> GeneratePoissonPoints()
    {
        List<Vector3> points = new List<Vector3>();
        List<Vector2> spawnPoints = new List<Vector2>();

        Bounds bounds = meshCollider.bounds;
        float cellSize = radius / Mathf.Sqrt(2);
        int gridWidth = Mathf.CeilToInt(bounds.size.x / cellSize);
        int gridHeight = Mathf.CeilToInt(bounds.size.z / cellSize);

        int[,] grid = new int[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                grid[x, y] = -1;

        for (float x = bounds.min.x; x <= bounds.max.x; x += radius)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += radius)
            {
                spawnPoints.Add(new Vector2(x, z));
            }
        }

        while (spawnPoints.Count > 0 && points.Count < maxPoints)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            spawnPoints.RemoveAt(spawnIndex);

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                float distance = Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                if (candidate.x < bounds.min.x || candidate.x > bounds.max.x ||
                    candidate.y < bounds.min.z || candidate.y > bounds.max.z)
                    continue;

                if (IsValid(candidate, grid, cellSize, gridWidth, gridHeight, points, bounds))
                {
                    spawnPoints.Add(candidate);
                    Vector3 raycastStart = new Vector3(candidate.x, bounds.max.y + radius, candidate.y);

                    if (meshCollider.Raycast(new Ray(raycastStart, Vector3.down), out RaycastHit hit, Mathf.Infinity))
                    {
                        if (hit.point.y >= seaLevel) // Check sea-level limitation
                        {
                            points.Add(hit.point);

                            int gridX = Mathf.FloorToInt((candidate.x - bounds.min.x) / cellSize);
                            int gridY = Mathf.FloorToInt((candidate.y - bounds.min.z) / cellSize);
                            grid[gridX, gridY] = points.Count - 1; // Store the point index
                        }
                    }
                }
            }
        }

        return points;
    }

    private bool IsValid(Vector2 candidate, int[,] grid, float cellSize, int gridWidth, int gridHeight, List<Vector3> points, Bounds bounds)
    {
        int gridX = Mathf.FloorToInt((candidate.x - bounds.min.x) / cellSize);
        int gridY = Mathf.FloorToInt((candidate.y - bounds.min.z) / cellSize);

        if (gridX < 0 || gridY < 0 || gridX >= gridWidth || gridY >= gridHeight)
            return false;

        int searchRadius = 2; // Neighbor cells to check
        for (int x = Mathf.Max(0, gridX - searchRadius); x <= Mathf.Min(gridWidth - 1, gridX + searchRadius); x++)
        {
            for (int y = Mathf.Max(0, gridY - searchRadius); y <= Mathf.Min(gridHeight - 1, gridY + searchRadius); y++)
            {
                int pointIndex = grid[x, y];
                if (pointIndex >= 0)
                {
                    float sqrDst = (new Vector2(points[pointIndex].x, points[pointIndex].z) - candidate).sqrMagnitude;
                    if (sqrDst < radius * radius)
                        return false;
                }
            }
        }

        return true;
    }

    public void ClearTrees()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
