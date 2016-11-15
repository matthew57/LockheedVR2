using UnityEngine;
using System.Collections;

public class GrabTool : MonoBehaviour {

	public Vector3 grabPointA;
	public Vector3 LookAtPoint;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public Vector3 getLookAtPoint()
	{
		return this.transform.position + this.transform.rotation *LookAtPoint;
	}
	public Vector3 getGrabPoint()
	{
		return grabPointA ;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere ((this.transform.position + this.transform.rotation * (grabPointA )),.04f);
		Gizmos.DrawSphere ((this.transform.position + this.transform.rotation * (LookAtPoint)),.04f);


	}
}
