using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyTest : MonoBehaviour {

    [SerializeField]
    private string joystring = string.Empty;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("<color=white>******</color> Joystick at " + joystring + " reads " + Input.GetAxis(joystring));
	}
}
