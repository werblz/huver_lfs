using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad_Manager : MonoBehaviour {


    // Tells the game manager it's the next target pad
    [HideInInspector]
    public bool isNext = false;

    // Tells the game manager which pad this is
    [HideInInspector]
    public int padNumber = 0;

    // Lights up the beam
    [HideInInspector]
    public bool isLit = false;

    // Has this pad been touched yet?
    [HideInInspector]
    public bool isTouched = false;

    [SerializeField]
    private Game_Manager gm = null;

    [SerializeField]
    private Rigidbody taxiRb = null;

    [SerializeField]
    public int lightNumber = 9;

    [SerializeField]
    private Texture2D[] lightNumTexture = null;

    [SerializeField]
    private GameObject lightBeam = null;

    [SerializeField]
    private MeshRenderer gaugeMesh = null;

    [SerializeField]
    private int landingTimeMax = 200;

    private bool isTouching = false;

    private int landingTime = 200;

    private float revealOffset = 1.0f;

    [SerializeField]
    private Renderer rend = null;

    private MaterialPropertyBlock mpb = null;

    // Use this for initialization
    void Start () {

        
        isTouched = false; // Successfully completed?
        isTouching = false; // Is currently in contact with taxi?
        landingTime = landingTimeMax;

        // Here I need to get the MaterialPropertyBlock.
        //mpb = new MaterialPropertyBlock();
        //mpb.SetTexture("_Additive", lightNumTexture[lightNumber]);
        //rend.SetPropertyBlock(mpb);

    }

    private void OnEnable()
    {
        
        //Debug.Log("<color=black>         LIGHT COLOR = " + lightNumber + "</color>");
        // Here I need to get the MaterialPropertyBlo ck.
        mpb = new MaterialPropertyBlock();
        // Stupidly, since we're counting UP pads, but counting DOWN numbers, I have
        // to do a little math there to get the right index number
        mpb.SetTexture("_Additive", lightNumTexture[gm.numPads - lightNumber -1]);
        rend.SetPropertyBlock(mpb);
    }


    void FixedUpdate()
    {
        if ( isTouching && taxiRb.velocity.magnitude < 0.01f )
        {
            landingTime = landingTime - (int)(Time.deltaTime * landingTimeMax); // Later turn this into seconds, instead of an arbitrary int

            //Debug.Log("<color=red>************************* </color> Landing Time: " + landingTime + ", Max Time: " + landingTimeMax + ", and divide: " + (float)((float)landingTime / (float)landingTimeMax));

            revealOffset = 1.0f - (((float)landingTime / (float)landingTimeMax) * .9f);
            //Debug.Log("<color=blue>************************* </color> revealOffset = " + revealOffset);

            gaugeMesh.material.SetFloat("_Cutoff", revealOffset);

            if (landingTime < 0)
            {
                isTouching = false; 
                isTouched = true;
                Debug.LogWarning(" **** TOUCHED PAD " + padNumber);
                gm.Advance();
                Debug.LogWarning(" **** MOVING ON to pad " + gm.nextPad);
            }
        }
    }




    // This gets the flag from Beam in Game Manager to turn the beam on or off
    // And it passes the current nextPad number so we can put the right texture on
    public void LightBeam(bool flag, int padNum)
    {
        //Debug.Log("<color=black>         LIGHT COLOR = " + lightNumber + "</color>");
        // Here I need to get the MaterialPropertyBlo ck.
        mpb = new MaterialPropertyBlock();
        // Stupidly, since we're counting UP pads, but counting DOWN numbers, I have
        // to do a little math there to get the right index number
        mpb.SetTexture("_Additive", lightNumTexture[gm.numPads - lightNumber -1]);
        rend.SetPropertyBlock(mpb);

        lightBeam.SetActive(flag);
    }


    // This logs when the cab stops touching the pad, so sets the flag to false. Otherwise OnCollisionEnter is triggered once, and ever after, it thinks the flag is true
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            isTouching = false;
        }
    }

    void OnCollisionStay(Collision other)
    {

        if (other.gameObject.tag == "Player")
        {
            if (isTouched)
            {
                //Debug.Log("****************** BEEN TOUCHED ALREADY!");
                return;
            }
            else
            {
                
                if (padNumber == gm.nextPad)
                {
                    // In here I need to put a variable that I'm on the pad, which is sent to an update loop that times down. When done, sets isTouched = true. If false, 
                    isTouching = true;

                }
                else
                {
                    isTouching = false;
                    Debug.LogWarning("THIS IS NOT THE TARGET PAD! Try again! Target is " + gm.nextPad + " but this is pad " + padNumber );
                }
             
            }
            
        }
        else
        {
            //Debug.LogWarning("Uh-oh. Something hit me, but I didn't recognize it as player.");
        }
       

    }
        //This should get the progress bar working on sprite
        //void Update()
        //{
        //    float revealOffset = (float)(Time.timeSinceLevelLoad % 10) / 10.1F;
        //
        //    gameObject.renderer.material.SetFloat("_Cutoff", revealOffset);
        //}
    

}
