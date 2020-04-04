using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building_Camera_Collision_Manager : MonoBehaviour {

    // Each building's main MeshRenderer has this script.
    // It holds the MeshRenderer of its opaque version (itself) and a replacement translucent version so it can swap.

    [SerializeField]
    private MeshRenderer opaqueBuilding = null;

    [SerializeField]
    private MeshRenderer transBuilding = null;

    
    // When the buildings are first instantiated, set them to opaque.
    void Start ()
    {
        SetOpaque();
	}

    // Swap Opaque with Transparent
    public void SetTransparent()
    {
        transBuilding.enabled = true;
        opaqueBuilding.enabled = false;
    }

    // Swap Transparent with Opaque
    public void SetOpaque()
    {
        transBuilding.enabled = false;
        opaqueBuilding.enabled = true;
    }
    
}
