using System;
using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteract playerInteract);

    void EndInteract();
}
