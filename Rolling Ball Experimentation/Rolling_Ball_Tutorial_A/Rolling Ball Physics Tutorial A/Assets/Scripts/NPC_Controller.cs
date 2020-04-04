using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour 
{


	private Rigidbody rb;


	private void Start()
	{

		rb = GetComponent<Rigidbody>();

		float RandXThrust = Random.value;
		Vector3 StartMovement = new Vector3 ( (Random.value -.5f), Random.value, (Random.value -.5f) );
		rb.AddForce (StartMovement * 400); // 400 is the intensity multiplier on the random direction in the previous line
		Physics.sleepThreshold = 5.5F;

	}


	//public float thrust;


	//private void OnTriggerEnter( Collider other )
	//{
	//    Renderer rend = other.gameObject.GetComponent<Renderer>();
	//    
	//    if ( other.gameObject.CompareTag( "Square" ) )
	//		rend.material.SetColor( "_Color", Color.red );
	//      //other.gameObject.SetActive( false ); //- Destroys the square
	//	Scorekeeper.Score ++;
	//} 

}
