using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top_Camera_Follow : MonoBehaviour {

    [SerializeField]
    GameObject taxi = null;

    [SerializeField]
    Vector3 camOffset = new Vector3(0.0f, 100.0f, 0.0f);
	
	// Update is called once per frame
	void Update () {
        transform.position = taxi.transform.position + camOffset;

        float rot = taxi.transform.localRotation.eulerAngles.y;
        transform.localRotation = Quaternion.Euler(0.0f, rot, 0.0f);

    }
}
