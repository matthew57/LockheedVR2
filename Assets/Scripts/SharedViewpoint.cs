using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class SharedViewpoint : Tools
{

    public enum state { unShared, shared };
    public state viewState = state.unShared;
    private Transform lPlayer;
    private Transform nonLPlayer;

    // Use this for initialization

    //protected override void buttonPressEvent(object sender, ClickedEventArgs e)
    //{
    //    if (lPlayer == null)
    //    {
    //        var players = GameObject.FindGameObjectsWithTag("Player");

    //        foreach (GameObject player in players)
    //        {
    //            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
    //                lPlayer = player.transform;
    //        }

    //        if (lPlayer == null)
    //        {
    //            return;
    //        }
    //    }

    //    //Ok I need to put some code here that Finds the other player head. Not sure how to do this
    //    var Players = GameObject.FindGameObjectsWithTag("Player");
    //    foreach (GameObject player in Players)
    //    {
    //        if (!player.GetComponent<NetworkIdentity>().isLocalPlayer)
    //        {
    //            nonLPlayer = player.transform;
    //        }
    //    }
       

     


    //    //if (viewState == state.unShared)
    //    //{
    //    //    shareView();
    //    //}
    //    //else
    //    //{
    //    //    unShareView();
    //    //}

    //}



    void Update()
    {

    }


    //void shareView()
    //{
    //    NetworkIdentity lNetId = cuttingPlane.GetComponent<NetworkIdentity>();
    //    lPlayer.GetComponent<VRPlayerController>().CmdTurnOffCPlane(lNetId);
    //    viewState = state.shared;
    //}

    //void unShareView()
    //{

    //    NetworkIdentity lNetId = cuttingPlane.GetComponent<NetworkIdentity>();
    //    lPlayer.GetComponent<VRPlayerController>().CmdTurnOnCPlane(lNetId);
    //    viewState = state.unShared;
    //}
}
