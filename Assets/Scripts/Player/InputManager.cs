using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;

    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public Vector2 LookInput { get; private set; } = Vector2.zero;
    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Move.performed += SetMove;
        playerInput.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        
        playerInput.Player.Look.performed += SetLook;
        playerInput.Player.Look.canceled += SetLook;
    }
    
    private void SetMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
    
    private void SetLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }
    
    private void OnEnable()
    {
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }
}
