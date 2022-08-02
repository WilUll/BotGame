using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    
    private Vector3 movement;
    private Vector2 rotation;

    private float upDownRot;

    public InputManager InputManager;
    
    [Header("Movement")]
    public float Speed = 6f;
    public float MovementMultiplier = 10f;
    public float RbDrag = 6f;
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = RbDrag;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        movement = new Vector3(InputManager.MoveInput.x,0,InputManager.MoveInput.y);
    }

    private void FixedUpdate()
    {
        MoveCharacter(movement);
    }

    // private void RotateCharacter(Vector2 rotation)
    // {
    //     //UP DOWN ROT
    //     upDownRot -= rotation.y;
    //     upDownRot = Mathf.Clamp(upDownRot, -80f, 80);
    //     CameraHolder.transform.localRotation = Quaternion.Euler(upDownRot,0,0);
    //     
    //     Player.transform.Rotate(Vector3.up * rotation.x);
    // }

    private void MoveCharacter(Vector3 direction)
    {
        Vector3 inputDir = new Vector3(direction.x, 0f, direction.z);

        inputDir = transform.right * direction.x + transform.forward * direction.z;
        
        rb.AddForce(inputDir.normalized * Speed * MovementMultiplier, ForceMode.Acceleration);


        //rb.MovePosition(transform.forward + (direction * speed * Time.deltaTime));
    }
}
