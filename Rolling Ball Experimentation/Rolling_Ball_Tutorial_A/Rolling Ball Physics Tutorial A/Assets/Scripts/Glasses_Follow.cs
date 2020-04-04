using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Glasses_Follow : MonoBehaviour {

	public GameObject player; // This is the player object to 'parent' the glasses to. Not parenting, since it rolls. Placing it, and rotating it myself
	public Rigidbody body; // This is the ridigbody. Not sure it's needed yet.
	public Vector3 offset = new Vector3 (0.0f, 0.0f, 0.0f); // This is for later, when the glasses are modeled, in case not at ball origin
	//private float yRot = 0.0f;
	//private Vector3 totalRot = new Vector3( 0.0f, 0.0f, 0.0f );
	private Vector3 oldPos = new Vector3 (0.0f, 0.0f, 0.0f); // I am getting glasses angle by finding the angle between two positions
	private Vector3 newPos = new Vector3 (0.0f, 0.0f, 0.0f); // First position when Update starts, then again when it ends.
	private Double angle = 0.0f; // This is the Y angle to set the glasses to

	private int frameCount = 0; // This is intended to gate updates, by making them run 1/2 the time

	//void Start()
	//{

	//}

	// Update is called once per frame
	void LateUpdate()
	{
		frameCount++;

		oldPos = player.transform.position;
			


		// Determine the yrotation of the player and give that rotation to the glasses object
		// However, this is wonky. It flips around too much.
		//yRot = player.transform.eulerAngles.y;
		//transform.rotation = Quaternion.AngleAxis( yRot, Vector3.up );



		// New strategy. Grab previous postion and current position and get the angle from these two points on the grid
		if (frameCount % 2 == 0) 
		{
			angle = Math.Atan2 (newPos.x - oldPos.x, newPos.z - oldPos.z) * 180.0f / Mathf.PI;
			transform.rotation = Quaternion.AngleAxis ((float)angle, Vector3.up);
		}

		// place the glasses object on the sphere for each frame
		transform.position = player.transform.position + offset;

		newPos = player.transform.position;
  }
}
