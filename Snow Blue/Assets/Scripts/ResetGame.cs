using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public abstract class ResetScript : MonoBehaviour
{
    public abstract void Reset();
    public abstract void Reset(Vector3 sP);
}

public class ResetGame : MonoBehaviour
{
    public ResetScript[] resetScripts;
    private Vector3 _startPosition;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        _startPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r") || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            foreach (var script in resetScripts)
            {
                script.Reset();
                script.Reset(_startPosition);
            }
        }
    }
}
