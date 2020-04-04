using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour {


    [SerializeField]
    private int frameRate = 30;

	// Use this for initialization
	void Start () {

        Application.targetFrameRate = frameRate;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
