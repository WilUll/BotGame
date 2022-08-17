using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Cable : MonoBehaviour
{
    public Transform[] cablePositions;

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        Vector3[] positions = new Vector3[cablePositions.Length];
        for (int i = 0; i < positions.Length -1; i++)
        {
            positions[i] = cablePositions[i].position;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
