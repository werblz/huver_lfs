using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade_Base : MonoBehaviour {

    [SerializeField]
    private bool IsPurchased = false;

    [HideInInspector]
    public Upgrade_Base upScript = null;

    public bool isPurchased
    {
        get
        {
            return IsPurchased;
        }
        set
        {
            IsPurchased = value;
        }
    }

    /*
    public void Start()
    {
        upScript = GetComponent<Upgrade_Base>();
        Debug.LogError("<color=yellow> I FOUND AN UPGRADE DATA COMPONENT NAMED " + up.name + "</color>");

    }
    */


    // This is not used yet. Thankfully, as this may cause an infinite loop because I'm not sure
    // the upScript reference doesn't refer to itself.
    public void PerformUpgrade()
    {
        upScript.PerformUpgrade();
    }

}
