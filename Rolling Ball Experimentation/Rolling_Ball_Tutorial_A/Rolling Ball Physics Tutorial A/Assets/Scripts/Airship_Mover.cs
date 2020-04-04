using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Airship_Mover : MonoBehaviour {



    [SerializeField]
    public GameObject rotator = null;

    [SerializeField]
    public float airshipHeight = 10.0f;

    public bool collided = false;

    [SerializeField]
    public Animator anim = null;

    
    private RaycastHit hit;



    // Pass in the position to try placing the ship, and its index in the array of ahirships.
    // We need to know the rotation so the sphere cast can work in the right direction.
    // But it may be hard to determine that without knowing i, the index of the ship array
    // so we can rotate it here
    public void PlaceShip(Vector3 pos, int i)
    {
        // Distance to cast the sphere cast. LONG
        float distanceToCast = 10000.0f;

        // Rotate each ship 90 degrees more than previous in the passing array
        float rot = i * 90.0f;

        Vector3 castRotation = Vector3.forward; // Set up our cast rotation to forward
        // So we can, at every 90 degree turn, change it to left, then back to forward.
        
        // Just a stat to see how often we have to move the ship before it clears the buildings
        int tries = 0;

        // Variable to end the While loop. If a ship has cleared buildings, it is set to true
        bool clearedBuildings = false;

        // This code does two things:
        // It fixes castRotation so the SphereCast goes outward in the right direction
        // And it puts every odd ship one airshipHeight higher, so the cross grid can never collide.
        if (i % 2 == 1) // Check which ship in the array, mod it by 2 to see if odd or even, so we can rotate the SphereCast too
                        
        {
            castRotation = Vector3.right;

        }
        else
        {
            castRotation = Vector3.forward;
            pos.y = pos.y + airshipHeight;
        }

        // Place ship at first position. This will update in the while loop if it collides.
        transform.position = pos;

        // Rotate ship based on this particular ship in the array's Y rotation, which was passed
        // in to this method by Game_Manager. Odd ships rotate 90 degrees from the last
        rotator.transform.eulerAngles = new Vector3(0.0f, rot, 0.0f);

        // After placing the ship, we check to see if clearedBuildings is false. If it is, we
        // lift the ship up 2 heighs and try again. If it succeeds, it sets clearedBuildings to true
        // thus ending the loop
        while (clearedBuildings == false)
        {
            // Try placing the ship.
            // We place it at pos, and we rotate it so each subsequent one is 90 degrees more
            // for a nice bidirectional grid
            transform.position = pos;

            // Set ship rotation to rot
            rotator.transform.eulerAngles = new Vector3(0.0f, rot, 0.0f);

            // SphereCast forward at pos, radius height, at the proper rotation. Give us hit back. Test for distanceToCast
            if (Physics.SphereCast(pos, airshipHeight, castRotation, out hit, distanceToCast))
            {
                // If it hits something, rais the ship up and try again. Keep track of tries for our records.
                pos = new Vector3(transform.position.x, transform.position.y + (airshipHeight * 2.0f), transform.position.z);
                tries++;
            }
            else
            {
                // Else means it did NOT collide. Which means we're in the clear.
                // We can stop raising the ship, and set the clearedBuilding flag to true which ends the while loop
                clearedBuildings = true;
            }

        }
        // Debug.Log("<color=red>Setting clearedBuildings to true after " + tries + " tries.</color>");
        
        // Restart the animation.
        RestartAnimation();
    }

    // Tells the ship to restart. 
    // This is its own method becuase Airship_Event_Driver needs to reset the ship
    // when the animation gets to its end.
    public void RestartAnimation()
    {
        enabled = true;
        anim.Play("Airship_Move_Z", -1, Random.value);
    }
}
