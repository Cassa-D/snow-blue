using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float movement;
    private Rigidbody rb;

    public float speed;

    public float startImpulse;
    private float startImpulseTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movement = Input.GetAxis("Horizontal");
    }
    
    void FixedUpdate() 
    {
        if (startImpulseTimer <= 1)
        {
            startImpulseTimer += Time.fixedDeltaTime;
            rb.AddForce(Vector3.forward * startImpulse);
        }
        else
        {
            rb.AddForce(Vector3.forward * 4);
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

    public void Reset(Vector3 startPosition)
    {
        enabled = true;
        startImpulseTimer = 0;
        rb.velocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
