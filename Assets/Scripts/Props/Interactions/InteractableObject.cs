using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    private PlayerInteract playerInteractScript;
    public void Interact(PlayerInteract playerInteract)
    {
        playerInteractScript = playerInteract;
        StartCoroutine(playerInteractScript.InteractAnim(gameObject));
        playerInteractScript.isHolding = true;

        playerInteractScript.cameraLook.isInteracting = true;
        playerInteractScript.playerController.enabled = false;

    }

    public void EndInteract()
    {
        StartCoroutine(playerInteractScript.StopInteractAnim());
        playerInteractScript.isHolding = false;

        playerInteractScript.playerController.enabled = true;
        playerInteractScript.cameraLook.isInteracting = false;
    }
}
