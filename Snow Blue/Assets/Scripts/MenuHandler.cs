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
    [SerializeField] private PointsManager pointsManager;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    [SerializeField] private bool resetPoints;

    private void Start()
    {
        if (resetPoints)
        {
            PlayerPrefs.DeleteKey("HighScore");
        }
    }

    public void Show()
    {
        gameHUDAnimator.SetTrigger("Show");
        scoreMenuAnimator.SetTrigger("Show");
    }

    public void SetHighScore()
    {
        var points = Mathf.RoundToInt(pointsManager.meters);

        scoreText.text = $"Score: {points.ToString()}";

        var highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        var beatHighScore = false;
        if (points > highScore)
        {
            PlayerPrefs.SetInt("HighScore", points);
            highScore = points;

            beatHighScore = true;
        }

        scoreText.enabled = !beatHighScore;

        var prefix = beatHighScore ? "Novo " : "";

        highScoreText.text = $"{prefix}High Score: {highScore.ToString()}";
    }

    public void GoToStartMenu()
    {
        GetComponent<Menu.Menu>().ChangeScene(startMenuScene);
    }
}
