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
        if (startImpulse <= 1)
        {
            startImpulse += Time.fixedDeltaTime;
            rb.AddForce(Vector3.forward * 17.5f);
        }
        else
        {
            rb.AddForce(Vector3.forward * 5);
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
}
