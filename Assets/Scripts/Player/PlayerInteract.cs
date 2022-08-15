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
    [HideInInspector] public CameraLook cameraLook;
    [HideInInspector]public PlayerController playerController;

    public bool isHolding;
    private IInteractable heldObjectScript;

    private Camera cam;

    public InputManager InputManager;

    private bool activeCrosshair;

    [SerializeField] private PlayerCanvas canvasScript;
    
    //grabbed object
    private Vector3 startPos;
    
    private void Start()
    {
        cam = Camera.main;
        cameraLook = GetComponent<CameraLook>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)),out hit, 100f))
        {
            canvasScript.ActivateCrosshair(hit.collider.gameObject.GetComponent<IInteractable>() != null);
            activeCrosshair = hit.collider.gameObject.GetComponent<IInteractable>() != null;
        }
        if (isHolding && Input.GetKeyDown(KeyCode.E) || activeCrosshair && Input.GetKeyDown(KeyCode.E))
        {
            Interact(hit.collider.gameObject);
        }
    }

    public void Interact(GameObject interactableObject)
    {
        if (!isHolding)
        {
            heldObjectScript = interactableObject.GetComponent<IInteractable>();

            heldObjectScript.Interact(this);
        }
        else
        {
            heldObjectScript.EndInteract();

            heldObjectScript = null;
        }
    }

    public IEnumerator InteractAnim(GameObject interactableObject)
    {
        startPos = interactableObject.transform.position;
        float elapsedTime = 0;
        float waitTime = 0.5f;
        Vector3 currentPos = interactableObject.transform.position;
        while (elapsedTime < waitTime)
        {
            interactableObject.transform.position = Vector3.Lerp(currentPos, ObjectHolder.transform.position, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
 
            yield return null;
        }  
        interactableObject.transform.position = ObjectHolder.transform.position;
        interactableObject.transform.parent = ObjectHolder.transform;
        yield return null;
    }
    
    public IEnumerator StopInteractAnim()
    {
        GameObject heldObject = ObjectHolder.GetComponentInChildren<Transform>().gameObject;
        float elapsedTime = 0;
        float waitTime = 0.5f;
        Vector3 currentPos = heldObject.transform.position;
        while (elapsedTime < waitTime)
        {
            heldObject.transform.position = Vector3.Lerp(currentPos, startPos, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
 
            // Yield here
            yield return null;
        }  
        // Make sure we got there
        heldObject.transform.position = startPos;
        heldObject.transform.parent = null;
        yield return null;
    }
}
