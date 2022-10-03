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
    private int _currDifficultyLevel = 0;
    private int _lastDifficultyLevel = 0;

    private GameObject[] _currDifficultyLevelMaps;

    [SerializeField] private Transform ground; 

    void Start()
    {
        _generatedMaps = new List<GameObject>();
        transform.rotation = Quaternion.Euler(mapAngle);

        ground.localScale = new Vector3(ground.localScale.x,1,0);

        _currDifficultyLevelMaps = GetCurrDifficultyLevelMaps();
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
            ManageDifficultyLevel();
            GenerateMap();
        }

        if (_generatedMaps.Count > stayMaps + preBuildMaps)
        {
            ground.localScale -= new Vector3(0,0,_generatedMaps[0].GetComponent<MapSection>().groundSize * 10);
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
        var index = Random.Range(0, _currDifficultyLevelMaps.Length);
        var map = _currDifficultyLevelMaps[index];
        
        var childSize = map.GetComponent<MapSection>().groundSize * 10;
        var groundPos = childSize / 2;

        if (_generatedMaps.Count > 0)
        {
            var prevMap = _generatedMaps[^1];
            var prevChildSize = prevMap.GetComponent<MapSection>().groundSize;
            var prevPos = prevMap.transform.localPosition.z + 10 * prevChildSize / 2 + groundPos;
        
            groundPos = prevPos;
        }
        
        var createdMap = Instantiate(map, transform);
        createdMap.transform.localPosition = new Vector3(0, 0, groundPos);
        _generatedMaps.Add(createdMap);

        ground.localScale += new Vector3(0, 0, childSize);
        ground.localPosition += new Vector3(0, 0, childSize / 2 + _generatedMaps.Count > 1 ? _generatedMaps[^2].GetComponent<MapSection>().groundSize / 2 : 0);
    }

    private void ManageDifficultyLevel()
    {
        _currDifficultyLevel = Mathf.FloorToInt(pManager.meters / 100);

        if (_lastDifficultyLevel != _currDifficultyLevel)
        {
            _lastDifficultyLevel = _currDifficultyLevel;
            _currDifficultyLevelMaps = GetCurrDifficultyLevelMaps();
        }
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
        _currDifficultyLevelMaps = GetCurrDifficultyLevelMaps();
        foreach (var map in _generatedMaps)
        {
            Destroy(map);
        }

        ground.localScale = new Vector3(ground.localScale.x, 1, 0);
        ground.localPosition = new Vector3(0,-0.5f, 0);
        _generatedMaps = new List<GameObject>();
        for (var i = 0; i < preBuildMaps; i++)
        {
            GenerateMap();
        }
        _creatingSection = false;
    }

    public override void Reset(Vector3 sP) {}
}
