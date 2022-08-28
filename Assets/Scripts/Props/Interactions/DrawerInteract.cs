using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class DrawerInteract : MonoBehaviour, IInteractable
{
    public Vector3 OffsetPosAmount;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isActive = false;
    public AnimationCurve animationCurve;
    bool isAnimating;

    private void Start()
    {
        startPos = transform.position;
        endPos = startPos + OffsetPosAmount;
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
        Vector3 currentPos = transform.position;
        Vector3 posToGoTo = isActive ? startPos : endPos;
        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, posToGoTo, animationCurve.Evaluate(elapsedTime));
            elapsedTime += Time.deltaTime;
 
            yield return null;
        }  
        transform.position = transform.position;
        isAnimating = false;
        yield return null;
    }
}
