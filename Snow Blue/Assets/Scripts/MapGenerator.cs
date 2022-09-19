using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : ResetScript
{
    public GameObject[] maps;
    public Transform playerTra;

    public Vector3 mapAngle;
    
    public int stayMaps;
    public int preBuildMaps;

    private bool _creatingSection;
    private List<GameObject> _generatedMaps;

    public PointsManager pManager;
    private int _currDifficultyLevel;

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

        _currDifficultyLevel = Mathf.FloorToInt(pManager.meters / 100);
    }

    void GenerateMap()
    {
        var currDifficultyLevelMaps = GetCurrDifficultyLevelMaps();
        
        var index = Random.Range(0, currDifficultyLevelMaps.Length);
        var map = currDifficultyLevelMaps[index];
        
        var groundChild = map.transform.Find("Ground");
        // Z Scale * Half plane unit default size (10 / 2)
        var groundPos = groundChild.localScale.z * 10 / 2;
        
        if (_generatedMaps.Count > 0)
        {
            var prevMap = _generatedMaps[^1];
            var prevChildGround = prevMap.transform.Find("Ground");
            var prevPos = prevMap.transform.localPosition.z + 10 * prevChildGround.transform.localScale.z / 2 + groundPos;
        
            groundPos = prevPos;
        }
        
        var createdMap = Instantiate(map, transform);
        createdMap.transform.localPosition = new Vector3(0,  0, groundPos);
        _generatedMaps.Add(createdMap);
    }

    private GameObject[] GetCurrDifficultyLevelMaps()
    {
        var tmpMaps = new List<GameObject>();
        foreach (var map in maps)
        {
            if (_currDifficultyLevel >= map.GetComponent<MapSection>().difficulty)
            {
                tmpMaps.Add(map);
            }
        }
        return tmpMaps.ToArray();
    }

    public override void Reset()
    {
        _currDifficultyLevel = 0;
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

    public override void Reset(Vector3 sP) {}
}
