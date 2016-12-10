using UnityEngine;
using System.Collections;

public class GrabTool : MonoBehaviour {

	public Vector3 grabPointA;
	public float handleAngle;
	public int grabAnim;

	public Vector3 getGrabPoint()
	{
		return grabPointA ;
	}


	public void buttonPressed()
	{
		Debug.Log ("Button pressed");
	}

	public void buttonReleased()
	{
		Debug.Log ("Button Release");

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere ((this.transform.position + this.transform.rotation * (grabPointA )),.04f);
	
	}


}
