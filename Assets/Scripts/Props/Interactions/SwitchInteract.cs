using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInteract : MonoBehaviour, IInteractable
{
    public bool startsOff;
    public bool isButton;
    private bool isAnimating;
    private bool isActive;
    public AnimationCurve animationCurve;

    [Header("Switch Settings")] public Quaternion startRot;
    public Quaternion endRot;

    [Header("Button Settings")] public Vector3 startPos;
    public Vector3 endPos;

    void Start()
    {
        isActive = !startsOff;
        StartCoroutine(isButton ? ButtonInteractAnim() : SwitchInteractAnim());
    }


    public void Interact()
    {
    }

    public void EndInteract()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator SwitchInteractAnim()
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

    public IEnumerator ButtonInteractAnim()
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