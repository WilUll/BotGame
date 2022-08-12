using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] private Image Crosshair;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateCrosshair(bool state)
    {
        Crosshair.color = state ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
    }
}
