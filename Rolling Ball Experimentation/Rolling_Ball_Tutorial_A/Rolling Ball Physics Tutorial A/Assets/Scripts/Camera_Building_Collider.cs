using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Building_Collider : MonoBehaviour {


    // Camera needs to know the Building_Camera_Collision_Manager on the object it has collided with.
    private Building_Camera_Collision_Manager bmm = null;


    private void OnCollisionEnter(Collision other)
    {
        // If the camera collider hits a building, find the Building_Camera_Collision_Manager of what it hit so it can call SetTransparent() on it.
        if (other.gameObject.tag == "Building")
        {
            bmm = other.gameObject.GetComponent<Building_Camera_Collision_Manager>();
            bmm.SetTransparent();
        }
    }

    // This one is hoping to fix the situation where you spawn in already camera-in to a building behind you. Yes, leaving it and re-entering works
    // but shouldn't be required
    private void OnCollisionStay(Collision other)
    {
        // If the camera collider hits a building, find the Building_Camera_Collision_Manager of what it hit so it can call SetTransparent() on it.
        if (other.gameObject.tag == "Building")
        {
            bmm = other.gameObject.GetComponent<Building_Camera_Collision_Manager>();
            bmm.SetTransparent();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        // If the camera collider leaves a building, find the Building_Camera_Collision_Manager of what it hit so it can call SetOpaque() on it.
        if (other.gameObject.tag == "Building")
        {
            bmm = other.gameObject.GetComponent<Building_Camera_Collision_Manager>();
            bmm.SetOpaque();
        }
    }
    
}
