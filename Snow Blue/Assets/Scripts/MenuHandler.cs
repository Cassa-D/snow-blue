using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [Header("Menu Manager")]
    [SerializeField] private Animator gameHUDAnimator;
    [SerializeField] private Animator scoreMenuAnimator;

    [SerializeField] private string startMenuScene;
    
    [Header("Points Manager")]
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text totalCoinsText;

    [SerializeField] private bool resetPoints;

    private void Start()
    {
        if (resetPoints)
        {
            PlayerPrefs.DeleteKey("HighScore");
            PlayerPrefs.DeleteKey("Coins");
        }
    }

    public void Show()
    {
        gameHUDAnimator.SetTrigger("Show");
        scoreMenuAnimator.SetTrigger("Show");
    }

    public void SetHighScore()
    {
        var distance = Mathf.RoundToInt(PointsManager.instance.meters);
        var coins = Mathf.RoundToInt(PointsManager.instance.coins);

        distanceText.text = $"Distância percorrida: {distance.ToString()}";
        coinsText.text = $"Moedas coletadas: {coins.ToString()}";

        var highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        var beatHighScore = false;
        if (distance > highScore)
        {
            PlayerPrefs.SetInt("HighScore", distance);
            highScore = distance;

            beatHighScore = true;
        }

        distanceText.enabled = !beatHighScore;

        var prefix = beatHighScore ? "Novo " : "";

        highScoreText.text = $"{prefix}High Score: {highScore.ToString()}";
        totalCoinsText.text = $"Total de moedas: {GameManager.instance.GetCoins()}";
    }

    public void GoToStartMenu()
    {
        GetComponent<Menu.Menu>().ChangeScene(startMenuScene);
    }
}
