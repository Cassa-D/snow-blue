using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    [SerializeField] private float speed;

    [Header("Cameras")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera rotatingCamera;

    [Header("Player")]
    [SerializeField] private Movement playerScript;

    [Header("Canvas")]
    [SerializeField] private Animator gameUI;

    private void Start()
    {
        mainCamera.gameObject.SetActive(false);
        rotatingCamera.gameObject.SetActive(true);

        playerScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera.gameObject.activeSelf) return;
        
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }

    public void StartGame()
    {
        mainCamera.gameObject.SetActive(true);
        rotatingCamera.gameObject.SetActive(false);
        playerScript.enabled = true;

        gameUI.SetTrigger("Show");
    }

    public void GoToStartMenu()
    {
        mainCamera.gameObject.SetActive(false);
        rotatingCamera.gameObject.SetActive(true);
        playerScript.enabled = false;

        // gameUI.SetTrigger("Show");
    }
}
