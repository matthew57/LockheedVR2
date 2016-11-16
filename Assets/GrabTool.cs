using UnityEngine;
using System.Collections;

public class GrabTool : MonoBehaviour {

	public Vector3 grabPointA;
	public float handleAngle;


	public Vector3 getGrabPoint()
	{
		return grabPointA ;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere ((this.transform.position + this.transform.rotation * (grabPointA )),.04f);
	
	}
}
