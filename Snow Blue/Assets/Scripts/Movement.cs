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

    public AudioClip rigthDash;
    public AudioClip leftDash;

    public TMP_Text velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        
        if (rb.velocity.z > maxSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSpeed);
        }
        if (_startImpulseTimer > 1 && rb.velocity.z < minSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, minSpeed);
        }

        snowParticles.SetActive(isGrounded);

        velocity.text = Mathf.RoundToInt(rb.velocity.z) + " km/h";
    }
    
    void FixedUpdate() 
    {
        if (_startImpulseTimer <= 1)
        {
            _startImpulseTimer += Time.fixedDeltaTime;
            rb.AddForce(Vector3.forward * startImpulse);
        }
        else
        {
            rb.AddForce(Vector3.forward * 4, ForceMode.Acceleration);
            rb.MovePosition(rb.position + new Vector3(movement, 0) * speed * Time.fixedDeltaTime);

            if (movement != 0 && !audio.isPlaying && isGrounded)
            {
                audio.clip = movement > 0 ? rigthDash : leftDash;
                audio.Play();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            if (collision.gameObject.GetComponentInParent<AudioSource>() && enabled)
            {
                collision.gameObject.GetComponentInParent<AudioSource>().Play();
            }
            
            enabled = false;
            snowParticles.SetActive(false);
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

        if (other.gameObject.CompareTag("Ramp"))
        {
            rb.AddForce(Vector3.forward * 2, ForceMode.Impulse);
        }
    }

    public override void Reset() {}

    public override void Reset(Vector3 startPosition)
    {
        enabled = true;
        _startImpulseTimer = 0;
        rb.velocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
