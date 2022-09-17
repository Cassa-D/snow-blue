using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public float meters;
    public Transform player;

    public TMP_Text textGUI;

    // Update is called once per frame
    void Update()
    {
        // Show meters on screen
        textGUI.text = CalculateMeters().ToString();
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
