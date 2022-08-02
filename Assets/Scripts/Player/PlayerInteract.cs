using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public GameObject ObjectHolder;

    public bool isHolding;
    private IInteractable heldObjectScript;

    private Camera cam;

    public InputManager InputManager;
    
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (!isHolding)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                if (interactable == null) return;
                interactable.Interact(this);
                heldObjectScript = hit.collider.gameObject.GetComponent<IInteractable>();
                transform.rotation = quaternion.identity;
            }
        }
        else
        {
            heldObjectScript.EndInteract();
        }
    }
}
