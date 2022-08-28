using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    public Quaternion OffsetRotAmount;
    private Quaternion startRot;
    private Quaternion endRot;
    private bool isActive = false;
    public AnimationCurve animationCurve;
    bool isAnimating;

    private void Start()
    {
        startRot = transform.localRotation;
        endRot = startRot * OffsetRotAmount;
    }

    public void Interact()
    {
        if (!isAnimating)
        {
            StartCoroutine(InteractAnim());
            isActive = !isActive;
        }
    }

    public void EndInteract()
    {
        
    }
    
    public IEnumerator InteractAnim()
    {
        isAnimating = true;
        float elapsedTime = 0;
        float waitTime = 1f;
        Quaternion currentRot = transform.localRotation;
        Quaternion rotToGo = isActive ? startRot : endRot;
        while (elapsedTime < waitTime)
        {
            transform.rotation = Quaternion.Lerp(currentRot, rotToGo, animationCurve.Evaluate(elapsedTime));
            elapsedTime += Time.deltaTime;
 
            yield return null;
        }  
        transform.position = transform.position;
        isAnimating = false;
        yield return null;
    }
}
