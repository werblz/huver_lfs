using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UI_Panel_Controller : MonoBehaviour {

    [SerializeField]
    private Game_Manager gm = null;

    [SerializeField]
    private Taxi_Controller taxi = null;

    [SerializeField]
    private GameObject uiPanel = null;

    [SerializeField]
    private GameObject upgradePanel = null;

    [SerializeField]
    private GameObject[] upgradeHoldingLocations;

    private Upgrade_Data[] upgradeDataItems = null;

    [SerializeField]
    public GameObject shiftDialogParent = null;

    [HideInInspector]
    public GameObject myDialog; // This is specifically to hold the dialog instance so I can delete it later


    // The default choice in the upgrade array
    private int currentChoice = 1;

    // This could be the bool I test with the Y joystick button to ensure
    // I don't repeatedly perform an upgrade
    private bool hasChosenOnce = false;

    private int[] picks = null;

    // THIS SCRIPT WAS NOT DOING ANYTHING!
    // The GameObject reference above was not even filled out
    // And Game_Manager was referencing the UI panel itself, and putting the panel up and down
    // That's about to change

    /* The time has come to make this panel do more.
     Panel comes up, like it does now, with text congratulating you on the shift
     However, now this panel will handle your available upgrades, and let you select
     from them

        It will require:
        - 3 slots (for now) that I can attach prefabs to
        - A joystick controller that reads the joystick for up, middle, down
        - Based on those, this script will scale up the top, middle or bottom prefab scale parent
        - The B Button will then perform the action based in the selected prefab
        - This script will read all upgrade prefabs and make a list of 3 eligible candidates
          - If a prefab has already been chosen, it eliminates it as a candidate
          - If a prefab costs more than the player has, it eliminates it as a candidate
          - If a prefab has requirements not met, it eliminates it as a candidate
            (However, requirements may not be an issue, as I decided that things like enhanced
            strafe which requires strafe, will not show up at all until strafe is chosen, because
            strafe is in the first few. And the upgrade to it won't show up until later
          - Once you hit the B button, the action "PerformUpgrade()" will be taken on that prefab's script
            so it will do the upgrade.
          - Once you choose an upgrade, the 3 disappear, leaving the panel as it was, until you
            hit B again. Or maybe I'll use another button instead of B to choose. Like Y
              - RATIONALE:
                - I'm using B now to lower the UI
                - If I use B to choose, then it will automatically drop the UI since B is already pressed
                - Y is a nice "Yes" button
                - A is already used to reset the custom camera angle

     */



    // Use this for initialization
    void Awake() {
        // I definitely only want 3 upgrades visible at any one time
        Array.Resize(ref upgradeHoldingLocations, 3);

        upgradePanel.SetActive(false);
    }

    void OnEnable()
    {
        if (!gm.upgradesAvailable)
        {
            return;
        }

        // Ah. First, we have to GET all of the available prefabs. Before, when I was getting errors
        // I was only getting the first 3. This gets all of them.
        // THIS IS CAUSING ISSUES! Code further down is causing NREs becuase I believe this array here
        // is not getting set up correctly

        // First, resize the array, because right now, it's only set to 3, I think.
        Array.Resize(ref upgradeDataItems, gm.upgrades.Length + 1);
        Debug.LogWarning("<color=blue> Resizing upgradeDataItems to " + (gm.upgrades.Length + 1) + "</color>");
        for (int i = 0; i < gm.upgrades.Length; i++)
        {
            upgradeDataItems[i] = (Upgrade_Data)gm.upgrades[i].GetComponent(typeof(Upgrade_Data));
            Debug.LogWarning("<color=blue> UPGRADE " + i + " is " + upgradeDataItems[i].name + "</color>");
        }

        // If upgrades are available, set them up
        PopulateUpgrades();

        // First, put up the upgrades by making the holding locations visible
        ShowUpgrades(upgradeHoldingLocations.Length);

        // Here, do the calculations needed to determine which 3 upgrades are made available
        // to the player. Based on cost vs. cash, prerequisites, etc.

        hasChosenOnce = false;

    }

    // Update is called once per frame
    void Update() {
        if (!gm.upgradesAvailable)
        {
            return;
        }

        // This only runs if the panel is active, so no need to check for that.
        // However, I will definitely want to put in a check to see if upgrades
        // are currently available, and how many, since at the start, we want a panel
        // with no visible upgrades.
        // Do that check here, with a RETURN if none available

        // Eventually, I will want to handle 1 or 2 upgrades. 
        // At which point I will do a case statement.
        // Case 1: Nothing is highlighted unless you stick up or stick down, both of which highlights the only
        // Case 2: Nothing is highlighted unless you stick up or down, neutral highlights neither
        // Case 3: Exactly like it is now

        // Default joystick position results in middle choice, then it polls stick
        // to see if it should be one of the others
        currentChoice = 1;

        // Stupid Input Controller shit. In Taxi, I use Vertical for forward thrust
        // which is the left stick. Need to use left stick because the buttons are on
        // the right
        float getChoiceJoy = Input.GetAxis("Vertical");

        if (getChoiceJoy < -0.5f)
        {
            currentChoice = 2;
        }
        if (getChoiceJoy > 0.5f)
        {
            currentChoice = 0;
        }

        HighlightChosenUpgrade(currentChoice);

        // Trying to figure out which button is A. Fire1 or Jump
        // THIS CAN GET COMPLICATED
        // I want to have upgrades available, but you don't have to choose one.
        // So I think I'll keep the B button to go to next shift in the Taxi controller
        // Though I may have to move it here. 
        // And the A button will choose an upgrade. Until you hit B, however, you are
        // still in the UI panel. I guess when you choose an upgrade, the upgrade 
        // section should be disabled.
        // 
        // This is the part of writing a game I dislike.
        // "Fire1" is the A button, but I"m using that to reset custom camera angle
        // Try "Jump" which is the Y button I think, which works, 
        // becuase you are saying "YES" to an upgrade
        if (Input.GetAxis("Jump") > 0.1f)
        {
            PerformUpgrade(currentChoice);
        }

    }





    // Instantiate a dialog prefab, parent it, and move it, and turn it on
    public void PutDialog(int dialogNum)
    {
        // UI_Panel_Controller calls PutDialog(-1) if we crash. Otherwise, business as usual
        if (dialogNum == -1)
        {
            myDialog = Instantiate(gm.crashDialog);
        }
        else
        {
            // So this worked. It created a clone. Now can I move it, etc?
            myDialog = Instantiate(gm.shiftDialogs[dialogNum]);
            Debug.LogWarning("<color=blue>*** myDialog = " + myDialog.name + "</color>");

        }


        // Yup. It movedd.
        myDialog.transform.SetParent(shiftDialogParent.transform, false);
        Debug.LogWarning("<color=blue>*** Setting Parent of " +
            myDialog.name + " to " +
            shiftDialogParent.name +
            "</color>");

        // Can I parent it?
        myDialog.transform.localPosition = Vector3.zero;
        Debug.LogWarning("<color=blue>*** Setting localPos to 0,0,0 </color>");

        myDialog.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        myDialog.SetActive(true);

    }


    // In preparation to put up a new dialog, let's uparent the old one, move it to 0, turn it off
    // So this one removes by passing the old myDialog so it removes it directly. 
    // The PutDialog passes a number in the array of dialogs
    public void RemoveDialog(GameObject myOldDialog)
    {
        

        //Deparent it and put it at origin
        myOldDialog.transform.parent = null;

        myOldDialog.transform.localPosition = Vector3.zero;
        myOldDialog.SetActive(false);
    }




    private void PopulateUpgrades()
    {
        // Picks will be an array of numbers, the number of the prefabs chosen.
        // This will always be an array of 1, 2 or 3, but the index numbers inside can be
        // any number of the prefabs I have created
        picks = FindUpgrades();



        //Now populate the prefabs, for now just pop all 3.
        for (int i = 0; i < upgradeHoldingLocations.Length; i++)
        {   // Parent the prefab to the holding location, with false, so it will move
            gm.upgrades[picks[i]].transform.SetParent(upgradeHoldingLocations[i].transform, false);
            gm.upgrades[picks[i]].transform.localPosition = Vector3.zero;
            gm.upgrades[picks[i]].transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            gm.upgrades[picks[i]].transform.localScale = Vector3.one;
            gm.upgrades[picks[i]].SetActive(true);

            upgradeDataItems[picks[i]] = (Upgrade_Data)gm.upgrades[picks[i]].GetComponent(typeof(Upgrade_Data));
            Debug.Log("<color=white>UPGRADE CHOSEN = " + upgradeDataItems[picks[i]].name + "</color>");
        }

        


    }

   

    private int[] FindUpgrades()
    {
        // Prepare the array. STOP DOING CODE IN THE ARRAY WITHOUT RESIZING IT FIRST, YOU IDIOT!
        int[] chosenUpgrades = null;
        int numChoices = 3;
        // Stupid me wasn't resizing the array BEFORE trying to add to it. DUH!
        Array.Resize(ref chosenUpgrades, numChoices);




        /* 
        The Upgrade Plan for now:
        
        First, populate the 3 slots with upgrades 0-2. These upgrades will be small incremental
        updates that can be there when all the real ones are gone. But we put them there first,
        so when we run out, and the selection only finds 2 or 1 remaining, the other slots
        will have these 3 upgrades, which can carry us through to whenever the game ends.

        - Gas +5%
        - Shields +5%
        - Deductile -5% - Keep in mind, we have not yet implemented Deductibles on crash.

        These three won't even check to see if they are purchased, because they can be purchased over and over again

        Then we go through the Upgrade list from 3-end, (so our loop begins at 3, not 0)
        eliminating any that have been purchased.

        */
        

        // First, populate the 3 slots with recurring ones for when we run out
        for (int i = 0; i < numChoices; i++)
        {
            chosenUpgrades[i] = i;
            Debug.Log("**** Upgrade " + i + " is 4");
        }


        
        // Start with slot 0
        int slot = 0;


        // Loop through all of the NON-DEFAULT upgradeDataItems, 3 through last
        for (int i = 3; i < upgradeDataItems.Length - 1; i++)
        {
            // This stops code if slot gets to numchoices-1 (so slot 2). We should never get to 3
            if (slot < numChoices)
            {
                Debug.Log("<color=blue>**** Looking at upgrade = " + i + "</color>");
                Debug.Log("****** Currently trying to fit Slot = " + slot);
                if (!upgradeDataItems[i].isPurchased)
                {
                    Debug.Log("<color=blue>******** upgrade " + i + " is NOT already purchased</color>");
                    // Pick i
                    Debug.Log("********* ADDING UPGRADE " + i + " TO SLOT " + slot);
                    chosenUpgrades[slot] = i;
                    slot++;
                    Debug.Log("\n\n\n**********************************");
                    for (int j = 0; j < numChoices; j++)
                    {
                        Debug.Log("**** Upgrade " + i + " is " + i + " which is " + upgradeDataItems[i].name);
                    }
                }
            }
            
        }



        // TMP - Insert my own values to see if it works
        /*
        chosenUpgrades[0] = 2;
        chosenUpgrades[1] = 3;
        chosenUpgrades[2] = 4;
        */
        Debug.Log("\n******************* CHOICES " + chosenUpgrades);
        return chosenUpgrades;
    }


    private void HighlightChosenUpgrade(int choice)
    {
        // First, reset scales
        for (int i = 0; i < upgradeHoldingLocations.Length; i++)
        {
            upgradeHoldingLocations[i].transform.localScale = Vector3.one;
            upgradeDataItems[picks[i]].glowSprite.enabled = false;
            upgradeDataItems[picks[i]].glowSpriteRed.enabled = false;
            upgradeDataItems[picks[i]].noPurchase.enabled = false;
        }

        // Now enlarge the one I am currently choosing with the stick
        upgradeHoldingLocations[choice].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        // Also turn on the glow aura SpriteRenderer
        if (gm.cash < upgradeDataItems[picks[choice]].upgradeCost
            &&
            upgradeDataItems[picks[choice]].upgradeCost > 0.0f)
        {
            upgradeDataItems[picks[choice]].glowSpriteRed.enabled = true;
            upgradeDataItems[picks[choice]].glowSprite.enabled = false;
            upgradeDataItems[picks[choice]].noPurchase.enabled = true;
        }
        else
        {
            upgradeDataItems[picks[choice]].glowSpriteRed.enabled = false;
            upgradeDataItems[picks[choice]].glowSprite.enabled = true;
            upgradeDataItems[picks[choice]].noPurchase.enabled = false;
        }
        
    }

    private void ShowUpgrades(int numUpgrades)
    {
        upgradePanel.SetActive(true);

        // Make the holding locations visible.
        for (int i = 0; i < numUpgrades; i++)
        {
            upgradeHoldingLocations[i].SetActive(true);
        }
    }

    private void PerformUpgrade(int choice)
    {
        if (hasChosenOnce)
        {
            return;
        }

        // Check to see if you have enough cash for the upgrade. OR if the upgrade is not free, return.
        if (!gm.debugOn && gm.cash < upgradeDataItems[picks[choice]].upgradeCost
            && upgradeDataItems[picks[choice]].upgradeCost > 0.0f )
        {
            return;
        }


        // This is a bit convoluted, but let me explain, so I can understand it later.
        // upgradeDataItems is the prefab
        // The prefab has a Upgrade_Base component on it.
        // The Upgrade_Data component on it gets this Upgrade_Base reference which can be
        // Any unique upgrade script that does the specific upgrade
        // Then the Upgrade_Base class gets the Upgrade Script from its base, even though
        // It will have its own class. 
        // Then this code which has chosen the upgradeDataItem in the array picks[choice]
        // an ordered array in itself (ie: could be 3, 5, 1) and looks into that upgradeScript
        // finds the base class up, and then uses the base class to do a PerformUpgrade
        // which literall just tells its child script of whatever class, to do a PerformUpgrade.
        // PLEASE LET THIS WORK!
        
        //upgradeDataItems[picks[choice]].upgradeScript.PerformUpgradeInData();

        // Switch on the upgradeNumber in the chosen upgrade.
        
        switch (upgradeDataItems[picks[choice]].upgradeNumber)
        {
            case 0:
                // This used to be a 5% gas tank size increase. 
                // But I've decided to change it to a home base radar indicator
                // So keep this stuff around for when I introduce that one again later.
                taxi.maxGas = taxi.maxGas * 1.05f; // Upgrade maxGas 5%
                taxi.gas = taxi.maxGas; // Tell the taxi we did it
                gm.hasTank = true; // Make sure the stat is set so the icon can appear in dash
                Debug.Log("<color=blue>INCREASE GAS TANK 5%!</color>");
                
                break;
            case 1:
                taxi.shieldPercent *= 1.05f; // increase minCollisionThreshold 5%
                // No need to set a gm. bool for the icon to appear. It is coded to appear at above 1
                Debug.Log("<color=blue>SHIELD UPGRADE 5%!</color>");
                break;
            case 2:
                gm.crashDeductible = gm.crashDeductible * .95f; // increase minCollisionThreshold 5%
                // No need to set a gm. bool for the icon to appear. Itis coded to appear at above 1
                // No need to set a gm. bool for the icon to appear. Itis coded to appear at above 1
                Debug.Log("<color=blue>DEDUCTIBLE REDUCTION 5%!</color>");
                break;

            // NOW FOR THE REGULAR UPGRADES
            case 3:
                taxi.hasNextIndicator = true;
                gm.hasRadarPad = true;
                Debug.Log("<color=blue>PLATFORM RADAR UPGRADE!</color>");
                break;
            case 4:
                taxi.hasStationIndicator = true;
                gm.hasRadarStation = true;
                Debug.Log("<color=blue>STATION RADAR UPGRADE!</color>");
                break;
            case 5:
                taxi.hasStrafe = true;
                gm.hasStrafe = true;
                Debug.Log("<color=blue>STRAFE CONTROL UPGRADE!</color>");
                break;
            case 6:
                taxi.hasTurbo = true;
                gm.hasTurbo = true;
                Debug.Log("<color=blue>TURBO UPGRADE!</color>");
                break;
            case 7:
                taxi.maxGas = taxi.maxGas * 1.2f; // Upgrade maxGas
                taxi.gas = taxi.maxGas; // Filler up the tank
                gm.hasTank = true;
                Debug.Log("<color=blue>BIGGER TANK UPGRADE!</color>");
                break;
            case 8:
                taxi.shieldPercent *= 1.2f; // Upgrade the minCollisionThreshold to new value
                Debug.Log("<color=blue>SHIELD UPGRADE 20%!</color>");
                break;
            case 9:
                taxi.sideThrustMult *= 2f; // Upgrade the minCollisionThreshold to new value
                Debug.Log("<color=blue>STRAFE UPGRADE *2!</color>");
                break;
            case 99:
                taxi.hasHomeIndicator = true;
                gm.hasHomePad = true;
                break;
            default:
                // Nothing happens on default
                Debug.Log("<color=blue>NO UPGRADE CHOSEN!</color>");
                break;
        }
 

        // Deduct cost
        Debug.Log("CASH BEFORE UPGRADE - " + gm.cash);
        if (!gm.debugOn) // Only spend the cash if NOT in debug mode
        {
            gm.cash -= upgradeDataItems[picks[choice]].upgradeCost;
        }
        
        Debug.Log("CASH AFTER UPGRADE - " + gm.cash);
        upgradeDataItems[picks[choice]].isNew = false;
        // Don't forget to turn off the upgrade prefab or it will remain visible next time
        // While I'm at it, unparent it and set it to origin, so the holding location doesn't
        // just keep getting stacks and stacks of new ones parented.
        upgradeDataItems[picks[choice]].enabled = false;
        upgradeDataItems[picks[choice]].transform.parent = null;
        upgradeDataItems[picks[choice]].transform.localPosition = Vector3.zero;


        // For now this does nothing but disappear the choices.
        for (int i = 0; i < upgradeHoldingLocations.Length; i++)
        {
            upgradeHoldingLocations[i].SetActive(false);
        }
        upgradePanel.SetActive(false);

        // HERE WE HAVE TO FIGURE OUT HOW TO GET THE UPGRADE TO NOT DO IT MANY TIMES
        hasChosenOnce = true;
        // We set this bool true when you select. We set it to false when panel comes up
        // THIS SHOULD WORK
        upgradeDataItems[picks[choice]].isPurchased = true;
    }

    public void UIOff()
    {
        uiPanel.SetActive(false);
    }
}
