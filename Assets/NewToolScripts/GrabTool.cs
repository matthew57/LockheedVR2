using UnityEngine;
using System.Collections;

public class GrabTool : MonoBehaviour {

	public Vector3 grabPointA;
	public float handleAngle;
	public int grabAnim;
	public Vector3 BoltPoint;

	public GameObject Bolt;

	GameObject currentBolt;
	public Vector3 getGrabPoint()
	{
		return grabPointA ;
	}


	public void buttonPressed(bool timeNormal)
	{
		
		currentBolt = (GameObject)Instantiate(Bolt,this.transform.position + this.transform.rotation * (BoltPoint ), this.transform.rotation *Quaternion.Euler(90,0,0)  );
		currentBolt.transform.SetParent (this.transform);

	}

	public void buttonReleased(bool timeNormal)
	{
		if (currentBolt) {
			currentBolt.transform.SetParent (null);
		}


	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere ((this.transform.position + this.transform.rotation * (grabPointA )),.04f);
		Gizmos.DrawSphere ((this.transform.position + this.transform.rotation * (BoltPoint )),.04f);
	
	}


}
