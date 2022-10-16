using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Movement : ResetScript
{
    private int movement;
    private Rigidbody rb;

    public float speed;

    public float startImpulse;
    private float _startImpulseTimer;
    
    public float maxSpeed;
    public float minSpeed;

    private bool isGrounded;
    public GameObject snowParticles;

    private AudioSource audio;

    public TMP_Text velocity;

    private bool _hasCrashed;

    [SerializeField] private MenuHandler menuHandler;

    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip loseMusic;
    [SerializeField] private AudioClip gameLoopMusic;

    private const int MinRotation = 45;
    private const int MaxRotation = 355;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        velocity.text = Mathf.RoundToInt(rb.velocity.z) + " km/h";

        if (isGrounded && rb.velocity.z > 1)
        {
            snowParticles.SetActive(true);
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
        else
        {
            snowParticles.SetActive(false);
            audio.Stop();
        }

        if (_hasCrashed || _startImpulseTimer <= 1) return;

        movement = GetTouch();

        var rbVelocity = rb.velocity;
        rb.velocity = new Vector3(rbVelocity.x, rbVelocity.y, Mathf.Clamp(rbVelocity.z, minSpeed, maxSpeed));

        var rbRotation = rb.rotation;
        if (rbRotation.eulerAngles.x is >= MinRotation and < 180)
        {
            rb.rotation = Quaternion.Euler(MinRotation, rbRotation.eulerAngles.y, rbRotation.eulerAngles.z);
        }
    
        if (rbRotation.eulerAngles.x is <= MaxRotation and > 180)
        {
            rb.rotation = Quaternion.Euler(MaxRotation, rbRotation.eulerAngles.y, rbRotation.eulerAngles.z);
        }
    }

    private int GetTouch()
    {
        if (Input.touchCount == 0)
        {
            return 0;
        }
        
        var firstTouch = Input.GetTouch(0);

        var halfScreen = (float) Screen.width / 2;
        
        return firstTouch.position.x > halfScreen ? 1 : -1;
    }
    
    private void FixedUpdate() 
    {
        if (_hasCrashed) return;
        
        if (_startImpulseTimer <= 1)
        {
            _startImpulseTimer += Time.fixedDeltaTime;
            rb.AddForce(Vector3.forward * startImpulse);
        }
        else
        {
            rb.AddForce(Vector3.forward * 5, ForceMode.Acceleration);
            rb.MovePosition(rb.position + new Vector3(movement, 0) * speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCrashed || !collision.gameObject.CompareTag("Objects")) return;
        
        collision.gameObject.GetComponentInParent<AudioSource>()?.Play();

        _hasCrashed = true;
        menuHandler.SetHighScore();
        menuHandler.Show();

        PlayBgMusic(loseMusic, 0.75f);
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Floor") || collisionInfo.gameObject.CompareTag("Ramp"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Floor") || other.gameObject.CompareTag("Ramp"))
        {
            isGrounded = false;
        }

        if (other.gameObject.CompareTag("Ramp") && !_hasCrashed)
        {
            rb.AddForce(Vector3.forward, ForceMode.Impulse);
        }
    }

    public override void Reset() {}

    public override void Reset(Vector3 startPosition)
    {
        if (_hasCrashed)
        {
            menuHandler.Show();

            PlayBgMusic(gameLoopMusic, 0.25f);
        }
        _hasCrashed = false;
        _startImpulseTimer = 0;
        rb.velocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void PlayBgMusic(AudioClip music, float volume)
    {
        backgroundMusic.clip = music;
        backgroundMusic.volume = volume;
        backgroundMusic.Play();
    }
}
