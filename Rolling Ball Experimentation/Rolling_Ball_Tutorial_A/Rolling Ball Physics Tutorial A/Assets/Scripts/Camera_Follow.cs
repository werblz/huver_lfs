using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour {


    [SerializeField]
    private Game_Manager gm = null; // Get the game manager. I think I need it for camera control

  [SerializeField]
  private Transform target = null; // This and the Taxi_Controller below may be done better by grabbing the GameObject, then GetComponent for those two items
    // rather than referencing basically teh same object twice.

    [SerializeField]
    private Taxi_Controller taxi = null;

    [SerializeField]
    private Camera myCamera = null;

  [SerializeField]
  private float MoveSmoothSpeed = 10.0f;

  [SerializeField]
  private float RotationSmoothSpeed = 10.0f;

    private float rotSmoothSpeed = 10.0f;
    private float moveSmoothSpeed = 10.0f;

  [SerializeField]
  private Vector3 offset = new Vector3( 0.0f, 0.0f, 0.0f );



    // Set up an offset for the camera follow that uses the righ-hand joystick
    /*
    [SerializeField]
    private TextMesh joy2X_Text = null;
    [SerializeField]
    private TextMesh joy2Y_Text = null;
    [SerializeField]
    private TextMesh joy2Button_Text = null;
    [SerializeField]
    private TextMesh joy2ButtonBooltest_Text = null;
    [SerializeField]
    private TextMesh buttonTest_Text = null;
    */

    private float joy2X = 0.0f;
    private float joy2Y = 0.0f;
    private float joy2Button = 0.0f;

    [Header("Control")]

    [SerializeField]
    private bool cameraBank = true;

    [SerializeField]
    private float camSpeedX = 0.1f;

    [SerializeField]
    private float minCamAngleX = 30.0f;
    [SerializeField]
    private float maxCamAngleX = 30.0f;

    Vector3 tmpEulerAngles = new Vector3 (0.0f, 0.0f, 0.0f);
    Quaternion camOffsetQ = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);


    // Offset For Camera
    float camOffsetX = 0.0f;
    float camOffsetY = 0.0f;

    private Vector3 desiredPosition = Vector3.zero;
    private Vector3 smoothedPosition = Vector3.zero;

    private void Start()
    {
        moveSmoothSpeed = MoveSmoothSpeed;
        rotSmoothSpeed = RotationSmoothSpeed;
        // This is dumb, but because I want Camera_Main to be at 0 degrees in editor,
        // so I can align UI elements, but it has to be 20 degrees on x during play,
        // I set it here
        myCamera.transform.localRotation = Quaternion.Euler(new Vector3(20.0f, 90.0f, 0.0f));
    }

    private void FixedUpdate()
  {

        // First, get the camera angle offset from the right joystick
        // The joystick works fine. I CANNOT GET THE JOY PUSH TO WORK!
        joy2X = Input.GetAxis("Mouse X");
        joy2Y = Input.GetAxis("Mouse Y");
        joy2Button = Input.GetAxis("Fire3"); // FIRE3 is X button

        /*
        joy2X_Text.text = joy2X.ToString("0.00");
        joy2Y_Text.text = joy2Y.ToString("0.00");
        joy2Button_Text.text = camOffsetX.ToString("0.00");
        joy2ButtonBooltest_Text.text = camOffsetY.ToString();

        buttonTest_Text.text = joy2Button.ToString("0.00");
        */


        if (gm.uiIsUp)
        {
            transform.position = target.position + offset; // This blips the camera to the taxi's position when the UI is up (between shifts)
            transform.rotation = target.transform.rotation; // Hopefully this will rotate it
        }

        if (taxi.cameraFollow == true )
        {
            // If Turbo is on, make the camera lerp much faster, otherwise we lose sight of the car as that speed increases.
            if (taxi.turboTrigger > 0.01 && taxi.hasTurbo)
            {
                RotationSmoothSpeed = rotSmoothSpeed * 1.5f; // taxi.turboMultiplier;
                MoveSmoothSpeed = moveSmoothSpeed * 1.05f; // taxi.turboMultiplier;
                
            }
            else
            {
                RotationSmoothSpeed = rotSmoothSpeed;
                MoveSmoothSpeed = moveSmoothSpeed;
            }

            desiredPosition = target.position + offset;
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, MoveSmoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

        }
        else // This contingency allows for a camera SNAP if the taxi's "cameraFollow" is set to false
        {
            transform.position = target.position + offset;
        }



        // If the rigidbody query equals the rigidbody freezex (x is frozen) then make the camera rotate follow car. Otherwise, do not follow rotation, only position (above)

        //if ( taxi.Controlled )
        if (!taxi.isCrashing && taxi.hasControl )
            //( rb.constraints & RigidbodyConstraints.FreezeRotationX ) == RigidbodyConstraints.FreezeRotationX )
        {
          GetCameraOffset();
          Quaternion desiredRotation = target.localRotation;

            if (!cameraBank)
                {
                    Vector3 noBankRotation = new Vector3(0.0f, desiredRotation.eulerAngles.y, 0.0f); 
                    desiredRotation = Quaternion.Euler(noBankRotation);
                }
          Quaternion smoothedRotation = Quaternion.Lerp( transform.localRotation, desiredRotation, RotationSmoothSpeed * Time.deltaTime );
          transform.rotation = smoothedRotation * camOffsetQ;
        }
  }



    // Get simple euler angle for left/right, up/down.
    void GetCameraOffset()
    {
    
        
        // FOR NOW, REMOVE THIS. BUT LATER PUT IT BACK UNDER THE JOYPAD CONTROL OR X,Y


        float tmpX = Input.GetAxis("JoyPadUp") * camSpeedX;
        if (Mathf.Abs(tmpX) < 0.1f)
    {
      tmpX = 0.0f;
    }
        //float tmpY = Input.GetAxis("Mouse Y") * camSpeedY;
    //if ( Mathf.Abs(tmpY) < 0.1f )
    //{
    //  tmpY = 0.0f;
    //}

    camOffsetX = camOffsetX + (tmpX * camSpeedX * -1.0f);
        if (camOffsetX > maxCamAngleX)
        {
            camOffsetX = maxCamAngleX;
        }
        if (camOffsetX < minCamAngleX * -1)
        {
            camOffsetX = minCamAngleX * -1;
        }

/*
        camOffsetY = camOffsetY + (tmpY * camSpeedY * -1.0f);
        if (camOffsetY > maxCamAngleY)
        {
            camOffsetY = maxCamAngleY;
        }
        if (camOffsetY < maxCamAngleY * -1)
        {
            camOffsetY = maxCamAngleY * -1;
        }
        */

        // This results in an X and Y euler offset
        // Now to add that to the Quaternion


        // If "Fire3" (X button) is pressed, reset angle offset to 0
        float resetCamera = Input.GetAxis("Fire1");
        if (resetCamera > 0.01)
        {
            camOffsetX = 0.0f;
            camOffsetY = 0.0f;
        }
        tmpEulerAngles = new Vector3(0.0f, camOffsetY, camOffsetX); // for some reason my X is in the Z pos to work
        
        camOffsetQ = Quaternion.Euler(tmpEulerAngles);

        
    }
}
