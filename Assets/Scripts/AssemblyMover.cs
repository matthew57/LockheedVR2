// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool allows
 * you to move the entire model assembly all at once, as though you are
 * sticking a fork in the assembly and moving it around all together. 
 * Click and hold the assigned button in order to effectively grab the
 * model assembly and move it around with your controller. Release the 
 * assigned button in order to release the model assembly from movement. */

 //LET IT BE NOTED THAT THIS TOOL DOESN'T WORK OVER NETWORK - TOO MUCH DATA
 //IT CAN'T HANDLE THAT MUCH DATA BEING TRANSFERRED BETWEEN NETWORKED USERS

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class AssemblyMover : Tools
{

    public enum state { idle, movingAssembly };
    public state grabbingState = state.idle;

    public button moveButton;

    private ButtonSubHandler move;



    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        move.clicked = moveClicked;
        move.unclicked = moveUnlicked;

        buttonMap.Add(moveButton, move);
        sub();

    }

    protected void moveClicked(object sender, ClickedEventArgs e)
    {
        bool padPressed = controllerInit.padPressed;
        if (!padPressed)
        {
            GameObject cadModel = GameObject.Find("Model Manager").GetComponent<ModelManager>().cadModel;
            CmdServerAssignClient(cadModel.GetComponent<NetworkIdentity>());
     
            cadModel.transform.SetParent(controllerInit.transform);
        }
    }

    protected void moveUnlicked(object sender, ClickedEventArgs e)
    {
        bool padPressed = controllerInit.padPressed;
        if (!padPressed)
        {
            GameObject cadModel = GameObject.Find("Model Manager").GetComponent<ModelManager>().cadModel;
            CmdServerRemoveClient(cadModel.GetComponent<NetworkIdentity>());

            cadModel.transform.SetParent(null);
        }
    }


   
}


