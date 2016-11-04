// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool allows you
 * to create an orange, vertical running axis line in front of you, which 
 * you can set as an axis which the entire model assembly will rotate around.
 * Instructions are these:
 * Drag Axis gameobject into Axis slot on script
 * Drag AxisTracker gameobject (under Camera(eye) in [CameraRig] gameobject) into Axis Tracker slot on script
 * Drag Camera (eye) gameobject into Camera_eyes slot on script
 * The assigned button toggles on/off the orange axis line. Everytime you toggle 
 * the axis line on, it will always be created in front of you where you are looking.
 * The touchpad of the controller is already assigned to do the rest of the 
 * functionality. When you click up/down on the touchpad, it will pan the axis line
 * forward or backward based on where you were looking when you created the axis line. 
 * Clicking and holding down left on the touchpad will rotate the model assembly left 
 * around the axis line. Clicking and holding down right on the touchpad will rotate
 * the model assembly right around the axis line. */

//LET IT BE NOTED THAT THIS TOOL DOESN'T WORK OVER NETWORK - TOO MUCH DATA
//IT CAN'T HANDLE THAT MUCH DATA BEING TRANSFERRED BETWEEN NETWORKED USERS

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AxisRotation : Tools
{
    public GameObject axisPrefab;
    public Vector3 direction;
    private GameObject cadModel;

    public button onOffButton;
    public button panButton;
    private ButtonSubHandler onOff;
    private ButtonSubHandler pan;
    private GameObject axis;


    public enum state { off, idle, moving, rotateRight, rotateLeft };
    public state axisState = state.off;

    // Use this for initialization
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        onOff.clicked = onOffClicked;
        onOff.unclicked = null;

        pan.clicked = padClicked;
        pan.unclicked = padUnclicked;

        buttonMap.Add(onOffButton, onOff);
        buttonMap.Add(panButton, pan);

    }

    private void onOffClicked(object sender, ClickedEventArgs e)
    {
        if (axisState == state.off)
        {
            turnOnAxis();
        }
        else
        {
            turnOffAxis();
        }
    }


    IEnumerator panCoroutine(Vector3 nVector)
    {
        Debug.Log("COOOR");
        while (axisState == state.moving)
        {

            axis.transform.rotation = Quaternion.Euler(axis.transform.rotation.z, controllerInit.transform.rotation.eulerAngles.y, axis.transform.rotation.x);

            axis.transform.Translate(nVector * Time.deltaTime);

            nVector = new Vector3(controllerInit.controllerState.rAxis0.x, 0, controllerInit.controllerState.rAxis0.y);

            yield return null;
        }

        yield return null;

        print("MyCoroutine is now finished.");
    }

    IEnumerator rotate()
    {
        Debug.Log("COOOR");
        while (axisState == state.rotateRight || axisState == state.rotateLeft)
        {

           if (axisState == state.rotateRight)
            {
                rotateAxisRight();
            }
           else if(axisState == state.rotateLeft)
            {
                rotateAxisLeft();
            }

            yield return null;
        }

        yield return null;

        print("MyCoroutine is now finished.");
    }

    private void padClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("rotate");
        if (controllerInit.triggerPressed)
        {
            Debug.Log("t Pressed");
            //Left
            if (controllerInit.controllerState.rAxis0.x <= -0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
            {
                axisState = state.rotateLeft;
                StartCoroutine("rotate");
            }

            //DPAD RIGHT BUTTON SETTINGS -TOGGLE THROUGH CUTTING PLANE SNAPPED TO ORIGIN PLANES//////////////////
            else if (controllerInit.controllerState.rAxis0.x >= 0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
            {
                axisState = state.rotateRight;
                StartCoroutine("rotate");
            }

            //Up
            else if (controllerInit.controllerState.rAxis0.y >= 0.5f && controllerInit.controllerState.rAxis0.x >= -0.5f && controllerInit.controllerState.rAxis0.x <= 0.5f)
            {
                Vector3 startLocation = new Vector3(cadModel.transform.position.x, axis.transform.position.y, cadModel.transform.position.z);
                axis.transform.position = startLocation;
            }
        }
       
        else
        {
            axisState = state.moving;
            Vector3 nVector = new Vector3(controllerInit.controllerState.rAxis0.x, 0, controllerInit.controllerState.rAxis0.y);
            StartCoroutine(panCoroutine(nVector));
        }

    }

    private void padUnclicked(object sender, ClickedEventArgs e)
    {
        if (axisState != state.off)
        {
            axisState = state.idle;
        }
    }


    void turnOffAxis()
    {
        Destroy(axis);
        axisState = state.off;
    }

    void turnOnAxis()
    {
        cadModel = GameObject.Find("Model Manager").GetComponent<ModelManager>().cadModel;
        axis = Instantiate(axisPrefab);

        Vector3 startLocation = new Vector3(cadModel.transform.position.x, axis.transform.position.y, cadModel.transform.position.z);
        axis.transform.position = startLocation;
        axisState = state.idle;
    }


    void rotateAxisRight()
    {
        cadModel.transform.RotateAround(axis.transform.position, axis.transform.up, Time.deltaTime * 90f);
    }

    void rotateAxisLeft()
    {
        cadModel.transform.RotateAround(axis.transform.position, axis.transform.up, Time.deltaTime * -90f);
    }
   
}
