using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Crash_Dialog_Random : MonoBehaviour {

    [SerializeField]
    private TextMeshPro[] dialog = null;

	// Use this for initialization
	void Start () {

        for (int i = 0; i < dialog.Length; i++)
        {
            dialog[i].enabled = false;
        }

        int randChoice = (int)(Random.value * dialog.Length);
        Debug.Log("CHOSEN NUMBER " + randChoice);

        dialog[randChoice].enabled = true;
    }
	
	// Update is called once per frame 
	void Update () {
		
	}
}
