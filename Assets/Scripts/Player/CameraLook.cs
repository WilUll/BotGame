using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    public InputManager InputManager;

    public float sensitivity;
    public bool InvertY;
    
    public GameObject CameraHolder;

    private float multiplier = 0.01f;
    
    private float xRotation;
    private float yRotation;

    public float minClamp = -80f;
    public float maxClamp = 80f;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotationInput();

        CameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0,yRotation,0);
    }

    private void RotationInput()
    {
        yRotation += InputManager.LookInput.x * sensitivity * multiplier;
        
        float lookInputX = InvertY ? -InputManager.LookInput.y : InputManager.LookInput.y;
        
        xRotation -= lookInputX * sensitivity * multiplier;
        
        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);
    }
}
