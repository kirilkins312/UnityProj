using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class PlacementGenerator : MonoBehaviour
//{
//    [Header("Spawn Objects")]
//    public List<SpawnObject> spawnObjects; // List of objects to spawn with height constraints

//    [Header("Settings")]
//    public MeshCollider meshCollider; // Reference to the mesh collider
//    public float radius = 2.0f;  // Minimum spacing between points
//    public int numSamplesBeforeRejection = 30;  // Controls the density of the points
//    public int maxPoints = 100;  // Maximum number of points to generate
//    public float seaLevel = 0f;  // Minimum height for tree placement

//    private List<Vector3> points; // Store generated points

//    private void Start()
//    {
//        ClearObjects();
//        GeneratePoints();
//    }
//    public void GeneratePoints()
//    {
//        if (spawnObjects == null || spawnObjects.Count == 0 || meshCollider == null)
//        {
//            Debug.LogWarning("Spawn objects list is empty or mesh collider is not assigned!");
//            return;
//        }

//        points = GeneratePoissonPoints();

//        if (points.Count == 0)
//        {
//            Debug.LogWarning("No points were generated!");
//            return;
//        }

//        // Calculate total weight and determine spawn quotas
//        float totalWeight = spawnObjects.Sum(so => so.weight);
//        Dictionary<SpawnObject, int> prefabQuotas = new Dictionary<SpawnObject, int>();
//        Dictionary<SpawnObject, int> prefabCounts = new Dictionary<SpawnObject, int>();

//        foreach (var spawnObject in spawnObjects)
//        {
//            int quota = Mathf.FloorToInt(maxPoints * (spawnObject.weight / totalWeight));
//            prefabQuotas[spawnObject] = quota;
//            prefabCounts[spawnObject] = 0;
//        }

//        foreach (var point in points)
//        {
//            List<SpawnObject> validSpawnObjects = new List<SpawnObject>();

//            foreach (var spawnObject in spawnObjects)
//            {
//                if (point.y >= spawnObject.minHeight && point.y <= spawnObject.maxHeight)
//                {
//                    if (prefabCounts[spawnObject] < prefabQuotas[spawnObject])
//                    {
//                        validSpawnObjects.Add(spawnObject);
//                    }
//                }
//            }

//            if (validSpawnObjects.Count > 0)
//            {
//                // Randomly select a spawn object from the valid list
//                var selectedObject = validSpawnObjects[Random.Range(0, validSpawnObjects.Count)];

//                Instantiate(selectedObject.prefab, point, Quaternion.identity, transform);
//                prefabCounts[selectedObject]++;
//            }

//            // Break early if all prefabs have reached their limits
//            if (prefabCounts.Values.Sum() >= maxPoints)
//            {
//                break;
//            }
//        }
//    }



//    private List<Vector3> GeneratePoissonPoints()
//    {
//        List<Vector3> points = new List<Vector3>();
//        List<Vector2> spawnPoints = new List<Vector2>();

//        Bounds bounds = meshCollider.bounds;
//        float cellSize = radius / Mathf.Sqrt(2);
//        int gridWidth = Mathf.CeilToInt(bounds.size.x / cellSize);
//        int gridHeight = Mathf.CeilToInt(bounds.size.z / cellSize);

//        int[,] grid = new int[gridWidth, gridHeight];
//        for (int x = 0; x < gridWidth; x++)
//            for (int y = 0; y < gridHeight; y++)
//                grid[x, y] = -1;

//        for (float x = bounds.min.x; x <= bounds.max.x; x += radius)
//        {
//            for (float z = bounds.min.z; z <= bounds.max.z; z += radius)
//            {
//                spawnPoints.Add(new Vector2(x, z));
//            }
//        }

//        while (spawnPoints.Count > 0 && points.Count < maxPoints)
//        {
//            int spawnIndex = Random.Range(0, spawnPoints.Count);
//            Vector2 spawnCenter = spawnPoints[spawnIndex];
//            spawnPoints.RemoveAt(spawnIndex);

