using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Gas_Plus_Five_Percent : Upgrade_Base {





 // This specific upgrade class is derived from Upgrade_Base which is how we perform this
 // function. We use the Upgrade_Base.PerformUpgrade which calls THIS class's PerformSpecificUpgrade
    public void PerformSpecificUpgrade () {
        Debug.Log("<color=red>!!!!!!!!!!!!!!!!!! UPGRADE DONE!</color>");
		
	}

}
