using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable All

[Serializable]
public class Map
{
    [Range(0, 4)]
    public int difficulty;
    public GameObject prefab;
}

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public Transform playerTra;

    public Vector3 mapAngle;
    
    public int stayMaps;
    public int preBuildMaps;

    private bool _creatingSection;
    private List<GameObject> _generatedMaps;

    void Start()
    {
        _generatedMaps = new List<GameObject>();
        transform.rotation = Quaternion.Euler(mapAngle);

        for (var i = 0; i < preBuildMaps; i++)
        {
            GenerateMap();
        }
    }

    private void Update()
    {
        if (!_creatingSection)
        {
            _creatingSection = true;
            GenerateMap();
        }

        if (_generatedMaps.Count > stayMaps + preBuildMaps)
        {
            Destroy(_generatedMaps[0]);
            _generatedMaps.RemoveAt(0);
        }

        if (playerTra.position.z >= _generatedMaps[^preBuildMaps].transform.position.z)
        {
            _creatingSection = false;
        }
    }

    void GenerateMap()
    {
        var index = Random.Range(0, maps.Length);
        var map = maps[index];

        var groundChild = map.prefab.transform.Find("Ground");
        // Z Scale * Half plane unit default size (10 / 2)
        var groundPos = groundChild.localScale.z * 10 / 2;

        if (_generatedMaps.Count > 0)
        {
            var prevMap = _generatedMaps[^1];
            var prevChildGround = prevMap.transform.Find("Ground");
            var prevPos = prevMap.transform.localPosition.z + 10 * prevChildGround.transform.localScale.z / 2 + groundPos - 0.1f;

            groundPos = prevPos;
        }
        
        var createdMap = Instantiate(map.prefab, transform);
        createdMap.transform.localPosition = new Vector3(0, 0, groundPos);
        _generatedMaps.Add(createdMap);
    }

    public void Reset()
    {
        foreach (var map in _generatedMaps)
        {
            Destroy(map);
        }
        _generatedMaps = new List<GameObject>();
        for (var i = 0; i < preBuildMaps; i++)
        {
            GenerateMap();
        }
        _creatingSection = false;
    }
}
