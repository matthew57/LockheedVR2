// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. Drag the 
 * cutting plane game object onto the Cutting Plane slot on the 
 * script. The higher the cutting speed reduction, the slower the 
 * plane will pan when you click up or down on the touchpad of the controller. 
 * The assigned button will turn on/off the cutting plane, while the touchpad 
 * is already assigned to do the rest of the functionality. When you click up/down
 * on the touchpad, it will pan the plane forward or backward on its
 * normal. Clicking left on the touchpad will flip the normal of the plane.
 * Clicking right on the touchpad will toggle snap the plane through the 
 * three main coordinate axis planes (i.e. xz, yz, and xy orientation) */

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CuttingPlane : Tools{

    public GameObject cuttingPlane;
    public int cuttingSpeedReduction = 200;

    public button onOffButton;
    public button panFlipButton;
    private ButtonSubHandler onOff;
    private ButtonSubHandler panFlip;

    public enum state {off , idle, panForward, panBackward};
    public state cPlaneState = state.off;

    [SyncVar(hook = "onOffSwitch")]
    public bool cPlaneOnOff = true;

    private int set = 0;

    // Use this for initialization
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        onOff.clicked = onOffClicked;
        onOff.unclicked = null;

        panFlip.clicked = padClicked;
        panFlip.unclicked = padUnclicked;

        buttonMap.Add(onOffButton, onOff);
        buttonMap.Add(panFlipButton, panFlip);
        
        CmdTurnOffCPlane();
    }





    private void onOffClicked(object sender, ClickedEventArgs e)
    {
        if (cPlaneState == state.off)
        {
            TurnOnCPlane();
        }
        else
        {
            turnOffCPlane();
        }
    }


    IEnumerator panCoroutine()
    {
        Debug.Log("COOOR");
        while (cPlaneState != state.idle && cPlaneState != state.off)
        {
            if (cPlaneState == state.panForward)
            {
                Debug.Log("MOVING");
                PanPlaneForward();
            }
            else if (cPlaneState == state.panBackward)
            {
                Debug.Log("MOVING");
                PanPlaneBackward();
            }
            yield return null;
        }

        yield return null;

        print("MyCoroutine is now finished.");
    }



private void padClicked(object sender, ClickedEventArgs e)
    {
        NetworkIdentity lNetId = cuttingPlane.GetComponent<NetworkIdentity>();
        CmdServerAssignClient(lNetId);

        //DPAD UP BUTTON SETTINGS - MOVE CUTTING PLANE UP ON NORMAL/////
        if (controllerInit.controllerState.rAxis0.y >= 0.5f && controllerInit.controllerState.rAxis0.x >= -0.5f && controllerInit.controllerState.rAxis0.x <= 0.5f)
        {
            Debug.Log("FFFFFFFFFF");
            cPlaneState = state.panForward;
            StartCoroutine("panCoroutine");
        }

        //DPAD DOWN BUTTON SETTINGS - MOVE CUTTING PLANE DOWN ON NORMAL/////
        if (controllerInit.controllerState.rAxis0.y <= -0.5f && controllerInit.controllerState.rAxis0.x >= -0.5f && controllerInit.controllerState.rAxis0.x <= 0.5f)
        {
            Debug.Log("BBBBBBBBB");
            cPlaneState = state.panBackward;
            StartCoroutine("panCoroutine");
        }

        //DPAD LEFT BUTTON SETTINGS - FLIP NORMAL OF CUTTING PLANE/////////////
        else if (controllerInit.controllerState.rAxis0.x <= -0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
        {
            FlipPlaneNormal();
        }

        //DPAD RIGHT BUTTON SETTINGS -TOGGLE THROUGH CUTTING PLANE SNAPPED TO ORIGIN PLANES//////////////////
        else if (controllerInit.controllerState.rAxis0.x >= 0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
        {
            TogglePlaneOriginDir();
        }
        else
        {
            return;
        }
    }

    private void padUnclicked(object sender, ClickedEventArgs e)
    {
        if (cPlaneState == state.panForward)
        {
            Debug.Log("RRRRRRRRRR");
            cPlaneState = state.idle;
        }
        if (cPlaneState == state.panBackward)
        {
            Debug.Log("RRRRRRRRRR");
            cPlaneState = state.idle;
        }
        NetworkIdentity lNetId = cuttingPlane.GetComponent<NetworkIdentity>();
        CmdServerRemoveClient(lNetId);
    }

    void turnOffCPlane()
    {

        CmdTurnOffCPlane();
        cPlaneState = state.off;
    }

    void TurnOnCPlane()
    {
        CmdTurnOnCPlane();
        cPlaneState = state.idle;
    }

    void PanPlaneForward()
    {
        cuttingPlane.transform.position = cuttingPlane.transform.position + (cuttingPlane.gameObject.transform.up) / 200;
        Debug.Log("MOVING CCCCCCCCCCCC");
    }

    void PanPlaneBackward()
    {
        cuttingPlane.transform.position = cuttingPlane.transform.position - (cuttingPlane.gameObject.transform.up) / 200;
        Debug.Log("MOVING CCCCCCCCCCC");
    }

    void FlipPlaneNormal()
    {
        cuttingPlane.gameObject.transform.up = cuttingPlane.gameObject.transform.up * -1;
        cPlaneState = state.idle;
    }

    void TogglePlaneOriginDir()
    {
        if (set == 0)
        {
            cuttingPlane.gameObject.transform.up = Vector3.up;
            set++;
            return;
        }
        if (set == 1)
        {
            cuttingPlane.gameObject.transform.up = Vector3.right;
            set++;
            return;
        }
        if (set == 2)
        {
            cuttingPlane.gameObject.transform.up = Vector3.forward;
            set = 0;
            return;
        }
        cPlaneState = state.idle;
    }

    //command changes sync var
    //client has hook which turns plan off or on

    [Command]
    public void CmdTurnOnCPlane()
    {
        cPlaneOnOff = true;
        //RpcTurnOnCPlane(controlledNetId);
    }

    [Command]
    public void CmdTurnOffCPlane()
    {
        cPlaneOnOff = false;
        //RpcTurnOffCPlane(controlledNetId);
    }

    //[ClientRpc]
    //public void RpcTurnOnCPlane(NetworkIdentity controlledNetId)
    //{
    //    GameObject cuttingPlane = controlledNetId.gameObject;
    //    cuttingPlane.SetActive(true);
        
    //    Shader.SetGlobalFloat("_clip", 1.0f);
    //}

    //[ClientRpc]
    //public void RpcTurnOffCPlane(NetworkIdentity controlledNetId)
    //{
    //    GameObject cuttingPlane = controlledNetId.gameObject;
    //    cuttingPlane.SetActive(false);
    //    Shader.SetGlobalFloat("_clip", 0.0f);
    //}
    //
    void onOffSwitch(bool onOff)
    {
        if (!cuttingPlane)
        {
            cuttingPlane = GameObject.Find("CuttingPlane");
        }
        if(onOff)
        {
            cuttingPlane.GetComponent<BoxCollider>().enabled = true;
            cuttingPlane.GetComponent<MeshRenderer>().enabled = true;
        
            Shader.SetGlobalFloat("_clip", 1.0f);
        }
        else
        {
            cuttingPlane.GetComponent<BoxCollider>().enabled = false;
            cuttingPlane.GetComponent<MeshRenderer>().enabled = false;
            Shader.SetGlobalFloat("_clip", 0.0f);
        }

    }
}
