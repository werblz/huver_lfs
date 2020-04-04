using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_01 : Upgrade_Base {

    private Upgrade_Data up = null;

    private void Start()
    {
        up = GetComponent<Upgrade_Data>();
        Debug.LogError("<color=yellow> I FOUND AN UPGRADE DATA COMPONENT NAMED " + up.name + "</color>");
    }

    // Use this for initialization
    public void DoThisUpgrade ()
    {
        Debug.LogError("<color=blue> DOING UPGRADE!</color>");
	}
	
}
