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
    public ParticleSystem snowParticles;

    private AudioSource audio;

    public TMP_Text velocity;

    private bool _hasCrashed;

    [SerializeField] private MenuHandler menuHandler;

    [SerializeField] private LoopSounds loseSounds;
    [SerializeField] private LoopSounds gameSounds;

    private const float MinRotation = 30.25f;
    private const float MaxRotation = 355;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        velocity.text = Mathf.RoundToInt(rb.velocity.z * 1.5f) + " km/h";

        if (isGrounded && rb.velocity.z > 1)
        {
            // snowParticles.SetActive(true);
            if (!snowParticles.isPlaying)
            {
                snowParticles.Play();
            }
            if (!audio.isPlaying)
            {
                audio.Play();
            }
        }
        else
        {
            // snowParticles.SetActive(false);
            if (snowParticles.isPlaying)
            {
                snowParticles.Stop();                
            }
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
            rb.AddForce(Vector3.forward * 8, ForceMode.Acceleration);
            rb.MovePosition(rb.position + new Vector3(movement, 0) * (Time.fixedDeltaTime * (speed + rb.velocity.z / 4)));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCrashed || !collision.gameObject.CompareTag("Objects")) return;
        
        PointsManager.instance.AddCoins();
        
        collision.gameObject.GetComponentInParent<AudioSource>()?.Play();

        _hasCrashed = true;
        menuHandler.SetHighScore();
        menuHandler.Show();

        gameSounds.enabled = false;
        loseSounds.enabled = true;
        loseSounds.SwitchMusic(0, 0.75f);
        
        PointsManager.instance.Reset();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            PointsManager.instance.CollectCoin();
            other.gameObject.GetComponent<Coin>().Collect();
        }
    }

    public override void Reset() {}

    public override void Reset(Vector3 startPosition)
    {
        if (_hasCrashed)
        {
            gameSounds.enabled = true;
            loseSounds.enabled = false;
            gameSounds.SwitchMusic(0, 0.5f);
        }
        _hasCrashed = false;
        _startImpulseTimer = 0;
        rb.velocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
