using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    public InputManager InputManager;

    public float sensitivity;
    
    private Camera cam;

    private float multiplier = 0.01f;
    
    private float xRotation;
    private float yRotation;

    public float minClamp = -80f;
    public float maxClamp = 80f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotationInput();

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0,yRotation,0);
    }

    private void RotationInput()
    {
        xRotation += InputManager.LookInput.x * sensitivity * multiplier;
        yRotation -= InputManager.LookInput.y * sensitivity * multiplier;

        xRotation = Mathf.Clamp(xRotation, minClamp, maxClamp);
    }
}
