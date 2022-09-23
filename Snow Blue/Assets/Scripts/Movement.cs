using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Movement : ResetScript
{
    private float movement;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        velocity.text = Mathf.RoundToInt(rb.velocity.z) + " km/h";
        
        snowParticles.SetActive(isGrounded && rb.velocity.z > 1);
        
        if (!audio.isPlaying && isGrounded && rb.velocity.z > 1)
        {
            audio.Play();
        }

        if (!isGrounded || rb.velocity.z < 1)
        {
            audio.Stop();
        }
        
        if (_hasCrashed) return;
        
        movement = Input.GetAxis("Horizontal");
        
        if (rb.velocity.z > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSpeed);
        }
        if (_startImpulseTimer > 1 && rb.velocity.z < minSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, minSpeed);
        }
    }
    
    void FixedUpdate() 
    {
        if (_hasCrashed) return;
        
        if (_startImpulseTimer <= 1)
        {
            _startImpulseTimer += Time.fixedDeltaTime;
            rb.AddForce(Vector3.forward * startImpulse);
        }
        else
        {
            rb.AddForce(Vector3.forward * 4, ForceMode.Acceleration);
            rb.MovePosition(rb.position + new Vector3(movement, 0) * speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCrashed) return;
        
        if (collision.gameObject.CompareTag("Objects"))
        {
            if (collision.gameObject.GetComponentInParent<AudioSource>() && enabled)
            {
                collision.gameObject.GetComponentInParent<AudioSource>().Play();
            }
            
            _hasCrashed = true;
            menuHandler.SetHighScore();
            menuHandler.Show();

            backgroundMusic.clip = loseMusic;
            backgroundMusic.volume = 0.75f;
            backgroundMusic.Play();
        }
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
            
            backgroundMusic.clip = gameLoopMusic;
            backgroundMusic.volume = 0.25f;
            backgroundMusic.Play();
        }
        _hasCrashed = false;
        _startImpulseTimer = 0;
        rb.velocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
