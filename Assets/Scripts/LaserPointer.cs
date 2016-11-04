// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool allows
 * you to toggle on/off the laser pointer on your remote. However, 
 * regardless of whether you put this script on the left or right 
 * controller, the laser is always on the right hand. This could be changed,
 * but you would have to put the VR_LaserPointer script as well on the left
 * hand of the VRPlayer prefab. It must be noted, this script only turns
 * on/off the laser, the actual laser pointer script and game object is 
 * being done and created on the VR_LaserPointer script which is on the 
 * right hand of the VRPlayer prefab. The assigned button is clicked in 
 * order to toggle the laser on/off */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class LaserPointer : Tools
{
    public enum state { laserOff, laserOn };
    public state laserState = state.laserOff;

    public button onOffButton;

    private ButtonSubHandler onOff;



    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        onOff.clicked = onOffClicked;
        onOff.unclicked = null;

        buttonMap.Add(onOffButton, onOff);
        sub();

    }

    protected  void onOffClicked(object sender, ClickedEventArgs e)
    {
        
        if (laserState == state.laserOff)
        {
            TurnOnCLaser();
        }
        else
        {
            turnOffLaser();
        }
    }

    void turnOffLaser()
    {
        NetworkIdentity lNetId = gameObject.GetComponent<NetworkIdentity>();
        CmdTurnOffLaser(lNetId);
        laserState = state.laserOff;
    }

    void TurnOnCLaser()
    {
        NetworkIdentity lNetId = gameObject.GetComponent<NetworkIdentity>();
        CmdTurnOnLaser(lNetId);
        laserState = state.laserOn;
    }

    [Command]
    public void CmdTurnOnLaser(NetworkIdentity playerNetId)
    {
        RpcTurnOnLaser(playerNetId);
    }

    [Command]
    public void CmdTurnOffLaser(NetworkIdentity playerNetId)
    {
        RpcTurnOffLaser(playerNetId);
    }

    [ClientRpc]
    public void RpcTurnOnLaser(NetworkIdentity playerNetId)
    {
        GameObject player = playerNetId.gameObject;
        player.transform.FindChild("RightHand").GetComponent<VR_LaserPointer>().holder.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcTurnOffLaser(NetworkIdentity playerNetId)
    {
        GameObject player = playerNetId.gameObject;
        player.transform.FindChild("RightHand").GetComponent<VR_LaserPointer>().holder.gameObject.SetActive(false);
    }
}



