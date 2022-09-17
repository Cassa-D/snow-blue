using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            enabled = false;
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
