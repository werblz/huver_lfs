using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Radar_Manager : MonoBehaviour {

    [SerializeField]
    private GameObject nextIndicator = null;

    [SerializeField]
    private GameObject gasIndicator = null;

    [SerializeField]
    private GameObject homeIndicator = null;
    
    [SerializeField]
    private GameObject[] ledSegment = null;

    [SerializeField]
    private GameObject compassDirectionIndicator = null;

    [SerializeField]
    private SpriteRenderer checkEngineLight = null;

    [SerializeField]
    private Image gasGaugeImage = null;

    [SerializeField]
    private Image damageImage = null;

    // The following are for the cracked radar. THIS SYSTEM IS ABOUT TO BE DEPRECATED!
    /*
    [SerializeField]
    private SpriteRenderer[] crackSpriteRenderer = null;
    private MaterialPropertyBlock mpb = null;
    */

    // IN FAVOR OF THIS ONE:
    [Tooltip("Sprite Renderer object that holds the crack sprite image")]
    [SerializeField]
    private SpriteRenderer[] crackSpriteRenderers = null;

    [Tooltip("Sprite images")]
    [SerializeField]
    private Sprite[] crackSprites = null;

    [SerializeField]
    private Taxi_Controller taxi = null;

    [SerializeField]
    private Game_Manager gm = null;

    [SerializeField]
    private TextMeshPro shiftText = null;

    [SerializeField]
    private TextMeshPro gasTankText = null;

    [SerializeField]
    private float gasFlashPercentage = .2f;

    [Header("Upgrade Icons")]
    [SerializeField]
    private SpriteRenderer[] upgradeIcon = null;

    [Header("Shield Text")]
    [SerializeField]
    private TextMeshPro shieldText = null;



    private float angle = 0.0f;

    private Vector3 indRotation = new Vector3(0.0f, 0.0f, 0.0f);

    

    private Vector3 compassRotation = new Vector3(0.0f, 0.0f, 0.0f);

    private Vector3 gasIndRotation = new Vector3(0.0f, 0.0f, 0.0f);

    private Vector3 homeIndRotation = new Vector3(0.0f, 0.0f, 0.0f);

    private float taxiAngle = 0.0f;

    private int padNum = 0;

    

    private Vector3 padLoc = new Vector3(0.0f, 0.0f, 0.0f);

    private Vector3 taxiLoc = new Vector3(0.0f, 0.0f, 0.0f);

    private Taxi_Controller tc = null;

    private static bool[,] seg = new bool[,]
    {
        {true, true, true, true, true, true, false },
        {false, true, true, false, false, false, false },
        {true, true, false, true, true, false, true },
        {true, true, true, true, false, false, true },
        {false, true, true, false, false, true, true },
        {true, false, true, true, false, true, true },
        {true, false, true, true, true, true, true },
        {true, true, true, false, false, false, false },
        {true, true, true, true, true, true, true },
        {true, true, true, true, false, true, true },
        {true, true, true, true, true, true, false },
        {true, false, false, true, false, false, true }
    };

    private bool flashGasFlag = false;
    private float flashTime = 0.0f;
    private float alphaColor = 1.0f;



    // THE PLAN
    //
    // On Update, the radar gets the taxi's current position and rotation.
    // Then it gets the position of the next pad
    // Using trigonometry, we get the angle the pad is at compared to the taxi, subtracting the taxi's rotation
    // The radius will not matter, since we are simply going to rotate the indicator's pad.


    // Use this for initialization
    void Start ()
    {
        //Debug.Log("<color=white>*************************** Array element 3, 6 = " + seg[3, 5].ToString());

        tc = (Taxi_Controller)taxi.GetComponent(typeof(Taxi_Controller));

        // Turn off all upgrade icons, just in case

        for (int i = 0; i < upgradeIcon.Length; i++)
        {
            upgradeIcon[i].enabled = false;
        }

        checkEngineLight.enabled = false;

        DisplayLED(gm.numPads);
        //Debug.Log("<color=blue>************************************ Number of pads to start = " + gm.numPads);

        //mpb = new MaterialPropertyBlock();
        //crackedRadarSprite.GetPropertyBlock(mpb);
        
    }

    // Update is called once per frame

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!! INVESTIGATE transform.LookAt! This apparently does it 
    void Update()
    {
        padNum = gm.nextPad;

        nextIndicator.SetActive(padNum < gm.numPads);

        shiftText.text = gm.shift.ToString();

        gasTankText.text = taxi.gas.ToString("#0");

        // Get the taxi angle. It is used for all angular indicators on radars
        taxiAngle = taxi.transform.localEulerAngles.y;

        // THIS ONE IS THE REAL DIGITAL READOUT
        DisplayLED(gm.numPads - padNum); // Better way to do it, with an array of bools

        // If level is over, do not try getting the next pad's location, since it will put the array out of bounds
        if (padNum < gm.numPads && taxi.hasNextIndicator)
        {
            // Find the location of the current pad
            padLoc = gm.pads[gm.nextPad].transform.position;

            // Find the taxi's location
            taxiLoc = taxi.transform.position;

            // Use trig to get the angle
            angle = Mathf.Atan2(padLoc.x - taxiLoc.x, padLoc.z - taxiLoc.z) * 180 / Mathf.PI;
            //taxiAngle = taxi.transform.localEulerAngles.y;
            indRotation = new Vector3(0.0f, angle + 180.0f - taxiAngle, 0.0f); // This is in case angle needs an offset.] DOES THIS DO ANYTHING?
            compassRotation = new Vector3(0.0f, 0.0f, taxiAngle);

            //!!!!!!!!!!!!!!!!!!!! - SO FAR THE INDICATOR DOES ROTATE, but not accurately. It's in the angle offset first And then has to add the taxi's rotation
            nextIndicator.transform.localRotation = Quaternion.Euler(indRotation);

        }

        // Turn pad indicator on or off depending on if we have it
        nextIndicator.SetActive(taxi.hasNextIndicator);

        homeIndicator.SetActive(taxi.hasHomeIndicator);


        // Gas Station Indicator
        if (taxi.hasStationIndicator)
        {
            // Find the closest pad to the taxi
            Vector3 gasLoc = gm.stations[ClosestStation()].transform.position;
            

            // Find the taxi's location
            taxiLoc = taxi.transform.position;

            // Use trig to get the angle
            angle = Mathf.Atan2(gasLoc.x - taxiLoc.x, gasLoc.z - taxiLoc.z) * 180 / Mathf.PI;
            //taxiAngle = taxi.transform.localEulerAngles.y;
            gasIndRotation = new Vector3(0.0f, angle + 180.0f - taxiAngle, 0.0f); // This is in case angle needs an offset.] DOES THIS DO ANYTHING?
            //gasIndicatorRot = new Vector3(0.0f, 0.0f, taxiAngle);

            //!!!!!!!!!!!!!!!!!!!! - SO FAR THE INDICATOR DOES ROTATE, but not accurately. It's in the angle offset first And then has to add the taxi's rotation
            gasIndicator.transform.localRotation = Quaternion.Euler(gasIndRotation);
            
        }

        if (taxi.hasHomeIndicator)
        {
            // Find the home pad location
            Vector3 homeLoc = gm.homeBldg.transform.position;

            // Find the taxi's location
            taxiLoc = taxi.transform.position;

            // Use trig to get the angle
            angle = Mathf.Atan2(homeLoc.x - taxiLoc.x, homeLoc.z - taxiLoc.z) * 180 / Mathf.PI;
            //taxiAngle = taxi.transform.localEulerAngles.y;
            homeIndRotation = new Vector3(0.0f, angle + 180.0f - taxiAngle, 0.0f); // This is in case angle needs an offset.] DOES THIS DO ANYTHING?
            //gasIndicatorRot = new Vector3(0.0f, 0.0f, taxiAngle);

            //!!!!!!!!!!!!!!!!!!!! - SO FAR THE INDICATOR DOES ROTATE, but not accurately. It's in the angle offset first And then has to add the taxi's rotation
            homeIndicator.transform.localRotation = Quaternion.Euler(homeIndRotation);

        }

        // Turn indicator on if we have it, off if not
        gasIndicator.SetActive(taxi.hasStationIndicator);

        // Compass overlay
        compassDirectionIndicator.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, taxiAngle);

        // Update the gas gauge
        float fillPercentage = Mathf.Abs(tc.gas / tc.maxGas);
        gasGaugeImage.fillAmount = fillPercentage * 0.75f;

        // If gas gets into the red, flash
        if ( fillPercentage < gasFlashPercentage)
        {
            flashGasFlag = true;
        }
        else
        {
            flashGasFlag = false;
            gasGaugeImage.color = Color.white;
        }
        FlashGas(0.5f);




        // Display the appropriate upgrade icons
        IconsUp();
    }



    private void FlashGas (float speed)
    {
        if (flashGasFlag == false)
        {
            return;
        }


        flashTime += Time.deltaTime;
        
        if (flashTime > speed )
        {
            alphaColor = 1.0f;
            flashTime = 0;
        }
        else
        {
            alphaColor -= 0.05f;
        }

        gasGaugeImage.color = new Color(1.0f, 1.0f, 1.0f, alphaColor);

    }


    public void CrackRadar()
    {
        // Update damage gauge
        float damagePercentage = Mathf.Abs(tc.damage / tc.maxDamage);
        damageImage.fillAmount = damagePercentage * 0.165f;

        // Update the glass crack overlay
        // Get the damage, stepped by dividing by 5
        // 0 = no damage, 1-4 = increasing damage. 4 is max
        // So take damagePercentage, mult by 5. (0-1 becomes 0-4)
        // But this gauges to howver many sprites you put in the array. I now have 5 sprites.
        // 0 is no damage. 1,2,3,4 are incremental damage, then I want the final damage NOT 
        // to change sprites, but keep 4 up there. This way 4 is max damage, but you keep flying
        // for ONE MORE damage pip
        float damageStepped = (int)(damagePercentage * crackSprites.Length);
        if (damageStepped > crackSprites.Length)
        {
            damageStepped = crackSprites.Length;
        }
        //Debug.Log("<color=red>**********DAMAGE DAMAGE DAMAGE - " + damageStepped + "</color>");
        /*
        Debug.Log("<color=red>**********DAMAGE DAMAGE DAMAGE - " + damageStepped +
            " which uses SPRITE " + crackSprites[(int)damageStepped] + "</color>");
        */

        for (int i = 0; i < crackSpriteRenderers.Length; i++)
        {
            if (damageStepped > crackSpriteRenderers.Length)
            {
                damageStepped = crackSpriteRenderers.Length;
            }
            crackSpriteRenderers[i].sprite = crackSprites[(int)damageStepped];
            //Debug.Log("<color=blue> Sprite is " + crackSprites[(int)damageStepped].name + "</color>");
        }
    }



    int ClosestStation()
    {
        int closest = 0;

        float minDistance = 10000.0f;

        for (int i = 0; i < gm.numStations; i++)
        {
            // Get location of this station
            Vector3 sLoc = gm.stations[i].transform.position;
            Vector3 tLoc = taxi.transform.position;



            float distance = Vector3.Distance(sLoc, tLoc);
            

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = i;
            }
            
        }
        return closest;
    }

    // seg is a static array set up with the segments of the LED display set to on or off to correspond to the digits 0-9
    void DisplayLED(int number)
    {
        if (number <0 || number > 9)
        {
            number = 10;
        }
            
        for (int i = 0; i < 7; i++)
        {
            ledSegment[i].SetActive(seg[number, i]);
        }
        
    }


    void IconsUp()
    {
        // Change this code section to this form: upgradeIcon[0].enabled = gm.hasRadarPad.
        /*
        if (gm.hasRadarPad)
        {
            upgradeIcon[0].enabled = true;
        }
        if (gm.hasRadarStation)
        {
            upgradeIcon[1].enabled = true;
        }
        if (gm.hasStrafe)
        {
            upgradeIcon[2].enabled = true;
        }
        if (gm.hasTurbo)
        {
            upgradeIcon[3].enabled = true;
        }
        if (gm.hasTank)
        {
            upgradeIcon[4].enabled = true;
        }
        if (taxi.minCollisionThreshold > 10.0f)
        {
            upgradeIcon[5].enabled = true;
        }
        */

        upgradeIcon[0].enabled = gm.hasRadarPad;
        upgradeIcon[1].enabled = gm.hasRadarStation; ;
        upgradeIcon[2].enabled = gm.hasStrafe;
        upgradeIcon[3].enabled = gm.hasTurbo;
        upgradeIcon[4].enabled = gm.hasTank;
        upgradeIcon[5].enabled = taxi.shieldPercent > 1.0f;
        shieldText.enabled = taxi.shieldPercent > 1.0f; // Enable the text ONLY if shieldPercent > 1.0. Otherwise disble
        shieldText.text = "+" + (taxi.shieldPercent - 1.0f).ToString("P0");
        
        upgradeIcon[6].enabled = gm.hasHomePad;

        if (!gm.uiIsUp)
        {
            // hasControl? If not, CHECK ENGINE LIGHT
            checkEngineLight.enabled = !gm.hasControl;

        }

    }

}
