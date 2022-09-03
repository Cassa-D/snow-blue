using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    Tree, Rock, Ramp
}

[Serializable]
public class Map
{
    public MapType type;
    public GameObject prefab;
}

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    private int createTMPMaps = 3;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int i = 0; i < createTMPMaps && i < maps.Length; i++)
        {
            var map = maps[i];


            var groundChild = map.prefab.transform.Find("Ground");
            var groundSize = groundChild.localScale.z;
            var rotationAngle = groundChild.rotation.eulerAngles.x;
            
            var createdMap = Instantiate(map.prefab);
            // createdMap.transform.position

            if (i == 0) continue;
            
            var prevMap = maps[i - 1];
            var prevGroundChild = prevMap.prefab.transform.Find("Ground");
            var prevGroundSize = prevGroundChild.localScale.z;
            var prevRotationAngle = prevGroundChild.rotation.eulerAngles.x;
        }
    }
}
