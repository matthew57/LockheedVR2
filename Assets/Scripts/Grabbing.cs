// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool
 * allows you to grab any parts of the model in the scene. The 
 * assigned button is what needs to be pushed and held down once 
 * you collide with an object with the controller in order to grab
 * and hold it. Let go of the button to release. The collider on the
 * remote (called GrabSphere, a tiny ball on front end of remote) must
 * collide with the part in order for it to be grabbed. This script 
 * also creates a copy of the gameobject once it is grabbed which 
 * highlights blue, showing you where the part fits in the assembly
 * after you have taken it out.   */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class Grabbing : Tools
{
    private GameObject collidedObject;
    public enum state { idle, colliding, pickedUp };
    public state grabbingState = state.idle;
    private Transform grabbedObjParent;

    public button resetButton;
    public button grabButton;

    private ButtonSubHandler reset;
    private ButtonSubHandler grab;

    public List<NetworkIdentity> movedObjects = new List<NetworkIdentity>();

    private float dist;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        grab.clicked = grabClicked;
        grab.unclicked = grabUnclicked;

        reset.clicked = resetClicked;
        reset.unclicked = null;

        buttonMap.Add(resetButton, reset);
        buttonMap.Add(grabButton, grab);

        controllerInit.gameObject.GetComponent<CollisionCallback>().triggerEnter += controllerTrigEnter;
        controllerInit.gameObject.GetComponent<CollisionCallback>().triggerExit += controllerTrigExit;
    }

    private  void resetClicked(object sender, ClickedEventArgs e)
    {
        CmdResetModel();
    }



    IEnumerator snapCoroutine()
    {
        Debug.Log("COOOR");
        while ((grabbingState == state.pickedUp))
        {
            if (collidedObject.GetComponent<SnapBack>())
            {
                if (collidedObject.GetComponent<SnapBack>().objectCopy)
                {
                    dist = Vector3.Distance(collidedObject.transform.position, collidedObject.GetComponent<SnapBack>().objectCopy.transform.position);
                    if (dist <= 0.25)
                    {
                        collidedObject.GetComponent<SnapBack>().objectCopy.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else if (dist > 0.25)
                    {
                        collidedObject.GetComponent<SnapBack>().objectCopy.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }
            yield return null;
        }

        yield return null;

        print("MyCoroutine is now finished.");
    }


    protected void grabClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Button press" + grabbingState);
        if (collidedObject == null || collidedObject.tag == "UIObject")
        {
            return;
        }

        Debug.Log("Button press" + grabbingState);
        if (grabbingState ==  state.colliding)
        {
            if (collidedObject.GetComponent<SnapBack>())
            {
                if (!collidedObject.GetComponent<SnapBack>().objectCopy)
                {
                    Debug.Log("creating copy obj");
                    
                    CmdCreateCollidedObjCopy(collidedObject.GetComponent<NetworkIdentity>());
                }
            }
            pickUp(collidedObject);
            StartCoroutine("snapCoroutine");
        }
    }

    protected void grabUnclicked(object sender, ClickedEventArgs e)
    {
        if (grabbingState == state.pickedUp)
        {
            releaseObj(collidedObject);
            Debug.Log("distance is" + dist);
            if (dist <= 0.25 && collidedObject.GetComponent<SnapBack>())
            {
                Debug.Log("calling destory command");
                CmdDestroyCollidedObjCopy(collidedObject.GetComponent<NetworkIdentity>());
            }
        }
    }
    
    private void pickUp(GameObject obj)
    {
        Debug.Log("pickup");
        NetworkIdentity lNetId = obj.GetComponent<NetworkIdentity>();

        CmdServerAssignClient(lNetId);
        grabbedObjParent = obj.transform.parent;
        obj.transform.SetParent(controllerInit.transform);
        grabbingState = state.pickedUp;
    }

    private void releaseObj(GameObject Obj)
    {
       // Debug.Log("release");
        Obj.transform.parent = grabbedObjParent;

        NetworkIdentity lNetId = Obj.GetComponent<NetworkIdentity>();
        CmdServerRemoveClient(lNetId);

        grabbingState = state.colliding;
    }

    public void controllerTrigEnter(object sender, Collider collider)
    {
      //  Debug.Log("Colllllllllision enter" + collider.gameObject.name);
        if(grabbingState == state.idle || grabbingState == state.colliding)
        {
            if(collider.gameObject.GetComponent<LocalPNetworkTransform>())
            {
                CmdEnableNetTransform(collider.gameObject.GetComponent<NetworkIdentity>());
            }
            collidedObject = collider.gameObject;
            grabbingState = state.colliding;
        }
    }

    public void controllerTrigExit(object sender, Collider collider)
    {
       // Debug.Log("Colllllllllision enter" + collider.gameObject.name);
        if (grabbingState != state.pickedUp)
        {
            if (collider.gameObject.GetComponent<LocalPNetworkTransform>())
            {
                CmdDisableNetTransform(collider.gameObject.GetComponent<NetworkIdentity>());
            }
            grabbingState = state.idle;
        }
    }



    [Command]
    public void CmdEnableNetTransform(NetworkIdentity controlledNetId)
    {
        RpcEnableNetTransform(controlledNetId);
    }

    [Command]
    public void CmdDisableNetTransform(NetworkIdentity controlledNetId)
    {
        RpcDisableNetTransform(controlledNetId);
    }

    [ClientRpc]
    public void RpcEnableNetTransform(NetworkIdentity controlledNetId)
    {
        GameObject collidedObj = controlledNetId.gameObject;
        collidedObj.GetComponent<LocalPNetworkTransform>().enabled = true;
    }

    [ClientRpc]
    public void RpcDisableNetTransform(NetworkIdentity controlledNetId)
    {
        GameObject collidedObj = controlledNetId.gameObject;
        collidedObj.GetComponent<LocalPNetworkTransform>().enabled = false;
    }

    [Command]
    public void CmdCreateCollidedObjCopy(NetworkIdentity controlledNetId)
    {
        RpcCreateCollidedObjCopy(controlledNetId);
    }

    [Command]
    public void CmdDestroyCollidedObjCopy(NetworkIdentity controlledNetId)
    {
        RpcDestroyCollidedObjCopy(controlledNetId);
    }

    [ClientRpc]
    public void RpcCreateCollidedObjCopy(NetworkIdentity controlledNetId)
    {
        GameObject collidedObj = controlledNetId.gameObject;
        collidedObj.GetComponent<SnapBack>().makeCopy();      
        movedObjects.Add(controlledNetId);
    }

    [ClientRpc]
    public void RpcDestroyCollidedObjCopy(NetworkIdentity controlledNetId)
    {
        GameObject collidedObj = controlledNetId.gameObject;
        collidedObj.GetComponent<SnapBack>().destroyCopy();
        movedObjects.Remove(controlledNetId);
    }

    [Command]
    public void CmdResetModel()
    {
        RpcResetModel();
    }

    [ClientRpc]
    public void RpcResetModel()
    {
  
        foreach (NetworkIdentity netID in movedObjects)
        {
            netID.gameObject.GetComponent<SnapBack>().destroyCopy();
        }
        movedObjects.Clear();
    }



}

