using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public float meters;
    public Transform player;

    [CanBeNull] public TMP_Text textGUI;

    // Update is called once per frame
    void Update()
    {
        // Show meters on screen
        if (textGUI != null) textGUI.text = CalculateMeters().ToString();
    }

    public void Reset()
    {
        meters = 0;
    }

    private float CalculateMeters()
    {
        var position = player.position;
        return meters = Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(position.z, 2) + Mathf.Pow(position.y, 2)) / 5);
    }
}
