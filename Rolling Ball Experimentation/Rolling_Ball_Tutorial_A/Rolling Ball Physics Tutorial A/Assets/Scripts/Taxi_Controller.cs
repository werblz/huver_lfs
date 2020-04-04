using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class Taxi_Controller : MonoBehaviour 
{

    // Character controller
    CharacterController controller;

    // Rigidbody for control
	public Rigidbody rb;

    [Header("Game Manager")]

    [Tooltip("Need reference to Game Manager. Nothing gets done otherwise.")]
    [SerializeField]
    private Game_Manager gm = null;

    [Tooltip("Need reference for Radar Manager to tell it to show cracked glass")]
    [SerializeField]
    private Radar_Manager rm = null;

    [Header("Movement")]

    [SerializeField]
    private bool invertFlightControl = false;

    // Is the taxi grounded?
    private bool isGrounded = false;

    // Is the taxi on a pad?
    private bool isAtPad = false;

    // Is the taxi on Home pad?
    private bool isHome = false;

    // Angle to rotate car based on stick
	private float angle = 0.0f;

	private float moveSideways = 0.0f;
	private float moveForward = 0.0f;
    private float turn = 0.0f;
	//private float jump = 0.0f;
	private float upwardThrust = 0.0f; // SAVE

    [SerializeField]
    private float upThrustMult = 2.0f; // SAVE

    [SerializeField]
    private float downThrustMult = 1.0f; // SAVE

    [SerializeField]
    private float forwardThrustMult = 1.0f; // SAVE

    [SerializeField]
    public float sideThrustMult = 1.0f; // SAVE

    [SerializeField]
    private float thrustMultiplier = 20.0f; // SAVE

    [SerializeField]
    public float turboMultiplier = 1.0f; // SAVE

    private float turboAmount = 1.0f; // SAVE

    [HideInInspector]
    public float turboTrigger = 0.0f;

    [SerializeField]
    private float torque = 1.0f;

    [SerializeField]
    private float bank = 0.5f;

    [SerializeField]
    public float taxiRotationSmoothSpeed = 1.0f;

    private bool invertControl = true;

    private string invertControlString = "Inverted Control";


    private string forwardJoy = "Vertical"; // Default "Vertical"
    private string turnJoy = "Mouse Y";  // Default "Horizontal"
    private string sideJoy = "Horizontal";     // Default "Mouse Y"
    private string verticalJoy = "Mouse X"; // Default "Mouse X"
    private string turboButton = "Left Trigger"; // was Left Bumper

  [SerializeField]
  private float joyToleranceMax = 0.5f;
  [SerializeField]
  private float joyToleranceMin = -0.5f;


  [Header("Text")]


    [SerializeField]
    private TextMesh gasText = null;

    public bool isCrashing = false;

    public bool isDead = false;
          

    [SerializeField]
  private TextMesh velocityText = null;

    [SerializeField]
    private TextMesh collisionText = null;

    [SerializeField]
    private TextMesh invertControlText = null;



	private Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);




    [Header("Gas")]

    [SerializeField]
    public float maxGas = 50.0f; // SAVE

  
  public float gas = 0.0f; // SAVE

  [SerializeField]
  private float gasUseRateUpThrust = 0.1f; // SAVE

  [SerializeField]
  private float gasUseRateDownThrust = 0.05f; // SAVE

  [SerializeField]
  private float gasUseRateForwardThrust = 0.1f; // SAVE

  [SerializeField]
  private float gasUseRateRotateThrust = 0.05f; // SAVE

    [SerializeField]
    private float gasUseRateSideThrust = 0.05f; // SAVE





    public bool hasGas = true;
    public bool hasControl = true;

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
    private GameObject[] enginesSideLeft;
    [SerializeField]
    private GameObject[] enginesSideRight;
    [SerializeField]
    private GameObject[] enginesTurboRear;
    [SerializeField]
    private GameObject[] enginesTurboFront;


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
    [SerializeField]
    private float sideJetScale = 0.3f;

    // Keep track of these because we have to reset it after a crash, but upgrades may change them
    public float defaultAngularDrag = 0; // SAVE
    public float defaultDrag = 0; // SAVE

    [Header("Collision")]

    [SerializeField]
    public float minCollisionThreshold = 10.0f; // SAVE

    [SerializeField]
    public float shieldPercent = 1.0f; // This is the percentage shield added

    [SerializeField]
    public float maxDamage = 50.0f; // SAVE

    [SerializeField]
    private GameObject explodeObject = null;


    [HideInInspector]
    public float damage = 0.0f; // SAVE

    [HideInInspector]
    public bool taxiMovedToInitialLocation = false;


    [HideInInspector]
    public bool cameraFollow = true;


    [Header("Upgrades")]

    [SerializeField]
    public bool hasHomeIndicator = false;

    [SerializeField]
    public bool hasNextIndicator = false;

    [SerializeField]
    public bool hasStationIndicator = false;
    
    [SerializeField]
    public bool hasStrafe = false;

    [SerializeField]
    public bool hasTurbo = false;





    //Don't need this.
    //private Vector3[] originalScale = null;

    private void Start()
	{

        taxiMovedToInitialLocation = false;

        gas = maxGas;
		rb = GetComponent<Rigidbody>();
		Physics.sleepThreshold = 8.5F;

        defaultAngularDrag = rb.angularDrag;
        defaultDrag = rb.drag;

        //gas = initialGas;



        // Switch in Inspector to invert control ON START ONLY
        if (invertFlightControl)
        {
            invertControl = false;
            invertControlString = "Inverted Control";

        }

       


    }



    public bool Controlled
    {
        get { return hasControl; }
    }


    // This may be entirely unnecessary in LateUpdate. Perhaps put it on Awake or Enable, and make Game Manager turn the taxi on.
    private void LateUpdate()
    {
        if (taxiMovedToInitialLocation == false)
        {
            MoveTaxiToStartPad();
        }

        //taxiMovedToInitialLocation = true;
    }

    
    public void MoveTaxiToStartPad()
    {
        cameraFollow = false;

        // Place taxi on starting gas pad (for now. Home later?)
        //
        // Yes, now is the time to place it at home.
        // To do that, I have to change Game_Manager to add the Home pad
        // Place it near the center of the city, adapt the building x/z scale to fit the pad
        // and then use that pad's transform position here instead of Gas 0.

        /* THIS IS THE OLD ONE, PLACING TAXI ON GAS STATION. BEFORE HOME WENT IN
        Vector3 taxiStartPos = new Vector3(gm.stations[0].transform.position.x + .1f,
                gm.stations[0].transform.position.y + 2.0f,
                gm.stations[0].transform.position.z); // the .1f offset in the X is to ensure the gas radar doesn't jiggle, because it's at exactly zero position relative
        rb.position = taxiStartPos;
        rb.transform.localRotation = Quaternion.identity;
        */



        Vector3 taxiStartPos = new Vector3(gm.homeBldg.transform.position.x - .1f,
                gm.homeBldg.transform.position.y + 2.0f,
                gm.homeBldg.transform.position.z); // the .1f offset in the X is to ensure the gas radar doesn't jiggle, because it's at exactly zero position relative
        rb.position = taxiStartPos;
        rb.transform.localRotation = Quaternion.identity;



        // FOr some reason my taxi often starts off and jumps because I believe I have moved it which makes it think it's on a trajectory.
        // So zero out the velocity at the start.




        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        //Debug.Log("<color=red>*****************</color> I HAVE MOVED THE TAXI TO " + taxiStartPos.ToString());

        taxiMovedToInitialLocation = true;
        


    }

    void FixedUpdate()
    {

        // I think I will change the Update loop to call single methods such as:
        // LegacyControl();
        // AdvancedControl();
        // RotateTaxi();
        // UprightTaxi();
        // UpdateUI();

        /*
        // THIS WHOLE SECTION GOES!
        // This is temporary, using the left trigger to end UI panel. Eventually I will move that to one of the four lettered buttons, on buttontrigger or something
        turboTrigger = Input.GetAxis(turboButton);
        // For now, to test, take UI down when you hit the left trigger. This goes away soon.
        if (turboTrigger > 0.01f)
        {
            Debug.Log("<color=green>****</color> Pulling UI down with Turbo Trigger.");
            gm.UiDown();
        }

        */



        // I guess first, I should check to see if I've crashed, and am dead on the ground somewhere

        // First, have I gone through the ground? This is the extreme case where you fall through or hit the ground. Dead.
        /*
        if (transform.position.y < 0.0f)
        {
            Debug.Log("<color=red> YOU HAVE BUST THROUGH THE GROUND!</color>");
            gm.RestartShift();
        }
        */
        

        // Have I crashed and stopped somewhere above ground?
        // IS THIS MY PROBLEM? That I'm doing this for normal crashing, that is, crashing and stopping above-ground
        // but also if you just happen to drop below city ground level (abandonment)
        // Perhaps it was that the ground was wet to 0. Perhaps I need to lower that ground level for simple abandomnent
        if ( (isCrashing && rb.velocity == Vector3.zero) )
        {
            
            gm.RestartShift();
        }

        if (transform.position.y < 10.0f)
        {
            gm.cash += gm.crashDeductible / 2.0f;
            gm.RestartShift();
        }
        


        // 
        if (Input.GetAxis("Fire2") >= 0.1f)
        {

            Debug.Log("<color=green>****</color> Pulling UI down with B button.");
            gm.PullUiDown();
            


        }







        if (gm.uiIsUp)
        {
            isCrashing = false;
            
            // If the UI is up, don't do anything in the update loop. This should only happen when you're parked at home at the beginning of a shift
            return;
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
            //float thrust = Input.GetAxis("Vertical");

            // ACTUALLY, I have to instead get the angle of the joystick which I used to use to point the car in legacy control,
            // and sunglasses on Boxxy, and apply thrust in that direction instead.
            //
            //!!!!!! Something is wrong. It is not taking the angle as a 0-360 I think.
            // I think rb.transform.localRotation is the transform rotation of the rigidbody component specifically where I want
            // the transform of the game object, not the ridigbody attached

            angle = rb.transform.eulerAngles.y - 90.0f; // The 90.0f is added because 0 degrees on a trig circle points left, not
            // forward. Yet my car points forward at 0

            // For this scheme we only want forward/backward thrust in the direction of the vehicle
            // Direction of the vehicle is controlled below with torque.



            if (hasControl)
            {
                moveForward = Input.GetAxis(forwardJoy);
                if (moveForward > joyToleranceMin && moveForward < joyToleranceMax)
                {
                    moveForward = 0.0f;
                }

                

                movement = Quaternion.AngleAxis(angle + 90.0f,
                    Vector3.up * moveForward * moveForward * thrustMultiplier * forwardThrustMult * turboAmount)
                    * Vector3.right * moveForward * thrustMultiplier * turboAmount;
                rb.AddForce(movement);

                UseGas(gasUseRateForwardThrust * Math.Abs(moveForward) * turboAmount);
                if (moveForward >= joyToleranceMax )
                {
                    FireEngines(enginesForward, Math.Abs(moveForward * forwardJetScale));
                    if (turboTrigger > 0.01f && hasTurbo) // IF it's being thrust forward AND Turbo is on, fire turbo visuals at scale 1.
                    {
                        FireEngines(enginesTurboFront, 0.5f);
                    }
                    else
                    {
                        StopEngines(enginesTurboFront);
                    }
                }
                if (moveForward <= joyToleranceMin )
                {
                    FireEngines(enginesBackward, Math.Abs(moveForward * backJetScale));
                    if (turboTrigger > 0.01f && hasTurbo) // IF it's being thrust forward AND Turbo is on, fire turbo visuals at scale 1.
                    {
                        FireEngines(enginesTurboRear, 0.5f);
                    }
                    else
                    {
                        StopEngines(enginesTurboRear);
                    }
                }
                if (moveForward > joyToleranceMin && moveForward < joyToleranceMax )
                {
                    StopEngines( enginesForward );
                    StopEngines( enginesBackward );
                    StopEngines(enginesTurboFront);
                    StopEngines(enginesTurboRear);
                }

            }



            if (hasControl)
            {

                turboTrigger = Input.GetAxis(turboButton);

                if (turboTrigger > 0.01f && hasTurbo)
                {
                    turboAmount = turboMultiplier;
                    
                }
                else
                {
                    turboAmount = 1.0f;
                }
            }



            if (hasControl)
                {
                  // Now get rotation
                  turn = Input.GetAxis(turnJoy);

        if ( turn > joyToleranceMin && turn < joyToleranceMax )
        {
          turn = 0.0f;
        }

        rb.AddTorque(transform.up * torque * turn);
                 rb.AddTorque(transform.right * turn * bank * -1.0f);

                UseGas(gasUseRateRotateThrust * Math.Abs(turn));
                  if (turn >= joyToleranceMax )
                  {
                      FireEngines(enginesRight, Math.Abs(turn * turnJetScale));
                  }
                  if (turn <= joyToleranceMin )
                  {
                      FireEngines(enginesLeft, Math.Abs(turn * turnJetScale));
                  }
                  if (turn < joyToleranceMax && turn > joyToleranceMin )
                  {
                      StopEngines(enginesRight);
                      StopEngines(enginesLeft);
                  }

                }
                






            if (hasControl && hasStrafe)
            {
                moveSideways = Input.GetAxis(sideJoy);
                if ( moveSideways > joyToleranceMin && moveSideways < joyToleranceMax )
                {
                  moveSideways = 0.0f;
                }

                movement = Quaternion.AngleAxis(angle + 180.0f,
                    Vector3.up * Mathf.Abs(moveSideways))
                    * Vector3.right * moveSideways;
                rb.AddForce(movement * thrustMultiplier * sideThrustMult);

                UseGas(gasUseRateSideThrust * Math.Abs(moveSideways));
                if (moveSideways >= joyToleranceMax )
                {
                    FireEngines(enginesSideLeft, Math.Abs(moveSideways * sideJetScale));
                }
                if (moveSideways <= joyToleranceMin )
                {
                    FireEngines(enginesSideRight, Math.Abs(moveSideways * sideJetScale));
                }
                if (moveSideways > joyToleranceMin && moveSideways < joyToleranceMax )
                {
                    StopEngines(enginesSideLeft);
                    StopEngines(enginesSideRight);
                }

            }













        }


        //angleText.text = displayAngle.ToString("0.00");
        gasText.text = gas.ToString("0.00");
        // velocityText.text = rb.velocity.y.ToString("0.00"); // FOR NOW DON'T DO THIS, as I am using this text field to test collisionEffect
        
        /*
        // Invert Control toggle
        if (Input.GetAxis("Fire1") >= 0.1f)
        {
            invertControl = false;
            invertControlString = "Flight Control";
        }


        // Invert Control toggle
        if (Input.GetAxis("Jump") >= 0.1f)
        {
            invertControl = true;
            invertControlString = "Inverted Control";
        }
        */



        /*
        // Switch which joystick side motion turns and which strafes
        if (Input.GetAxis("Fire2") >= 0.1f)
        {

            turnJoy = "Horizontal";  // Default "Horizontal"
            sideJoy = "Mouse Y";     // Default "Mouse Y"
            Debug.Log("<color=blue>Fire2 - Strafe on Joy1. Turn on Joy2.</color>");


        }

        // Switch which joystick side motion turns and which strafes
        if (Input.GetAxis("Fire3") >= 0.1f)
        {

            turnJoy = "Mouse Y";  // Default "Horizontal"
            sideJoy = "Horizontal";     // Default "Mouse Y"
            Debug.Log("<color=green>Fire3 - Turn on Joy1. Strafe on Joy2.</color>");


        }

        */



        //Oh, and for now, I'm leaving in the A/Y buttons for up/down, but also adding this:
        invertControlText.text = invertControlString;



  // !!!!!!!!!!!!!!!!! NEXT
  //    GET THE JETS WORKING AS INTENDED! INVERSION FAILS THEM
  // 
  // CLUE: Right now, the stick itself determines which engines fire, not the actual thrust direction as determined by "invertedControl".
  // RIGHT NOW, after putting in some debug text, it seems DOWN in Flight control think
  // 

        upwardThrust = Input.GetAxis(verticalJoy);

    if ( upwardThrust > joyToleranceMin && upwardThrust < joyToleranceMax )
    {
      upwardThrust = 0.0f;
    }

    // Thrust Upward. Unless Inverted Control. Then Thrust Downward
    if ( upwardThrust >= joyToleranceMax && hasControl )
        {
            if (invertControl)
            {
                ThrustDown(Math.Abs(upwardThrust));
                FireEngines(enginesDown, Math.Abs(upwardThrust) * downJetScale);
            }
            else
            {
                ThrustUp(Math.Abs(upwardThrust));
                FireEngines(enginesUp, Math.Abs(upwardThrust) * upJetScale);
            }
        }


        // Thrust Downward. Unless Inverted Control. Then Thrust Upward
        if (upwardThrust <= joyToleranceMin && hasControl)
        {
            if (invertControl)
            {
                ThrustUp(Math.Abs(upwardThrust));
                FireEngines(enginesUp, Math.Abs(upwardThrust) * upJetScale);
            }
            else
            {
                ThrustDown(Math.Abs(upwardThrust));
                FireEngines(enginesDown, Math.Abs(upwardThrust) * downJetScale);
            }
        }





        // NO JOY. STOP ALL ENGINES
        if (upwardThrust <= joyToleranceMax && upwardThrust > joyToleranceMin )
        {
            StopEngines(enginesDown);
            StopEngines(enginesUp);
        }

        GasUp();
        UprightTaxi();

        // Display damage
        collisionText.text = damage.ToString();

        



        gm.hasControl = hasControl;


        if (!hasControl)
        {
            StopEngines(enginesForward);
            StopEngines(enginesBackward);
            StopEngines(enginesUp);
            StopEngines(enginesDown);
            StopEngines(enginesLeft);
            StopEngines(enginesRight);
            StopEngines(enginesSideLeft);
            StopEngines(enginesSideRight);
            StopEngines(enginesTurboFront);
            StopEngines(enginesTurboRear);
        }

    }


    
    



    // !!!!!!!!!!!!!!!!!!!!!! It's just not appearing to fire the thrusters when inverted. I mean only one side fires for each type of control, not both.


    void ThrustUp(float thrustAmount)
    {
        movement = new Vector3(0.0f, thrustAmount * thrustMultiplier * upThrustMult, 0.0f);
        rb.AddForce(movement);
        UseGas(gasUseRateUpThrust * thrustAmount);
        //angleText.text = movement.y.ToString();

        //FireEngines(enginesUp, upJetScale * Math.Abs(thrustAmount));
    }



    void ThrustDown(float thrustAmount)
    {
        movement = new Vector3(0.0f, thrustAmount * -1.0f * thrustMultiplier * downThrustMult, 0.0f);
        rb.AddForce(movement);
        UseGas(gasUseRateDownThrust * Math.Abs(thrustAmount));
        //angleText.text = movement.y.ToString();

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
        // Gas up at a GasStation
        if (hasControl && isAtPad && Math.Abs(rb.velocity.y) < 0.001)
        {
            if ( gas < maxGas )
            {
                gas += gm.gasFillRate;
                gm.cash -= gm.gasCost;
                gm.gasCostThisShift += gm.gasCost; // This is on an update basis. Each fixed frame.
            }

            if (damage > 0.0f)
            {
                damage -= gm.damageRepairRate;
                gm.cash -= gm.damageRepairCost;
                gm.repairsCostThisShift += gm.damageRepairCost; // This is on an update basis. Each fixed frame.
                ShowRadarCrack(); // Refresh radar, which should now be seen miraculously repairing itself
            }
            
            
            //hasControl = true; //This may be needed. But for now it's interfering with the UI, as I want no control during UI screens.
            // Eventually, I think I will have you go back to a home pad, so this won't be a problem, and can go back in.
        }


        // Gas up at Home Base. Faster, cheaper
        if (hasControl && isHome && Math.Abs(rb.velocity.y) < 0.001)
        {
            if (gas < maxGas)
            {
                gas += gm.homePadGasFillRate;
                gm.cash -= (gm.homePadGasCost);
                gm.gasCostHome += gm.homePadGasCost; // This is on an update basis. Each fixed frame.                
            }

            if (damage > 0.0f)
            {
                damage -= gm.homePadDamageRepairRate;
                gm.cash -= gm.homePadDamageRepairCost;
                gm.repairsCostHome += gm.homePadDamageRepairCost; // This is on an update basis. Each fixed frame.
                ShowRadarCrack(); // Refresh radar, which should now be seen miraculously repairing itself
            }


            //hasControl = true; //This may be needed. But for now it's interfering with the UI, as I want no control during UI screens.
            // Eventually, I think I will have you go back to a home pad, so this won't be a problem, and can go back in.
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

        // How much collision?
        float collisionEffect = other.relativeVelocity.magnitude;
        //Debug.Log("<color=white>COLLISION! " + collisionEffect + "</color>");

        // If enough, log it in the text object (which we aren't using anymore)
        if (collisionEffect > 0.01f)
        {
            velocityText.text = collisionEffect.ToString();
        }



        // For now, hitting the airships does no damage, but DOES affect tip
        if ( other.gameObject.tag == "Airship")
        {
            if (collisionEffect > minCollisionThreshold * shieldPercent )
            {
                // Decrease tip for ANY collision at all. Later, multiply that by the amount of collision
                gm.tip -= gm.tipDrain * collisionEffect;
                if (gm.tip <= 0.0f)
                {
                    gm.tip = 0.0f;
                }

                // Also, no VFX if we collid e with an airship. They are bouncy

            }
           
            
            return;
        }




        if (collisionEffect > minCollisionThreshold * shieldPercent )
        {
            // If collisionEffect (amount of collision) is greater than the minCollisionThreshold, add it to damage, MINUS the minimum threshold
            // ie: if minCollisionThreashold is 10 and you take a hit of 12, add 2 to damage, not 12
            // minCollisionThreashold can then be upped later, as you get better armor for your car as upgrades
            damage += ( collisionEffect - ( minCollisionThreshold * shieldPercent) );


            //Debug.Log("<color=red>COLLISION! " + ( collisionEffect - minCollisionThreshold ) + " for accumulated damage of " + damage + ".</color>");

            // Decrease tip for ANY collision at all. Later, multiply that by the amount of collision
            gm.tip -= gm.tipDrain * collisionEffect;
            if (gm.tip <= 0.0f)
            {
                gm.tip = 0.0f;
            }

            collisionText.text = damage.ToString();


            // SILLY THING: try to create a sphere where the two objects collide
            // DAMN! Right now, the thing freaks out by making way too many spheres.
            // PUT A VFX IN INSTEAD THAT IS SELF-KILLING
            // --- BUT FOR NOW: Create a new sphere, but turn off its collider, because that crashes the game causing cascading collisions/creations
            // So get the collider component, and disable it.

            // This version instantiates a spark object every time, but it never goes away
            // And instantiation causes a blip sometimes.
            GameObject tmpExploder = Instantiate(explodeObject);

            // So let's try a version where I just move and enable it.
            //tmpExploder = explodeObject; // This is just so all the math below still works

            tmpExploder.transform.SetParent(rb.transform, false);
            tmpExploder.transform.position = other.contacts[0].point;
            Vector3 splodeScale = new Vector3(collisionEffect * 0.05f, collisionEffect * 0.05f, collisionEffect * 0.05f);
            tmpExploder.transform.localScale = splodeScale;
            tmpExploder.SetActive(true);

            // This is test code to see if it's finding out where the taxi has been hit. 
            // This creates a new sphere object, turns off its collider (otherwise hoooo boy!), and places it on the contact point
            // so I can see if it's doing it right.
            /* GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); // Create it
            sphere.transform.SetParent(rb.transform, false); // Should parent the sphere to the taxi rigidbody
            Collider sphereCollider = sphere.GetComponent(typeof(Collider)) as Collider; // Get its collider so we can
            sphereCollider.enabled = false; // turn it off, otherwise it will be a collider and wow.
            sphere.transform.position = other.contacts[0].point; // Move it to where the contact happened
             */

        }

        // If damage reaches too much, do something cool. Right now I just remove the gas, so it will fall.
        if (damage > maxDamage)
        {
            damage = maxDamage;
            LoseControl();
        }

        // I had this higher up in code, but it was causing problems, probably because
        // it was trying to show sprites in an array PAST maximum damage, because I had not
        // yet capped max damage right above here.
        ShowRadarCrack();
        //rm.CrackRadar();

        if (other.gameObject.tag == "GasStation")
        {
            isAtPad = true;
        }
        else
        {
            isAtPad = false;
        }

        if (other.gameObject.tag == "Home")
        {
            isHome = true;
        }
        else
        {
            isHome = false;
        }



        //////////////////!!!!!!!!!!!!! THIS SECTION was for the old pad system where the cab itself was handling pad landing collision
        ///// Now, however, the pad handles it. Remove this once it's all working

        /*
        if (other.gameObject.tag == "Pad")
        {
            Debug.LogWarning("<color=black> ### You hit a pad!</color>");
            if (gm.numPadsLandedOn > -1)  /////////////////////!!!!!!!!!!!!!!! THIS IS A FAKE CONDITION!
                // Soon, replace this with code to find out if 
            {
                Debug.Log("<color=black> YOU HAVE ALREADY LANDED ON THIS BUILDING!</color>");
                return;
            }
           

            if ( other.gameObject != gm.pads[gm.nextPad] )
            {
                Debug.Log("<color=black> THIS IS NOT THE TARGET BUILDING! You want # " + gm.nextPad + "</color>");

                return;
            }
                

            gm.numPadsLandedOn++;
            gm.nextPad++;
            Debug.Log("<color=black>   LANDED ON PAD " + gm.nextPad + "</color>");
            if ( gm.numPadsLandedOn >= gm.numPads )
            {
                Debug.Log("<color=red>   LEVEL OVER!!!!!!!!!!!!!!!!! </color>");
            }
        }
        */
        


    }


    // Why the hell would you have a method here that does ONE thing: Calls a method on
    // the Radar Manager?
    // Because it eliminates the need for Game Manager to hold its own reference to 
    // Radar Manager, and since it already references in the Taxi, just call it on the Taxi Controller
    // which in turn calls it on the Radar Manager
    public void ShowRadarCrack()
    {
        rm.CrackRadar();

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
        //taxiRotationSmoothSpeed = 1.0f; // Taxi Upright Speed

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
        isCrashing = true;
        rb.constraints = RigidbodyConstraints.None;

        rb.angularDrag = 0.0f;
        rb.drag = 0.0f;

        gm.tip = 0.0f;
        gm.fare = 0.0f;



        // THIS SECTION PUTS A RANDOM SPIN ON THE CAR IF YOU LOSE CONTROL
        // PUT IT BACK?
        float RandXThrust = UnityEngine.Random.value;
        Vector3 StartMovement = new Vector3(((UnityEngine.Random.value - .5f) * 20), 0.0f, (Math.Abs(UnityEngine.Random.value) * -20));
        rb.AddTorque(StartMovement); // 400 is the intensity multiplier on the random direction in the previous line

        
    }

}
