using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private GameObject model;
    [InspectorLabel("Particle System")][SerializeField] private ParticleSystem pS;

    public void Collect()
    {
        pS.Play();
        model.SetActive(false);
        
        Destroy(gameObject, 0.2f);
    }
}