//            for (int i = 0; i < numSamplesBeforeRejection; i++)
//            {
//                float angle = Random.value * Mathf.PI * 2;
//                float distance = Random.Range(radius, 2 * radius);
//                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

//                if (candidate.x < bounds.min.x || candidate.x > bounds.max.x ||
//                    candidate.y < bounds.min.z || candidate.y > bounds.max.z)
//                    continue;

//                if (IsValid(candidate, grid, cellSize, gridWidth, gridHeight, points, bounds))
//                {
//                    spawnPoints.Add(candidate);
//                    Vector3 raycastStart = new Vector3(candidate.x, bounds.max.y + radius, candidate.y);

//                    if (meshCollider.Raycast(new Ray(raycastStart, Vector3.down), out RaycastHit hit, Mathf.Infinity))
//                    {
//                        if (hit.point.y >= seaLevel) // Check sea-level limitation
//                        {
//                            points.Add(hit.point);

//                            int gridX = Mathf.FloorToInt((candidate.x - bounds.min.x) / cellSize);
//                            int gridY = Mathf.FloorToInt((candidate.y - bounds.min.z) / cellSize);
//                            grid[gridX, gridY] = points.Count - 1; // Store the point index
//                        }
//                    }
//                }
//            }
//        }

//        return points;
//    }

//    private bool IsValid(Vector2 candidate, int[,] grid, float cellSize, int gridWidth, int gridHeight, List<Vector3> points, Bounds bounds)
//    {
//        int gridX = Mathf.FloorToInt((candidate.x - bounds.min.x) / cellSize);
//        int gridY = Mathf.FloorToInt((candidate.y - bounds.min.z) / cellSize);

//        if (gridX < 0 || gridY < 0 || gridX >= gridWidth || gridY >= gridHeight)
//            return false;

//        int searchRadius = 2; // Neighbor cells to check
//        for (int x = Mathf.Max(0, gridX - searchRadius); x <= Mathf.Min(gridWidth - 1, gridX + searchRadius); x++)
//        {
//            for (int y = Mathf.Max(0, gridY - searchRadius); y <= Mathf.Min(gridHeight - 1, gridY + searchRadius); y++)
//            {
//                int pointIndex = grid[x, y];
//                if (pointIndex >= 0)
//                {
//                    float sqrDst = (new Vector2(points[pointIndex].x, points[pointIndex].z) - candidate).sqrMagnitude;
//                    if (sqrDst < radius * radius)
//                        return false;
//                }
//            }
//        }

//        return true;
//    }

//    public void ClearObjects()
//    {
//        foreach (Transform child in transform)
//        {
//            DestroyImmediate(child.gameObject);
//        }
//    }
//}


[CreateAssetMenu(fileName = "PrefabPlacementSettings", menuName = "Placement Generator/Settings")]
public class PrefabPlacementSettings : ScriptableObject
{
    [System.Serializable]
    public class PrefabCategory
    {
        public string Name;
        public GameObject Prefab;
        public int Count;
        public Vector2 SpawnAreaMin; // Bottom-left corner of the area
        public Vector2 SpawnAreaMax; // Top-right corner of the area
    }

    public List<PrefabCategory> prefabCategories;
}

public class PlacementGenerator : MonoBehaviour
{
    public PrefabPlacementSettings placementSettings;

    void Start()
    {
        foreach (var category in placementSettings.prefabCategories)
        {
            SpawnPrefabs(category);
        }
    }

    void SpawnPrefabs(PrefabPlacementSettings.PrefabCategory category)
    {
        for (int i = 0; i < category.Count; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(category.SpawnAreaMin.x, category.SpawnAreaMax.x),
                0, // Adjust for your game terrain height
                Random.Range(category.SpawnAreaMin.y, category.SpawnAreaMax.y)
            );

            Instantiate(category.Prefab, randomPosition, Quaternion.identity);
        }
    }
}

