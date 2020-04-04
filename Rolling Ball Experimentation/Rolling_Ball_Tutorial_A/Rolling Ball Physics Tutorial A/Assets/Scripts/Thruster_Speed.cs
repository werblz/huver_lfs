using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster_Speed : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Animator anim = gameObject.GetComponent<Animator>();

        float randomSpeed = Random.Range(0.9f, 1.1f);

        anim.speed = randomSpeed;
	}
	

}
