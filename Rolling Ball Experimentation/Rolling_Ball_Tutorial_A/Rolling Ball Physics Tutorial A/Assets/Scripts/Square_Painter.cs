using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Square Painter is the controlling script for the sqaures themselves.
// It keeps track of whether a Painter has hit, and changes color if so
// It keeps track of whether all of them are painted or not, and if they are,
// it performs a celebration, that for now, just raises the pieces up to 40f
// Meanwhile before scores begin to be affected, the board drops down in DropIn().
// Eventually I want to put each square on an animator controller, with four animations.
// Each one comes from one corner of the max board size, down to its final position.
// The controller will have a blend tree which tells the animation how much to blend in 4 directions
// which will result in the pieces falling in from completely random positions. Each square ends up at its local
// 0,0,0 which is where the level designer has placed it.
// Later, in Celebrate, it will trigger another animation where all the four balls fall down into space. This
// should also trigger the remval of the physics floor so the ball drops too.


public class Square_Painter : MonoBehaviour {

	public bool isPainted = false;

	[SerializeField]
	private Color myColor = new Vector4 (1.0f, 1.0f, 1.0f, 1.0f);

	[SerializeField]
	private Color targetColor = new Vector4 (1.0f, 0.0f, 0.0f, 1.0f);

	[SerializeField]
	private Color victoryColor = new Vector4 (0.0f, 0.0f, 0.0f, 1.0f);

	Renderer rend = null;

	float yPos = 0.0f;
	float yStartPos = 0.0f;
	float yLocal = 0.0f;
	float ySpeed = 0.0f;

	// Use this for initialization
	void Start () {

		ySpeed = Random.Range(.5f, 1.5f);

		rend = gameObject.GetComponent<Renderer>();
		rend.material.SetColor( "_Color", myColor );

		yPos = transform.position.y;
		yStartPos = yPos + 40.0f;
		yLocal = yStartPos;
		transform.position = new Vector3 (transform.position.x, yStartPos, transform.position.z);
		
	}


	// On triggering this object's collider it checks to see if the object is tagged "Painter".
	// If it is, then it checks this object's bool isPainted. If not, it paints it, increments score, and
	// sets the flag to true so it can only ever transition and score once.
	private void OnTriggerEnter( Collider other )
	{
		// !!!! ADD! 
		// So far every touch thinks it's painting, not unpainting.
		// This trigger should check to see if it's the target color, and set it only if it is not.
		// Right now, the ball and the pushmes all color them but differently.
		// My final game logic will have flags for behavior, and that may mean
		// 1. One touch paint. Done
		// 2. Paint/Unpaint on touch
		// 3. Cycle through multiple colors to target (ie multiple touch to finish)
		// 4. Other?
	    
		if (other.gameObject.CompareTag ("Painter")) {
			if (!isPainted) {
				rend.material.SetColor ("_Color", targetColor);
				Scorekeeper.Score++;
				isPainted = true;
			}

		}
	} 
	
	// Update is called once per frame
	void Update () {
		
		// If level is over, then celebrate
		if (Scorekeeper.Score >= Scorekeeper.Total) {
			//Scorekeeper.SeparatorText.text = "GAME OVER!";
			Celebrate ();
		} else { // else, if level is NOT over (ie: just begun) then drop in
			DropIn ();
			//Debug.Log ("Y Pos = " + transform.position.y + "; Y Start = " + yStartPos );
		}
	}

	private void Celebrate ()
	{
		rend.material.SetColor ("_Color", victoryColor);
		Debug.Log ("Now going back to start = " + yStartPos);

		// This part does not yet work. It shows yStartPos as being 40.0f, as set earlier, in Debug statement
		// but it no longer rises up past one iteration.

    // HOWEVER: I want to change this.
    // I want to turn off the static property and make all pieces fall downward using physics. I'd start by giving each one a small random impulse and rotation so they all
    // spin downward at realistically different ways, but if I can get each to interact physically to realistically bump each other, that could be cool
		if (transform.position.y < yStartPos ) {
			yLocal = yLocal + ySpeed;
			transform.position = new Vector3 (transform.position.x, yLocal, transform.position.z);

		} else {
			transform.position = new Vector3 (transform.position.x, yStartPos, transform.position.z);
		}

	}

	private void DropIn() // THIS NOW WORKS!
    // This is called on update. The Start sets each tile some distance above its origin.
	// During the update loop, it calls DropIn(). DropIn() then checks to see if the tile is above its origin.
	// If it is, it performs a simple Y drop per frame until it IS at its origin. Then when it is at origin
	// this does nothing.
	{
		

		if (transform.position.y > yPos) {
			yLocal = yLocal - ySpeed;
			transform.position = new Vector3 (transform.position.x, yLocal, transform.position.z);

		} else {
			transform.position = new Vector3 (transform.position.x, yPos, transform.position.z);
		}

	}

}
