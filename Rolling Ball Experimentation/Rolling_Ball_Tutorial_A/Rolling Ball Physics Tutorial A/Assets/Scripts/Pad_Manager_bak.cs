using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad_Manager_bak : MonoBehaviour {


    // Tells the game manager it's the next target pad
    [HideInInspector]
    public bool isNext = false;

    // Tells the game manager which pad this is
    [HideInInspector]
    public int padNumber = 0;

    // Lights up the beam
    [HideInInspector]
    public bool isLit = false;

    // Has this pad been touched yet?
    [HideInInspector]
    public bool isTouched = false;

    [SerializeField]
    private Game_Manager gm = null;

    [SerializeField]
    private GameObject lightBeam = null;

    


	// Use this for initialization
	void Start () {
		
	}
	



    public void LightBeam(bool flag)
    {
        lightBeam.SetActive(flag);
    }
    

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Player")
        {
            if (isTouched)
            {
                //Debug.Log("****************** BEEN TOUCHED ALREADY!");
                return;
            }
            else
            {
                
                if (padNumber == gm.nextPad)
                {
                    // In here I need to put a variable that I'm on the pad, which is sent to an update loop that times down. When done, sets isTouched = true. If false, 
                    isTouched = true;
                    Debug.LogWarning(" **** TOUCHED PAD " + padNumber );
                    gm.Advance();
                    Debug.LogWarning(" **** MOVING ON to pad " + gm.nextPad);
                }
                else
                {
                    isTouched = false;
                    Debug.LogWarning("THIS IS NOT THE TARGET PAD! Try again! Target is " + gm.nextPad + " but this is pad " + padNumber );
                }
             
            }
            
        }
        else
        {
            //Debug.LogWarning("Uh-oh. Something hit me, but I didn't recognize it as player.");
        }
       

    }




}
