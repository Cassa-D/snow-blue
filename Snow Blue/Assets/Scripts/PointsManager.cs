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
    [CanBeNull] public TMP_Text coinGUI;

    [SerializeField] private int _coins;

    public static PointsManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Show meters on screen
        if (textGUI) textGUI.text = CalculateMeters().ToString();
        if (coinGUI) coinGUI.text = _coins.ToString();
    }

    public void Reset()
    {
        meters = 0;
        _coins = 0;
    }

    private float CalculateMeters()
    {
        var position = player.position;
        return meters = Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(position.z, 2) + Mathf.Pow(position.y, 2)) / 5);
    }

    public void CollectCoin(int coin = 1)
    {
        _coins += coin;
    }

    public void AddCoins()
    {
        GameManager.instance.AddCoins(_coins);
    }
}
