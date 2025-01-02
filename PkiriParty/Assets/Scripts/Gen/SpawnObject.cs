using MapMagic.Terrains;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
    public float weight; // Used for probability
    public float minHeight;
    public float maxHeight;
}
