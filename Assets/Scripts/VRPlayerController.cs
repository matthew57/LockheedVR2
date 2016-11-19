// ===================== DOCUMENTATION ======================= //
/* Drag this script onto the VRPlayer prefab. It is within this script that 
 * once the VRPlayer is spawned in the game, it links up to the Vive HMD and 
 * controllers. Although you cannot see your hands nor head in the game, the
 * other users in the game will see them. It is also within this script where all
 * the RPC and Command functions are called in order to achieve networked 
 * gameplay. 
 * Make sure that the VRPlayer prefab is not in the scene before starting the 
 * game. This prefab is spawned through the Network Manager gameobject upon
 * starting the game.  */

using UnityEngine;
using UnityEngine.Networking;

public class VRPlayerController : NetworkBehaviour
{
    //private GameObject cameraRig;
    Transform eyeC;
	Transform leftC;
	Transform rightC;

    private Transform head;
    private Transform left;
    private Transform right;


    public override void OnStartLocalPlayer()
    {      
		Debug.Log ("This " + this.gameObject);
       // cameraRig = GameObject.Find("[CameraRig]");
        eyeC = GameObject.Find("Camera (eye)").transform;
       leftC = GameObject.Find("Controller (left)").transform;
        rightC = GameObject.Find("Controller (right)").transform;
        left = gameObject.transform.Find("LeftHand");
        right = gameObject.transform.Find("RightHand");
        head = gameObject.transform.Find("Head");


        left.GetComponent<MeshRenderer>().enabled = false;
        right.GetComponent<MeshRenderer>().enabled = false;
        head.GetComponent<MeshRenderer>().enabled = false;



        //GameObject.Find("CuttingPlane").SetActive(false);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        head.position = eyeC.position;
        head.rotation = eyeC.rotation;

        left.position = leftC.position;
        left.rotation = leftC.rotation;

        right.position = rightC.position;
        right.rotation = rightC.rotation;
    }
  
}

