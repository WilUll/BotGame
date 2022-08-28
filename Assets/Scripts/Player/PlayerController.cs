using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float Speed = 6f;
    [SerializeField] private float MovementMultiplier = 10f;
    [SerializeField] private float Drag = 6f;

    [Header("Camera")]
    public float sensitivity;
    public bool InvertY;

    public GameObject CameraHolder;

    public float minClamp = -80f;
    public float maxClamp = 80f;

    //INPUT VARIABLES
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;

    //Movement Variables
    private Rigidbody rb;
    private Vector3 movement;

    //Look Variables
    private float multiplier = 0.01f;
    private float xRotation;
    private float yRotation;
    private Camera cam;



    private void Awake()
    {
        cam = Camera.main;
        
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];
        lookAction = playerInput.actions["Look"];

        rb = GetComponent<Rigidbody>();
        rb.drag = Drag;
        rb.freezeRotation = true;

        interactAction.performed += Interact;
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out var hit))
        {
            if (hit.transform.GetComponent<IInteractable>() != null)
            {
                hit.transform.GetComponent<IInteractable>().Interact();
            }
        }
    }

    private void Update()
    {
        movement = new Vector3(moveAction.ReadValue<Vector2>().x,0,moveAction.ReadValue<Vector2>().y);

        RotationInput();
        
        CameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0,yRotation,0);
    }

    private void RotationInput()
    {
        yRotation += lookAction.ReadValue<Vector2>().x * sensitivity * multiplier;
        
        float lookInputX = InvertY ? -lookAction.ReadValue<Vector2>().y : lookAction.ReadValue<Vector2>().y;
        
        xRotation -= lookInputX * sensitivity * multiplier;
        
        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);
    }

    private void FixedUpdate()
    {
        MoveCharacter(movement);
    }

    private void MoveCharacter(Vector3 direction)
    {
        Vector3 inputDir = transform.right * direction.x + transform.forward * direction.z;
        
        rb.AddForce(inputDir.normalized * Speed * MovementMultiplier, ForceMode.Acceleration);
    }
}
