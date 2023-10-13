using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrailMove : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = 10f;
    Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    
    void Update()
    {
        direction.x = 0.1f;
        direction.y = 2f;
        rb.position = rb.position + (direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        
    }
}

