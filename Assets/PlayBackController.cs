using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayBackController : MonoBehaviour {
	//private GameObject cameraRig;
	 Transform eyeC;
	private Transform leftC;
	private Transform rightC;

	private Transform head;
	private Transform left;
	private Transform right;


	void Start()
	{      
		// cameraRig = GameObject.Find("[CameraRig]");
		eyeC = GameObject.Find("Camera (eye)B").transform;
		leftC = GameObject.Find("Controller (left)B").transform;
		rightC = GameObject.Find("Controller (right)B").transform;
		left = gameObject.transform.Find("LeftHandB");
		right = gameObject.transform.Find("RightHandB");
		head = gameObject.transform.Find("HeadB");



		left.GetComponent<MeshRenderer>().enabled = false;
		right.GetComponent<MeshRenderer>().enabled = false;
		head.GetComponent<MeshRenderer>().enabled = false;



		//GameObject.Find("CuttingPlane").SetActive(false);
	}

	void Update()
	{
		
	
		head.position = eyeC.position;
		head.rotation = eyeC.rotation;

		left.position = leftC.position;
		left.rotation = leftC.rotation;

		right.position = rightC.position;
		right.rotation = rightC.rotation;
	}

}
