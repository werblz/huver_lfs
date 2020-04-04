using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Controller : MonoBehaviour 
{

	CharacterController controller;

	[SerializeField]
	private TextMesh HorizTextObj = null;

	[SerializeField]
	private TextMesh VertTextObj = null;

	[SerializeField]
	private TextMesh JumpTextObj = null;

	[SerializeField]
	private TextMesh JumpThrustTextObj = null;

  [SerializeField]
  private TextMesh IsGroundedTextObj = null;

	[SerializeField]
	private bool IsColorer = true;

	private Rigidbody rb;

  private bool isGrounded = false;

	[SerializeField]
	private bool ThrustFlag = false;

	[SerializeField]
	private GameObject glasses = null;

	private float angle = 0.0f;

	private float moveHorizontal = 0.0f;
	private float moveVertical = 0.0f;
	private float jump = 0.0f;
	private float jumpThrust = 0.0f;

  [SerializeField]
  private float thrustMultiplier = 20.0f;

	private Vector3 movement = new Vector3 (0.0f, 0.0f, 0.0f);
	//private void Update()
	//{
	//}

	//public float thrust;

	private void Start()
	{
		
		rb = GetComponent<Rigidbody>();
		Physics.sleepThreshold = 8.5F;
	
	}





  void Update()
	{
		
		// Not sure why this should ask if ThrustFlag is false. It seems it should ask if it's true
		// If I remember right, ThrustFlag adds a random thrust at start of play, so the player drops
		// at a random angle, not straight down.
		if (ThrustFlag == false) 
		{
			float RandXThrust = UnityEngine.Random.value;
			Vector3 StartMovement = new Vector3 ( (UnityEngine.Random.value -.5f), UnityEngine.Random.value, (UnityEngine.Random.value -.5f) );
			rb.AddForce (StartMovement * 300);
			ThrustFlag = true;
		}

	    moveHorizontal = Input.GetAxis( "Horizontal" );
	    moveVertical = Input.GetAxis( "Vertical" );

      movement = new Vector3( moveHorizontal * thrustMultiplier, 0.0f, moveVertical * thrustMultiplier ); 
      rb.AddForce( movement );




    // THIS CURRENTLY DOES NOT DO WHAT I WANT.

    // What I want is that if on any update you detect a jump, it adds force ONCE, then times down.
    // However that would always mean you can only jump once in every 30 updates. NOT waht I want.
    // What I WANT is to be able to jump only if Grounded
    // So it's back to figuring out Grounded code using raycast

    // At the moment, this keeps thrust at 1 for the full second during countdown
    // To get THIS to work (forget about the above) set a flag for CanJump, which only is true if timer is 0
    // And then only allow a jump if CanJump is true

    // And it seems that for now, the thrust only works if you're actually thrustin X and Z too.
    /* if (jumpTimer < 30) { // If the timer is at 0
		
			jump = Input.GetAxis ("Jump"); // Get button
			if (jump > 0) { // Is it a JUMP?
				jumpThrust = 1.0f;  // If so, thrust upward 1.0
				movement = new Vector3( 0.0f, jumpThrust, 0.0f );
				rb.AddForce (movement * 20.0f);
				jumpTimer = 30; // And reset timer to 30 to re-count-down
			}
			else{ // If !JUMP
				jumpThrust = 0.0f; // Zero out upward thrust
				jumpTimer = 0; // Reset timer
			
			}
      */

    IsGroundedTextObj.text = isGrounded.ToString();


    if ( Input.GetAxis( "Jump" ) > 0 )
    {
      if ( isGrounded )
      {
        jumpThrust = 5.0f;
        movement = new Vector3( 0.0f, jumpThrust * thrustMultiplier, 0.0f );
        rb.AddForce( movement );
      }
    }

    if ( Input.GetAxis( "Fire1" ) > 0 )
    {
      jumpThrust = 1.0f;
      movement = new Vector3( 0.0f, jumpThrust * thrustMultiplier, 0.0f );
      rb.AddForce( movement );
    }




    // Right now this lumps jumpThrust in with the regular movement
    // I should change this to do Vector3(0, jumpThrust, 0) on jump, and leave this at 0 
    /*
	    movement = new Vector3( moveHorizontal, 0.0f, moveVertical );
		// Provided my IsGrounded code above works, this should ONLY add force if you're on the ground, which
		// may prove better, or maybe not. Right now you can steer while in the air
		rb.AddForce (movement * 20.0f);
    */


    // SHow me the vector onscreen
    HorizTextObj.text = moveHorizontal.ToString();
		VertTextObj.text = moveVertical.ToString();
		JumpTextObj.text = jump.ToString();
		JumpThrustTextObj.text = jumpThrust.ToString();



    Glasses_Follow ();


	}







	// Whenever the player enters another object, it checks to see if the player IsColorer (ie: is the character
	// doing the painting? It might not be. It might be pushing other painters.
	// And if IsColorer, then set the tag to Painter, as each tile has a script on it that colors itself
	// if it meets a Painter
	private void OnTriggerEnter( Collider other )
	{
		if (IsColorer) 
		{
			tag = "Painter"; // Simply put, the tiles themselves check to see if a "Painter" object hit them
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

  void Glasses_Follow()
	{



		// Now put the glasses on the ball, pointed towards the thrust direction of the controller
		glasses.transform.position = transform.position;
		if (moveVertical == 0.0f && moveHorizontal == 0.0f) 
		{
			return;
		}
		else
		{
			angle = ((float)Math.Atan2 (moveHorizontal, moveVertical) * 180.0f / Mathf.PI) + 90.0f;
			glasses.transform.rotation = Quaternion.AngleAxis ((float)angle, Vector3.up);

		}

	}

}
