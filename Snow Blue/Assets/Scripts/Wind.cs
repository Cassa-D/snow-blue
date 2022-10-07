using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private Rigidbody rbPlayer;
    [SerializeField] private AudioSource windSound;

    // Update is called once per frame
    void Update()
    {
        var velocity = rbPlayer.velocity.z / 40;

        windSound.volume = velocity;

        if (velocity > 0 && !windSound.isPlaying)
        {
            windSound.Play();
        }

        if (velocity == 0)
        {
            windSound.Stop();
        }
    }
}
