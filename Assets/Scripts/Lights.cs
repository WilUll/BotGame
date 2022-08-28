using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    private Material material;

    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = new Material(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial = material;
    }

    public void TurnOnLight()
    {
        
    }

    public void TurnOffLight()
    {
        Material offMaterial = new Material(material);
        
    }
}
