using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public Transform playerTra;

    public Vector3 mapAngle;
    public int stayMaps;

    private bool _creatingSection;
    private List<GameObject> _generatedMaps = new List<GameObject>();

    void Start()
    {
        transform.rotation = Quaternion.Euler(mapAngle);
    }

    private void Update()
    {
        if (!_creatingSection)
        {
            _creatingSection = true;
            GenerateMap();
        }

        if (_generatedMaps.Count > stayMaps)
        {
            Destroy(_generatedMaps[0]);
            _generatedMaps.RemoveAt(0);
        }

        if (playerTra.position.z >= _generatedMaps[^1].transform.position.z)
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
            var prevChildGroud = prevMap.transform.Find("Ground");
            var prevPos = prevMap.transform.localPosition.z + 10 * prevChildGroud.transform.localScale.z;

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
        _creatingSection = false;
    }
}
