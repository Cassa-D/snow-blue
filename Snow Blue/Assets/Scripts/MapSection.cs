using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSection
{
    public string name;
    public GameObject mapPrefab;
    
    [Range(0, 4)]
    public int difficulty;

    public float groundSize;
}
