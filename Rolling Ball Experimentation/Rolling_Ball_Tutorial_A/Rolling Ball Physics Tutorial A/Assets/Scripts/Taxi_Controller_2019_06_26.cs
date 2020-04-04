using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Taxi_Controller_2019_06_26 : MonoBehaviour 
{

    // Character controller
    CharacterController controller;

    // Rigidbody for control
	private Rigidbody rb;

    [Header("Game Manager")]

    [SerializeField]
    private Game_Manager gm = null;

    [Header("Movement")]



    // Is the taxi grounded?
    private bool isGrounded = false;

    // Is the taxi on a pad?
    private bool isAtPad = false;

    // Angle to rotate car based on stick
	private float angle = 0.0f;

	private float moveSideways = 0.0f;
	private float moveForward = 0.0f;
    private float turn = 0.0f;
	//private float jump = 0.0f;
	private float upwardThrust = 0.0f;

    [SerializeField]
    private float upThrustMult = 2.0f;

    [SerializeField]
    private float downThrustMult = 1.0f;

    [SerializeField]
    private float forwardThrustMult = 1.0f;



    [SerializeField]
    private float thrustMultiplier = 20.0f;

    [SerializeField]
    private float torque = 1.0f;

    [SerializeField]
    private float bank = 0.5f;

    private bool invertControl = true;

    private string invertControlString = "Inverted Control";
    




    [Header("Text")]

    [SerializeField]
    private TextMesh angleText = null;

    [SerializeField]
    private TextMesh gasText = null;
          

    [SerializeField]
  private TextMesh velocityText = null;

    [SerializeField]
    private TextMesh collisionText = null;

    [SerializeField]
    private TextMesh invertControlText = null;
    

    private float displayAngle = 0.0f;

	private Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);


    [Header("Gas")]

    [SerializeField]
    private float maxGas = 50.0f;

  
  private float gas = 0.0f;

  [SerializeField]
  private float gasUseRateUpThrust = 0.1f;

  [SerializeField]
  private float gasUseRateDownThrust = 0.05f;

  [SerializeField]
  private float gasUseRateForwardThrust = 0.1f;

  [SerializeField]
  private float gasUseRateRotateThrust = 0.05f;

    [SerializeField]
    private float gasFillRate = 0.1f;

    [SerializeField]
    private float taxiRotationSmoothSpeed = 1.0f;

    public bool hasGas = true;

    [Header("Engines")]
    
    [SerializeField]
    private GameObject[] enginesForward;
    [SerializeField]
    private GameObject[] enginesBackward;
    [SerializeField]
    private GameObject[] enginesUp;
    [SerializeField]
    private GameObject[] enginesDown;
    [SerializeField]
    private GameObject[] enginesLeft;
    [SerializeField]
    private GameObject[] enginesRight;

    [SerializeField]
    private float forwardJetScale = 0.6f;
    [SerializeField]
    private float backJetScale = 0.7f;
    [SerializeField]
    private float turnJetScale = 0.3f;
    [SerializeField]
    private float upJetScale = 0.4f;
    [SerializeField]
    private float downJetScale = 0.3f;
    // These will eventually be used to reset the cab after a crash, as the crash sets these to 0.
    // Soon, Start() will fill these with the given values, then use them to reset later.
    //private float defaultAngularDrag = 0;
    //private float defaultDrag = 0;

    [Header("Collision")]

    [SerializeField]
    private float minCollisionThreshold = 10.0f;

    [SerializeField]
    private GameObject explodeObject = null;

    private bool hasControl = true;

    private float damage = 0.0f;


    //Don't need this.
    //private Vector3[] originalScale = null;

  private void Start()
	{
        gas = maxGas;
		rb = GetComponent<Rigidbody>();
		Physics.sleepThreshold = 8.5F;

    float defaultAngularDrag = rb.angularDrag;
    float defaultDrag = rb.drag;

    //gas = initialGas;


  }



    public bool Controlled
    {
        get { return hasControl; }
    }





    void FixedUpdate()
    {

        // I think I will change the Update loop to call single methods such as:
        // LegacyControl();
        // AdvancedControl();
        // RotateTaxi();
        // UprightTaxi();
        // UpdateUI();

        if (false)
        {
            // This used to have a legacy control in it that is no longer needed.
            // Now I have to fix this to remove that ELSE and just make it work without the if/else
        }

        else
        {


            // Accurate thrust control:
            // - Joystick forward/backward adds a positive or negative force in only the forward direction
            // - Joystick left/right adds and subtracts torque
            // - A button is upward thrust, same as legacy control
            // - B button is downward thrust, in emergencies.

            // Get forwart thrust only
            // !!!! PROBLEM! This really does do only FORWARD thrust, as in not aimed in the direction of the torque
            // SO I have to figure out the torque angle of the object, and perhaps it's simply the Y rotation +90 (since default is -90)
            // And apply thrust in that direction. Probably requires trig to get the X and Y thrust based on angle (ArcTan?)
            float thrust = Input.GetAxis("Vertical");

            // ACTUALLY, I have to instead get the angle of the joystick which I used to use to point the car in legacy control,
            // and sunglasses on Boxxy, and apply thrust in that direction instead.
            //
            //!!!!!! Something is wrong. It is not taking the angle as a 0-360 I think.
            // I think rb.transform.localRotation is the transform rotation of the rigidbody component specifically where I want
            // the transform of the game object, not the ridigbody attached

            angle = rb.transform.eulerAngles.y - 90.0f; // The 90.0f is added because 0 degrees on a trig circle points left, not
            // forward. Yet my car points forward at 0
            displayAngle = angle; // For display only

            // For this scheme we only want forward/backward thrust in the direction of the vehicle
            // Direction of the vehicle is controlled below with torque.



            if (hasControl)
            {
                moveForward = Input.GetAxis("Vertical");


                movement = Quaternion.AngleAxis(angle + 90.0f,
                    Vector3.up * moveForward * thrust * thrustMultiplier * forwardThrustMult )
                    * Vector3.right * thrust * thrustMultiplier;
                rb.AddForce(movement);

                UseGas(gasUseRateForwardThrust * Math.Abs(moveForward));
                if (moveForward >= .01f)
                {
                    FireEngines(enginesForward, Math.Abs(moveForward * forwardJetScale));
                }
                if (moveForward <= -.01f)
                {
                    FireEngines(enginesBackward, Math.Abs(moveForward * backJetScale));
                }
                if (moveForward > -0.01f && moveForward < 0.01f )
                {
                    StopEngines( enginesForward );
                    StopEngines( enginesBackward );
                }

            }




                if (hasControl)
                {
                  // Now get rotation
                  turn = Input.GetAxis("Horizontal");
                  rb.AddTorque(transform.up * torque * turn);
                 rb.AddTorque(transform.right * turn * bank * -1.0f);

                UseGas(gasUseRateRotateThrust * Math.Abs(turn));
                  if (turn >= .1f)
                  {
                      FireEngines(enginesRight, Math.Abs(turn * turnJetScale));
                  }
                  if (turn <= -.1f)
                  {
                      FireEngines(enginesLeft, Math.Abs(turn * turnJetScale));
                  }
                  if (turn < 0.1f && turn > -0.1f )
                  {
                      StopEngines(enginesRight);
                      StopEngines(enginesLeft);
                  }

                }
                else
                {
                    StopEngines( enginesRight );
                    StopEngines( enginesLeft );
                    StopEngines( enginesForward );
                    StopEngines( enginesBackward );
                    StopEngines( enginesUp );
                    StopEngines( enginesDown );
                }
        }


        angleText.text = displayAngle.ToString("0.00");
        gasText.text = gas.ToString("0.00");
        velocityText.text = rb.velocity.y.ToString("0.00");
        

        // Invert Control toggle
        if (Input.GetAxis("Fire1") >= 0.1f && hasControl)
        {
            invertControl = false;
            invertControlString = "Flight Control";
        }


        // Invert Control toggle
        if (Input.GetAxis("Jump") >= 0.01f && hasControl)
        {
            invertControl = true;
            invertControlString = "Inverted Control";
        }

        //Oh, and for now, I'm leaving in the A/Y buttons for up/down, but also adding this:
        invertControlText.text = invertControlString;



  // !!!!!!!!!!!!!!!!! NEXT
  //    GET THE JETS WORKING AS INTENDED! INVERSION FAILS THEM
  // 
  // CLUE: Right now, the stick itself determines which engines fire, not the actual thrust direction as determined by "invertedControl".
  // RIGHT NOW, after putting in some debug text, it seems DOWN in Flight control think
  // 

        upwardThrust = Input.GetAxis("Mouse X");

        // Thrust Upward. Unless Inverted Control. Then Thrust Downward
        if ( upwardThrust >= 0.05f && hasControl )
        {
            if (invertControl)
            {
                ThrustDown(Math.Abs(upwardThrust));
                FireEngines(enginesDown, Math.Abs(upwardThrust) * downJetScale);
                angleText.text = "INV DOWN!" + " " + upwardThrust.ToString();
            }
            else
            {
                ThrustUp(Math.Abs(upwardThrust));
                FireEngines(enginesUp, Math.Abs(upwardThrust) * upJetScale);
                angleText.text = "FLIGHT UP!" + " " + upwardThrust.ToString();
            }
        }


        // Thrust Downward. Unless Inverted Control. Then Thrust Upward
        if (upwardThrust <= -0.05f && hasControl)
        {
            if (invertControl)
            {
                ThrustUp(Math.Abs(upwardThrust));
                FireEngines(enginesUp, Math.Abs(upwardThrust) * upJetScale);
                angleText.text = "INV UP!" + " " + upwardThrust.ToString();
            }
            else
            {
                ThrustDown(Math.Abs(upwardThrust));
                FireEngines(enginesDown, Math.Abs(upwardThrust) * downJetScale);
                angleText.text = "FLIGHT DOWN!" + " " + upwardThrust.ToString();
            }
        }


        // If stick in middle, stop engines
        if (upwardThrust <= 0.05f && upwardThrust > -0.05f)
        {
            StopEngines(enginesDown);
            StopEngines(enginesUp);
        }

        GasUp();
        UprightTaxi();

        // Display damage
        collisionText.text = damage.ToString();

        //gm.scoreText.text = gm.numPadsLandedOn.ToString();
    }





    // !!!!!!!!!!!!!!!!!!!!!! It's just not appearing to fire the thrusters when inverted. I mean only one side fires for each type of control, not both.


    void ThrustUp(float thrustAmount)
    {
        movement = new Vector3(0.0f, thrustAmount * thrustMultiplier * upThrustMult, 0.0f);
        rb.AddForce(movement);
        UseGas(gasUseRateUpThrust * thrustAmount);
        angleText.text = movement.y.ToString();

        //FireEngines(enginesUp, upJetScale * Math.Abs(thrustAmount));
    }



    void ThrustDown(float thrustAmount)
    {
        movement = new Vector3(0.0f, thrustAmount * -1.0f * thrustMultiplier * downThrustMult, 0.0f);
        rb.AddForce(movement);
        UseGas(gasUseRateDownThrust * Math.Abs(thrustAmount));
        angleText.text = movement.y.ToString();

        //FireEngines(enginesDown, downJetScale * Math.Abs(thrustAmount));
    }











    void FireEngines(GameObject[] whichengines, float scale) // ADD SECOND VAR, a scale factor, which gets passed above by the various thrust routines
    {
        for (int i = 0; i < whichengines.Length; i++)
        {
            // First, get default scale for each engine
            //  originalScale[i] = whichengines[ i ].transform.localScale;


            whichengines[i].SetActive(true);
            Vector3 curScale = whichengines[i].transform.localScale;
            whichengines[i].transform.localScale = new Vector3(curScale.x, scale, curScale.z); // !!!!! Update the 1.0f in .y to the value passed to this method
        }
    }

    void StopEngines(GameObject[] whichengines)
    {
        for (int i = 0; i < whichengines.Length; i++)
        {
            whichengines[i].SetActive(false);
        }
    }






















    void GasUp()
    {
        if (isAtPad && Math.Abs(rb.velocity.y) < 0.001 && gas < maxGas)
        {
            gas += gasFillRate;
            hasControl = true;
            damage = 0.0f;
        }

        if (gas > 0.0f)
        {
            hasGas = true;

        }
    }








    /*
  void ResetTaxi()
  {
    hasGas = true;
    rb.rotation = Quaternion.identity;

    rb.angularDrag = defaultAngularDrag;
    rb.drag = defaultDrag;

    gas = maxGas;

  }
  */


    void UseGas( float gasRate )
  {
    gas = gas - gasRate;
    if (gas <= 0)
    {
      hasGas = false;
            LoseControl();
      // Don't forget on reinitialize to push these values back to the rigidbo     

    }
  }


    // Whenever the player enters another object, it checks to see if the player IsColorer (ie: is the character
    // doing the painting? It might not be. It might be pushing other painters.
    // And if IsColorer, then set the tag to Painter, as each tile has a script on it that colors itself
    // if it meets a Painter


    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }

    }

    void OnCollisionEnter(Collision other)
    {
        float collisionEffect = other.relativeVelocity.magnitude;

        if (collisionEffect > 0.0f)
        {
            
            Debug.Log("<color=red>COLLISION! " + collisionEffect + " for accumulated damage of " + damage + ".</color>");
        }

        if (collisionEffect > minCollisionThreshold)
        {
            damage++;


            // SILLY THING: try to create a sphere where the two objects collide
            // DAMN! Right now, the thing freaks out by making way too many spheres.
            // PUT A VFX IN INSTEAD THAT IS SELF-KILLING
            // --- BUT FOR NOW: Create a new sphere, but turn off its collider, because that crashes the game causing cascading collisions/creations
            // So get the collider component, and disable it.
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject.Instantiate(explodeObject);
            explodeObject.transform.position = other.contacts[0].point;
            Vector3 splodeScale = new Vector3(collisionEffect * 0.05f, collisionEffect * 0.05f, collisionEffect * 0.05f);
            explodeObject.transform.localScale = splodeScale;
            //Collider sphereCollider = sphere.GetComponent(typeof(Collider)) as Collider;
            //sphereCollider.enabled = false;
            //sphere.transform.position = other.contacts[0].point;

        }

        // If damage reaches too much, do something cool. Right now I just remove the gas, so it will fall.
        if (damage > 50.0f)
        {
            LoseControl();
        }
            

        if (other.gameObject.tag == "GasStation")
        {
            isAtPad = true;
        }
        else
        {
            isAtPad = false;
        }


        if (other.gameObject.tag == "Pad")
        {
            gm.numPadsLandedOn++;
        }
        


    }

    // !!!! HERE
    // Now that I have velocity checked, I want to set it up so if it collides with anything, and the velocity is > X, it does something bad.
    // I will want to check for Y velocity for up/down, and forward velocity for crashes into sides of things. Both should do things, but would have different thresholds of crash

    // This is only called in legacy control. In regular control rotation is based on torque using left/right stick
    void Rotate_Taxi()
    {
	    // Now put the glasses on the ball, pointed towards the thrust direction of the controller
	    if (moveForward == 0.0f && moveSideways == 0.0f) 
	    {
		    return;
	    }
	    else
	    {
		    angle = ((float)Math.Atan2 (moveSideways, moveForward) * 180.0f / Mathf.PI) - 90.0f;
		    transform.rotation = Quaternion.AngleAxis ((float)angle * -1.0f, Vector3.up);
        }
    }

    void UprightTaxi()
    {

        if (!hasControl)
        {
            return;
        }
        taxiRotationSmoothSpeed = 1.0f; // Taxi Upright Speed

        // Now to set up the target rotation as current Y and X=0, Y=0, which should force it to "constrain" back in X and Z
        // First, we want to get the current Y, because that should not change.
        float currentTaxiYRotation = transform.localRotation.eulerAngles.y;

        // Now make a Vector3 of the desired rotation
        Vector3 desiredTaxiEulerRotation = new Vector3(0.0f, currentTaxiYRotation, 0.0f);

        Quaternion desiredTaxiRotation = Quaternion.Euler(desiredTaxiEulerRotation);
        //Quaternion desiredRotation = transform.localRotation;
        Quaternion smoothedTaxiRotation = Quaternion.Lerp(transform.localRotation, desiredTaxiRotation, taxiRotationSmoothSpeed * Time.deltaTime);
        transform.rotation = smoothedTaxiRotation;
    }




    void LoseControl()
    {
        hasControl = false;
        rb.constraints = RigidbodyConstraints.None;

        rb.angularDrag = 0.0f;
        rb.drag = 0.0f;

        // THIS SECTION PUTS A RANDOM SPIN ON THE CAR IF YOU LOSE CONTROL
        // PUT IT BACK?
        float RandXThrust = UnityEngine.Random.value;
        Vector3 StartMovement = new Vector3(((UnityEngine.Random.value - .5f) * 4), 0.0f, (Math.Abs(UnityEngine.Random.value) * -8));
        rb.AddTorque(StartMovement); // 400 is the intensity multiplier on the random direction in the previous line
        
    }

}
