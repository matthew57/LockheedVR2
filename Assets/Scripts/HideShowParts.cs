using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class HideShowParts : NetworkBehaviour
{
   
    private bool viewhide = false;
    private GameObject viewobject;

  
    private GameObject collidedObject;
    private Transform myPlayer;


    public enum state { idle, colliding, hidden};
    public state viewState = state.idle;

    //void Update()
    //{

    //    if (myPlayer == null)
    //    {
    //        myPlayer = gameObject.transform.parent.Find("Camera (eye)").Find("VRPlayer(Clone)");
    //        return;
    //    }

    //    switch (viewState)
    //    {
    //        case state.idle:
    //            break;

    //        case state.colliding:
    //            Debug.Log("colliding in hidden state");
    //            if (controller.GetPressDown(Touchpad) && viewState == state.colliding && controller.GetAxis().x >= 0.5f && controller.GetAxis().y >= -0.5f && controller.GetAxis().y <= 0.5f)
    //            {
    //                hide(collidedObject);
    //            }
    //            break;

    //        case state.hidden:
    //            Debug.Log("hidden");
    //            if (controller.GetPressDown(Touchpad) && viewhide ==true && controller.GetAxis().x >= 0.5f && controller.GetAxis().y >= -0.5f && controller.GetAxis().y <= 0.5f)
    //            {
    //                show(collidedObject);
    //            }
    //            break;

    //        default:
    //            break;
    //    }
    //}


    //private void hide(GameObject obj)
    //{
    //    Debug.Log("hide");
    //    NetworkInstanceId lNetId = obj.GetComponent<NetworkIdentity>().netId;

    //    myPlayer.GetComponent<VRPlayerController>().CmdServerAssignClient(lNetId);

    //    collidedObject.GetComponent<MeshRenderer>().enabled = false;
    //    viewhide = true;

    //    viewState = state.hidden;
    //}


    //private void show(GameObject Obj)
    //{
    //    Debug.Log("show");
        

    //    NetworkInstanceId lNetId = Obj.GetComponent<NetworkIdentity>().netId;
    //    myPlayer.GetComponent<VRPlayerController>().CmdServerRemoveClient(lNetId);

    //    collidedObject.GetComponent<MeshRenderer>().enabled = true;
    //    viewhide = false;

    //    viewState = state.colliding;
    //}


    //private void OnTriggerEnter(Collider collider)
    //{
    //    Debug.Log("Colllision");
    //    if (viewState == state.idle || viewState == state.colliding)
    //    {
    //        collidedObject = collider.gameObject;
    //        viewState = state.colliding;
    //    }
    //}

    //private void OnTriggerExit(Collider collider)
    //{
    //    if (viewState != state.hidden)
    //    {
    //        viewState = state.idle;
    //    }
    //}

}
