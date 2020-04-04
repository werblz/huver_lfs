using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelorometer_Input : MonoBehaviour {

	// Use this for initialization
//	void Start () {
		
//	}
	
	// Update is called once per frame
	void Update () {

    transform.Translate( Input.acceleration.x, 0, -Input.acceleration.z );
    	
	}
}
