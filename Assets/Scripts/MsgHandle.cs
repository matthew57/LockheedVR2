using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MsgHandle : MonoBehaviour {

	// Use this for initialization
	void Start () {

      //  Debug.Log("HHP");
        NetworkServer.RegisterHandler(105, LocalPNetworkTransform.HandleTransform);

    }

    void test(NetworkMessage netMsg)
    {
        LocalPNetworkTransform.HandleTransform(netMsg);
    }
    // Update is called once per frame

}
