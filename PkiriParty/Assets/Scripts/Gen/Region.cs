using UnityEngine;

[System.Serializable]
public class Region
{
    public Vector3 center; // Center of the region
    public Vector3 size; // Size of the region (for box-shaped areas)

    public Bounds GetBounds()
    {
        return new Bounds(center, size);
    }

}