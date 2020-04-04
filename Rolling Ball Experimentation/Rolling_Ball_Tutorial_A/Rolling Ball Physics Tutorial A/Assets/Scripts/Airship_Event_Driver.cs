using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship_Event_Driver : MonoBehaviour {

    // See, this is where I find Unity dumb.
    // I have to create a script on the object that has the Animator so the Animation can
    // call an event on it, which can restart the animation when the animation is over.
    // But that same event also has to call the paretnt's Airship_Mover script method
    // PlaceShip() so it can restet the ship's location and rotation
    //  In order to do this, I have to have the animator's script reference the parent

    // I call this an event driver only because this script has to be on the animator
    // for me to call an event, even if the event is going to call something from the parent
    // script (and also trigger in this object, so fair's fair.)

    // We get this so we can call its reset function to start a new ship at a different
    // location/rotation
    [SerializeField]
    private Airship_Mover mover = null;

    [SerializeField]
    private GameObject running_lights = null;

    private Animator anim = null;

    private MeshRenderer mesh = null;

    [SerializeField]
    private ParticleSystem[] lights = null;

    [SerializeField]
    private float speedVariance = 10.0f;

    // Stuff to delay the running lights on a per-ship basis so they don't all blink in sync
    private bool lightsOn = false; // Bool to stop the Update loop
    private float lightDelay = 0.0f; // How much to delay firing VFX of running lights
    private float lightTimer = 0.0f; // Counter in Update that counts up to fire it

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        mesh = GetComponent<MeshRenderer>();
        //    lights = GetComponent<ParticleSystem[]>();




        // Random starts with a given speed, then ADDS a multipler on the same times the random num 0-3
        anim.speed = 2.0f + (Random.value * speedVariance); // .01 is a good base. Now alter it by random

        lightDelay = Random.value * 30.0f;
        //running_lights.SetActive(false);
        lightsOn = false;


    }


    private void FixedUpdate()
    {
        if (lightsOn)
        {
            return;
        }
        else
        {
            lightTimer++;
            if (lightTimer > lightDelay)
            {
                foreach (ParticleSystem light in lights)
                {
                    light.Play();
                }
                //running_lights.SetActive(true);
                lightsOn = true;
            }

        }

    }

    public void TurnOffMesh()
    {
        mesh.enabled = false;
        //running_lights.SetActive(false);
        lightsOn = false;
    }

    public void TurnOnMesh()
    {
        mesh.enabled = true;
        //running_lights.SetActive(true);
        lightsOn = true;

    }

public void RestartAnim()
    {
        anim.speed = 2.0f + (Random.value * speedVariance); // .01 is a good base. Now alter it by random
        mover.RestartAnimation();
        anim.SetTrigger("Start");
        mesh.enabled = true;
    }
}
