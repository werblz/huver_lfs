using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy_Controller : MonoBehaviour 
{

    private Rigidbody rb;

    private bool isGrounded = false;


    [Header("Objects")]

    [Tooltip("The player Rigidbody")]
    [SerializeField]
    Rigidbody player = null;

    [Tooltip("The player's Vector Indicator object so Enemy can aim to it")]
    [SerializeField]
    GameObject playerVectorIndicator = null;

    [Tooltip("The player's Joystick Indicator object so Enemy can aim to it")]
    [SerializeField]
    GameObject joystickVectorIndicator = null;

    [Header("Movement Properties")]

    [Tooltip("Does the player, when game starts, get a random kick?")]
    [SerializeField]
    private bool initialThrustFlag = false;

    [Tooltip("How much kick does the player get if it gets one at start?")]
    [SerializeField]
    private float initialThrustMult = 1.0f;

    private float moveToTargetHoriz = 0.0f;
    private float moveToTargetVert = 0.0f;
    //private float moveToPlayerHoriz = 0.0f;
    //private float moveToPlayerVert = 0.0f;


    [Tooltip("Randomize the vital properties when instantiated?")]
    [SerializeField]
    private bool randomizeProperties = false;

    [Tooltip("How much to multiply the vector for thrust (speed)")]
    [SerializeField]
    private float thrustMultiplier = 20.0f;

    private Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);
    
    enum HuntMode { Player, Velocity, Joystick }; //Will be used to keep track of what's selected

    [Tooltip("Which kind of follow algorithm to use. Player aims directly to player. Velocity towards the velocity. Jostick to Velocity plus Joystick vectors")]
    [SerializeField]
    private HuntMode huntMode;

    [Tooltip("Is the Enemy lerping between target and player based on distance? (ie: closer to player, more to player)")]
    [SerializeField]
    public bool proximityHunt = false; // This, if true, will perform a lerp between the target and player, based on some factor
    // so the enemy will go for the player if it's closer, and the intended position, if farther

    [Tooltip("Set it to attack?")]
    [SerializeField]
    public bool kill = false;

    [Tooltip("Set it to explode? (ie: give an impulse directly to the player as a 'shock wave'?")]
    [SerializeField]
    public bool explode = false;

    [Tooltip("INFORMATION ONLY! Do not set here.")]
    [SerializeField]
    private float targetLerpAmount = 0.0f;


    //private GameObject compassAngleObj = null;

    private Vector3 followPos = new Vector3(0.0f, 0.0f, 0.0f);

    private float impulseAngle = 0.0f;
    private float impulseToPlayerAngle = 0.0f;

    private float diffTargetX = 0.0f; // Distance to Target on X axis
    private float diffTargetZ = 0.0f; // Distance to Target on Z axis
    private float diffPlayerX = 0.0f; // Distance to Player on X axis
    private float diffPlayerZ = 0.0f; // Distance to Player on Z axis

    private float radAngleTarget = 0.0f; // Angle from enemy pos to target pos
    private float radAnglePlayer = 0.0f; // Angle from enemy pos to player pos


    [Tooltip("INFORMATION ONLY! Do not set here.")]
    [SerializeField]
    private float targetAngle = 0.0f;

    [Tooltip("INFORMATION ONLY! Do not set here.")]
    [SerializeField]
    private float playerAngle = 0.0f;

    [Tooltip("INFORMATION ONLY! Do not set here.")]
    [SerializeField]
    public float lerpedAngle = 0.0f; // Public because this object's parent script needs it

    [Tooltip("INFORMATION ONLY! Do not set here.")]
    [SerializeField]
    private float distanceBetween = 0.0f;

    
    [Header("Visual Properties")]

    [Tooltip("Color to set Enemy if its target is Player")]
    [SerializeField]
    Color playerColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    [Tooltip("Color to set Enemy if its target is Velocity")]
    [SerializeField]
    Color velocityColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    [Tooltip("Color to set Enemy if its target is Joystick")]
    [SerializeField]
    Color joystickColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    [Tooltip("Renderer to color depending on hunt mode")]
    [SerializeField]
    private Renderer enemyRenderer = null;

    private string shaderPropertyColor = "_Color";

    private MaterialPropertyBlock enemyBlock = null;

    private int rand = 0;

    private Color chosenColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    private float joyAmount = 1.0f;

    [Tooltip("INFORMATION ONLY! Do not set here.")]
    [SerializeField]
    public float attackCountDownTimer = 5.0f;

    [Tooltip("Attack radius")]
    [SerializeField]
    public float attackDistance = 1.0f;

    [Tooltip("Attack force")]
    [SerializeField]
    private float attackForce = 100.0f;

    //private float attackHoriz = 0.0f;
    //private float attackVert = 0.0f;



    private void Start()
    {
        rand = 0;

        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 1.0F;

        // Material Property Block for enemy's renderer
        enemyBlock = new MaterialPropertyBlock();

        if (randomizeProperties)
        {
            RandomizeProperties();
        }


        attackCountDownTimer = 5.0f;
        
    }









  // NEXT:
  //
  // Add another LineRenderer set to this script that uses this object and the target point (based on huntModeInt) and draw a new line to visualize the vector he's trying to get to.
  // Add left stick check as a multiplier (0 to 1) on the ThrustMultiplier, so you can make the enemy more aggressive by pushing stick forward, and stop it from following at all by leaving it be




    void FixedUpdate()
    {
        InitialImpulse();

        // NEXT!

        // Get the player's predicted position, which will be the position of the Indicator_Joystick_Vector object
        // Then add an impulse at exactly that direction, with a multiplier.
        // Then, do this ONLY when left trigger is down. That way, we can see the ball just roll, and then 
        // make it actively pursue

        switch (huntMode)
        {
            case HuntMode.Player:
                followPos = player.transform.localPosition;
                chosenColor = playerColor;
                break;
            case HuntMode.Velocity:
                followPos = playerVectorIndicator.transform.localPosition;
                chosenColor = velocityColor;
                break;
            case HuntMode.Joystick:
                followPos = joystickVectorIndicator.transform.localPosition;
                chosenColor = joystickColor;
                break;
            default:
                followPos = player.transform.localPosition;
                break;

        }

        if (proximityHunt)
        {
            chosenColor = chosenColor * 0.75f;
        }

        if (kill)
        {
            chosenColor = chosenColor * 0.5f;
        }

        if (explode)
        {
            chosenColor = chosenColor * 0.25f;
        }

        // Apply the Material Property Block chosen above
        enemyBlock.SetColor(shaderPropertyColor, chosenColor);
        enemyRenderer.SetPropertyBlock(enemyBlock);


        if (isGrounded)
        {

      // ADD: Another huntModeInt that does something a little more complicated:
      // - It keeps TWO positions in mind. Player and Intended
      // - It then lerps between trying to get to those with a factor based on distance between the two. 
      //   - So the farther the enemy is away, the more it aims to Intended.
      //   - The closer it gets, the more it aims to Player

      // Another huntModeInt that has an attack radius. If the player gets inside that radius, the Enemy bursts to the player's position (or vector position)



            /* My AI Scheme:
            1 - Get huntModeInt
            2 - 0 = target is player; 1 target is vector; 2 target is joystick
            3 - Get diff between position of target and self
            4 - Get diff between position of player and self (always player)
            5 - Get angle between target and self using Atan
            6 - Get angle between player and self using Atan(always player)
            7 - Convert both angles to Radians for Cos and Sin (because fuck!!!)
            8 - Lerp between the two angles, depending on targetLerpAmount
                - targetLerpAmount, if 0, is the player. If 1 is the target. That is so as distance between the enemy
                and the player decreases, the targetLerpAmount decreases causing the lerp to focus more on player.
            9 - Get the Cosin and Sin of that lerped angle so I can have my Horiz and Vert force values
            10 - Apply force
            */

            // Get diff in x pos between the two
            diffTargetX = followPos.x - rb.transform.localPosition.x;
            // Get diff in z pos between the two
            diffTargetZ = followPos.z - rb.transform.localPosition.z;

            // Get diff in x pos between enemy and player (this will be the same if mode is 0
            diffPlayerX = player.transform.localPosition.x - rb.transform.localPosition.x;
            // Get diff in z pos between enemy and player (this will be the same if mode is 0
            diffPlayerZ = player.transform.localPosition.z - rb.transform.localPosition.z;


            // Get the angle from the difference between the two
            impulseAngle = Mathf.Atan2(diffTargetX, diffTargetZ) * Mathf.Rad2Deg;
            impulseToPlayerAngle = Mathf.Atan2(diffPlayerX, diffPlayerZ) * Mathf.Rad2Deg;

            targetAngle = impulseAngle;
            playerAngle = impulseToPlayerAngle;


            // Get the angle from the difference between enemy and target
            radAngleTarget = impulseAngle * Mathf.Deg2Rad; // First, convert the angle to Radians
            // Get the angle from the difference between enemy and player
            radAnglePlayer = impulseToPlayerAngle * Mathf.Deg2Rad; // First, convert the angle to Radians

            if (proximityHunt)
            {
                // Get distance between the enemy and player for the lerp. Smaller distance = lerp towards player
                // Larget distance = lerp towards target
                distanceBetween = Vector3.Distance(player.transform.localPosition, rb.transform.localPosition);

                // 10 units is a good max distance, so let's make sure at 10, lerp value is 1
                targetLerpAmount = distanceBetween / 20.0f;
                
                                                           // Greater distances like 30 units will divide to 3, but then we clamp.
                // Now get the lerp amount, but not by altering the distancebetween.
                targetLerpAmount = Mathf.Clamp(targetLerpAmount, 0.0f, 1.0f);

                // THIS IS BROKEN! FOr some reason, the math doesn't work, and the targetLerpAmount is not accurate!
                // Is it because I divide the distance, instead of finding a logarithm, which would make an infinite distance 1, and close 0? That's what I want
                // Currently I"m dividing by an arbitrary max distance, which is likely just not right
                
            }
            else
            {
                targetLerpAmount = 0.5f; // If not doing proximityHunt, just go all lerped towards Target
            }
            // We're fine until here. The lerp amount is coming out correctly.

            // I think I know what's going on. The Angle at 180 shifts from 180-360 to -180 - 0. The anomaly happens when the target and player angles
            // cross that positive/negative line. SO one is -179 the other 179 and LERP gets confused.
            // I need to convert that left range of angles to a full circle.

            // Here's the issue: Angles on the right half of the circle go from 0 - 180. Then on the left half, -0 - 180.
            // To fix that and make the circle a full 360 degrees (which is the only way to lerp correctly between the two halves)
            // I simply add the negative angle to 360 if it's negative

            // SHIT! I fixed it for 179 - -179 but not for 360 - 0.
            /*
             * if (playerAngle < 0)
            {
                playerAngle = 360.0f + playerAngle;
            }
            if (targetAngle < 0)
            {
                targetAngle = 360.0f + targetAngle;
            }
            * THIS SHOULD GO AWAY IF MY QUATERNION LERP WORKS!
            */

            // Fucking think I'm getting fucking GIMBOL LOCK! So I have to convert to Quaternions first!

            Vector3 playerV = new Vector3(0.0f, playerAngle, 0.0f);
            Vector3 targetV = new Vector3(0.0f, targetAngle, 0.0f);
            Vector3 lerpedV = new Vector3(0.0f, 0.0f, 0.0f);
            Quaternion playerQ = Quaternion.Euler(playerV);
            Quaternion targetQ = Quaternion.Euler(targetV);
            Quaternion lerpedQ = Quaternion.identity; // Put these ^^^ at the top

            //lerpedAngle = Mathf.Lerp(radAnglePlayer, radAngleTarget, targetLerpAmount);
            //lerpedAngle = Mathf.Lerp(playerQ, targetQ, targetLerpAmount);

            // First, lerp the two Quaternions
            lerpedQ = Quaternion.Lerp(playerQ, targetQ, targetLerpAmount);
            // Then get the degree angle of that lerped Q angle
            lerpedV = lerpedQ.eulerAngles;
            // But still we now have a Vector3, when we just want the Y rotation, so:
            lerpedAngle = lerpedV.y;
            // FUCK. I think FINALLY we have it.
            
            // I'm setting Player up as the 0 element, and Target as 1, so as distance decreases towards 0, player is more target

            // Convert to Rad for SIn, Cos
            lerpedAngle = lerpedAngle * Mathf.Deg2Rad;

            moveToTargetHoriz = Mathf.Sin(lerpedAngle);
            moveToTargetVert = Mathf.Cos(lerpedAngle);

            // Here, instead of lerping between the two axial forces (cos and sin) of the two diff positions
            // I am lerping between the angles, and applying force same as always, only along the lerp of
            // the two angles. Easier. And should work.

            joyAmount = GetJoystickAmount();

            movement = new Vector3(moveToTargetHoriz * thrustMultiplier * joyAmount, 0.0f, moveToTargetVert * thrustMultiplier * joyAmount);
            
            
            // DO not add the force for now, until I establish I'm getting the correct Sin,Cos from the angle
            rb.AddForce(movement);

            //Vector3 pointerAngle = new Vector3(rb.transform.localRotation.x, 0.0f, rb.transform.localRotation.z);
            //pointerParent.transform.localRotation = Quaternion.Euler(pointerAngle);




            Attack();

            

        }


    }


    void Attack()
    {
        attackCountDownTimer -= Time.deltaTime;
        distanceBetween = Vector3.Distance(player.transform.localPosition, rb.transform.localPosition);

        if (kill && (distanceBetween < attackDistance) && (attackCountDownTimer < 0.0f))
        {
            // Get a new lerped angle between player and target, only this one not so much. Just a bit ahead of player.
            lerpedAngle = Mathf.Lerp(radAnglePlayer, radAngleTarget, 0.2f);// - Do something to make it proportional to attackDistance: WRONG: Mathf.Clamp(0.05f * attackDistance, 0.0f, 1.0f));

            // Luckily we have already calculated the radian angle to the player above for the lerp.
            // And we do it outside of the IF statement checking if it is proximity hunting
            moveToTargetHoriz = Mathf.Sin(lerpedAngle);
            moveToTargetVert = Mathf.Cos(lerpedAngle);

            movement = new Vector3(moveToTargetHoriz * thrustMultiplier * attackForce * joyAmount, 0.0f, moveToTargetVert * thrustMultiplier * attackForce * joyAmount);
            //Debug.Log("<color=red>********************* Distance = " + distanceBetween + " KILL!</color>");
            if (explode)
            {
                // Make player move
                player.AddForce(movement );

                // This is a bit convoluted. I had to make the Enemy_pointer objects public so this could read it
                // BESIDES THIS GOES AWAY! Once I create a circle ring explosion VFX
                //enemyCircle.circleSprite.color = enemyCircle.boomColor;
            }
            else
            {
                // Make Enemy move
                rb.AddForce(movement);
                
            }
            attackCountDownTimer = 5.0f;
        }

    }


    float GetJoystickAmount()
    {
        float amount = Input.GetAxis("Mouse X");
        //Debug.Log("<color=white>***************</color> Joystick Amount = " + amount);
        return Math.Abs(amount);
    }



    void RandomizeProperties()
    {
        rand = (int)UnityEngine.Random.Range(0.0f, 3.0f);
        //Debug.Log("<color=white>*******</color> Rand0m = " + rand);


        // Randomly set AI properties

        // What kind of hunt mode?
        switch (rand)
        {
            case 0:
                huntMode = HuntMode.Player;
                break;
            case 1:
                huntMode = HuntMode.Velocity;
                break;
            case 2:
                huntMode = HuntMode.Joystick;
                break;
            default:
                huntMode = HuntMode.Player;
                break;
        }

        float tmp = UnityEngine.Random.Range(0.0f, 1.0f);

        kill = false;
        if (tmp > 0.5f)
        {
            kill = true;
            tmp = UnityEngine.Random.Range(0.0f, 1.0f);
            if (tmp > 0.5f)
            {
                explode = true;
            }

        }

        // What Thrust Multiplier
        thrustMultiplier = UnityEngine.Random.Range(5.0f, 30.0f);

        // Proximity Hunt?
        rand = (int)UnityEngine.Random.Range(0.0f, 2.0f);
        if (rand == 0)
        {
            proximityHunt = false;
        }
        else
        {
            proximityHunt = true;
        }

        attackDistance = UnityEngine.Random.Range(5.0f, 15.0f);

    }

    void InitialImpulse()
    {
        if (initialThrustFlag == true)
        {
            float RandXThrust = UnityEngine.Random.value;
            Vector3 StartMovement = new Vector3((UnityEngine.Random.value - .5f), UnityEngine.Random.value, (UnityEngine.Random.value - .5f));
            rb.AddForce(StartMovement * initialThrustMult);
            initialThrustFlag = false;
        }
    }


    void OnCollisionEnter( Collision other )
    {
        if ( other.gameObject.tag == "Ground" )
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit( Collision other )
    {
        if ( other.gameObject.tag == "Ground" )
        {
            isGrounded = false;
        }
    }
    
}
