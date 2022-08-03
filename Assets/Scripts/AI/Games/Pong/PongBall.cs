using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PongBall : MonoBehaviour
{
    private Rigidbody2D rb2d;

    [Range(-25f,25f)] private float speed;
    private float xOffset = 0f; 
    private float acceleration = 0.05f;

    private PongController pongController;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (Random.Range(0,100) < 50)
        {
            speed = -15f;
            acceleration = -acceleration;
        }
        else
        {
            speed = 15f;
        }
        pongController = GetComponentInParent<PongController>();
    }



    private void FixedUpdate()
    {
        speed += acceleration;
        
        rb2d.velocity = new Vector2(xOffset, speed);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            speed *= 1.5f;
            speed = -speed;
            acceleration = -acceleration;
            xOffset = -(col.transform.position.x - transform.position.x);
        }
        else
        {
            speed *= 1.1f;
        }
        

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        pongController.ResetBall();
    }
}
