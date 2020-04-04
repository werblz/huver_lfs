using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Upgrade_Data : MonoBehaviour {

    // For now, we will use this as a number for a case statement in UI Panel Controller or Game Manager
    // which will determine the upgrade chosen, and the case statement will handle the details
    [SerializeField]
    public int upgradeNumber = 0;

    // This is the new system. If it works, I should be able to reference individual update
    // scripts, and because they are of a base class (that's basically empty) Upgrade_Base, I will
    // find those scripts generically, and then be able to call into them to do the upgradeScript.DoUpgrade()
    // which divorces my code from having to use a case statement which uses an int on the upgradeData item
    // which is cumbersome to maintain. Instead, each upgrade will hold its own code for the upgrade
    // allowing me to upgrade any variable in the game that I want, without having to keep a long
    // list of case statements in Game Manager. Let's see if this works..

    // LATER WHEN I GET THIS FIGURED OUT: public Upgrade_Base upgradeScript = null;

    [SerializeField]
    private string friendlyDescription = null;

    // Since cost will be reflected in the text, and can differ from the cost
    // in data, I will make the text read the data
    [SerializeField]
    private TextMeshPro upgradeCostText = null;

    [SerializeField]
    private SpriteRenderer isNewSprite = null;

    [SerializeField]
    public float upgradeCost = 0.0f;

    [SerializeField]
    public bool isPurchased = false;

    [SerializeField]
    public bool isNew = false; // For each upgrade, this SHOULD start out as true

    [SerializeField]
    public SpriteRenderer glowSprite = null;

    [SerializeField]
    public SpriteRenderer glowSpriteRed = null;

    [SerializeField]
    public SpriteRenderer noPurchase = null;

    // LATER! public Upgrade_Base upPrefab = null;

    private void Start()
    {

        // EXPERIMENT:
        // First, find the Upgrade_Base component on this prefab. That will be a script
        // whose class is the name of the upgrade, but it derives from Upgrade_Base so I can
        // consistently get the reference using the base class, so each can be unique
        /*
         * upPrefab = GetComponent<Upgrade_Base>();
        Debug.LogError("<color=yellow> I FOUND AN UPGRADE DATA COMPONENT NAMED " + upPrefab.name + "</color>");
        */

        if (upgradeCost > 0.0f)
        {
            upgradeCostText.text = "\u00A7 " + upgradeCost.ToString("F2");
        }
        else
        {
            upgradeCostText.text = "FREE";
        }

        
        
    }

    private void Awake()
    {
        if (isNew)
        {
            isNewSprite.enabled = true;
        }
        else
        {
            isNewSprite.enabled = false;
        }
    }

    // TRYING TO MAKE THIS WORK, but it is not used at hte moment
    // This is for when I can make a base class for upgrades, then use unique scripts of that class
    // to call to make an upgrade from a common command in Ui_Panel_Controller
    /*
     * public void PerformUpgradeInData()
    {
        upPrefab.upScript.PerformUpgrade();
    }
    */


}
