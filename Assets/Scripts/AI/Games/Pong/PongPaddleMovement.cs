using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PongPaddleMovement : MonoBehaviour
{
    public InputManager InputManager;
    public bool isAIControlled;

    private Rigidbody2D rb2d;
    private Vector2 movement;
    private float speed = 5f;
    [HideInInspector] public GameObject Ball; 
    void Start()
    {
        if (InputManager == null) isAIControlled = true;
    }
    
    void Update()
    {
        if (!isAIControlled)
        {
            movement = new Vector2(InputManager.MoveInput.x,0) * speed;
            Debug.Log(InputManager.MoveInput.x);
        }
        else
        {
            AIMovment();
        }
    }

    private void AIMovment()
    {
        if (Ball.transform.position.x < transform.position.x - 3)
        {
            movement = new Vector2(-1, 0)* 100f * Time.deltaTime;
        }
        else if (Ball.transform.position.x > transform.position.x + 3)
        {
            movement = new Vector2(1, 0) * 100f * Time.deltaTime;
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)movement;
    }
}
