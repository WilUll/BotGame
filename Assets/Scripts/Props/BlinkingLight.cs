using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    [Tooltip("If Using Emission Material is checked, add materials ")]
    [SerializeField] private bool usingEmissionMaterial;
    [SerializeField] private Material OnMaterial;
    [SerializeField] private Material OffMaterial;
    [SerializeField] private float blinkTimer;

    private float currentTime;

    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > blinkTimer)
        {
            StartCoroutine(Blink());
            currentTime = 0;
        }
    }

    IEnumerator Blink()
    {
        meshRenderer.sharedMaterial = OnMaterial;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.sharedMaterial = OffMaterial;

    }
}
