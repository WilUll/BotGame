using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject ObjectHolder;
    public PlayerCanvas playerCanvas;
    
    private Camera cam;
    public bool isHolding;
    private bool crosshairActive;
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0)), out var hit))
        {
            crosshairActive = hit.transform.GetComponent<IInteractable>() != null;
            playerCanvas.ActivateCrosshair(crosshairActive);
        }

        if (isHolding && )
        {
            
        }
        
    }
}
