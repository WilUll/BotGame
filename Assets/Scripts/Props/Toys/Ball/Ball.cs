using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private Collider collider;

    private PlayerInteract playerInteractScript;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void Interact(PlayerInteract playerInteract)
    {
        playerInteractScript = playerInteract;
        Debug.Log("Interacted");
        playerInteractScript.isHolding = true;
        rb.isKinematic = true;
        transform.parent = playerInteractScript.ObjectHolder.transform;
        collider.enabled = false;
        transform.localPosition = Vector3.zero;
    }

    public void EndInteract()
    {
        playerInteractScript.isHolding = false;
        rb.isKinematic = false;
        transform.parent = null;
        collider.enabled = true;
    }
}
