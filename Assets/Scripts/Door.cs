using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private Quaternion startRot;
    private Quaternion endRot;
    public float yRot;

    float currentYRot;

    public bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        startRot = transform.localRotation;
        endRot = quaternion.Euler(270, 270,endRot.eulerAngles.z);
    }

    private void Update()
    {
    }

    public void Interact(PlayerInteract playerInteract)
    {
        StartCoroutine(PlayAnimation());
        isOpen = !isOpen;
    }

    IEnumerator PlayAnimation()
    {
        float t = 0;
        float angle = yRot / 100;
        if (!isOpen)
        {
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.01f);
                transform.Rotate(Vector3.up, angle, Space.World);
                t += 0.01f;
            }
        }
        else
        {
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.01f);
                transform.Rotate(Vector3.up, -angle, Space.World);
                t += 0.01f;
            }
        }
    }

    public void EndInteract()
    {
        
    }
}
