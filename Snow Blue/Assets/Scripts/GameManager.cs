using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int coins;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        coins = PlayerPrefs.GetInt("Coins", 0);
    }

    public void AddCoins(int coins)
    {
        this.coins += coins;
        PlayerPrefs.SetInt("Coins", this.coins);
    }
}
