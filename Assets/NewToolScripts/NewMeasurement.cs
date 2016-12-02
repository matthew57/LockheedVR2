using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class NewMeasurement : ITool {





	public GameObject measurementPrefab;
	private GameObject measurement;
	private List<measureInfo> allMeasurements = new List<measureInfo> ();

	private List<measureInfo> deletedMeasurements = new List<measureInfo> ();
	private GameObject startsphere;
	private GameObject endsphere;
	private GameObject measureLine;

	private float dist;
	private string dist_string;
	private GameObject grabSphere;
	private GameObject camera_eyes;

	//DISTANCE TEXT UPDATER//
	TextMesh texta;
	TextMesh textb;
	private GameObject distancebox;

	private measureInfo previousMeasure;
	private measureInfo closestMeasure;

	public enum state { idle, start, end };
	public state measureState = state.idle;


	void Update()
	{

		if (subscribed && allMeasurements.Count > 0 && measureState == state.idle) {
			previousMeasure = closestMeasure;
			closestMeasure = getClosest ();

			if (previousMeasure && previousMeasure != closestMeasure) {

				previousMeasure.myLine.material = previousMeasure.yellowShade;//.SetColors (Color.yellow, Color.yellow);
				closestMeasure.myLine.material = closestMeasure.greebShade;//.SetColors (Color.green, Color.green);
			}


		}

	}



	public override bool TriggerClick(ClickedEventArgs e, bool TimeNormal){
		if (TimeNormal) {
			if (measureState == state.idle) {	
				if (closestMeasure) {
					closestMeasure.myLine.material = closestMeasure.yellowShade;
				}
				measureState = state.start;
				measurement = Instantiate (measurementPrefab);

				if (playBackDevice) {
					measurement.layer = 10;
				}
				allMeasurements.Add (measurement.GetComponent<measureInfo> ());
				setSpheres ();
				StartCoroutine (measureCoroutine (false, false));     
			} else if (measureState == state.start) {
				finalCLick (false);
		
			} else if (measureState == state.end) {
				measureState = state.idle;
			}
		} else {
			if (measureState == state.idle) {
				MenuClick (e, true);
			} else {
				PadClick (e, true);
			}
		}
		return true;

	}

	//Rearrange existing measurement
	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal){
		if (TimeNormal) {
			
			if (measureState == state.idle) {
					
				if (closestMeasure) {
					closestMeasure.myLine.material = closestMeasure.yellowShade;
				}
				measureState = state.start;
				measurement = getClosest ().gameObject;
				if (!measurement) {
					return false;
				}
			
				setSpheres ();
				if (Vector3.Distance (grabSphere.transform.position, endsphere.transform.position) > Vector3.Distance (grabSphere.transform.position, startsphere.transform.position)) {
					GameObject tempStart = startsphere;

					startsphere = endsphere;
					startsphere.name = "EndSphere";

					endsphere = tempStart;
					endsphere.name = "StartSphere";
			

				} else {
	
					endsphere.transform.position = startsphere.transform.position;
				}

				StartCoroutine (measureCoroutine (false, true));  
			} else if (measureState == state.start) {
				finalCLick (false);

			} else if (measureState == state.end) {

				measureState = state.idle;
			}
		} else {
			if (measureState == state.idle) {

				Debug.Log ("Was idle");
				MenuClick (e, true);

			} else if (measureState == state.start) {
				Debug.Log ("Was Start ");
				TriggerClick (e, true);
			
			} else if (measureState == state.end) {
				Debug.Log ("Was End");
				TriggerClick (e, true);
			}
		
		}
		return true;
	}

	public override bool PadClick (ClickedEventArgs e, bool TimeNormal){


		measureInfo bestObj = getClosest ();
		if (TimeNormal) {
			if (bestObj) {
		
				allMeasurements.Remove (bestObj);
				if (playBackDevice) {
					deletedMeasurements.Add (bestObj);
					bestObj.gameObject.SetActive (false);
				} else {
					Destroy (bestObj.gameObject);
				}
				measureState = state.idle;
			}
		} else {

			float bestDistance = 100000;
			foreach (measureInfo obj in deletedMeasurements) {

					float tempDist = distanceToLine (obj.startSphere.transform.position, obj.endSphere.transform.position, grabSphere.transform.position);// = Vector3.Distance (obj.startSphere.transform.position, grabSphere.transform.position);
					if (tempDist < bestDistance) {
						bestObj = obj;
						bestDistance = tempDist;

					}

				}
			bestObj.gameObject.SetActive (true);
			deletedMeasurements.Remove (bestObj);
			allMeasurements.Add (bestObj);

		
		}

		return true;
	}






	//USED FOR SNAPPING
	public override bool Grip (ClickedEventArgs e, bool TimeNormal){
	
		if (TimeNormal) {
			if (measureState == state.idle) {
				measureState = state.start;
				measurement = Instantiate (measurementPrefab);
				allMeasurements.Add (measurement.GetComponent<measureInfo> ());
				setSpheres ();



				StartCoroutine (measureCoroutine (true, false));  
			} else if (measureState == state.start) {
				finalCLick (true);

			}
		} else {
			if (measureState == state.idle) {
				MenuClick (e, true);
			} else {
				PadClick (e, true);
			}

		}

		return true;
	}


	public void setSpheres()
	{
	
		foreach (var child in measurement.GetComponentsInChildren<Transform>())
		{
			switch (child.gameObject.name)
			{
			case ("StartSphere"):
				startsphere = child.gameObject;                       
				break;
			case ("EndSphere"):
				endsphere = child.gameObject;
				child.GetComponent<MeshRenderer>().enabled = true;
				break;
			case ("MeasureLine"):
				measureLine = child.gameObject;
				break;
			case ("DistanceBox"):
				distancebox = child.gameObject;
				texta = distancebox.GetComponent("TextMesh") as TextMesh;
				break;
			case ("ShadowBox"):
				distancebox = child.gameObject;
				textb = distancebox.GetComponent("TextMesh") as TextMesh;
				break;
			default:
				break;
			}
		}
	}

	public void finalCLick (bool snapTo){
		measureState = state.end;
		closestMeasure = endsphere.GetComponentInParent<measureInfo> ();
		closestMeasure.myLine.material = closestMeasure.greebShade;
		endsphere.GetComponent<MeshRenderer>().enabled = true;
		closestMeasure = null;
		if (snapTo) {
			endsphere.transform.position = snapToEdge (grabSphere.transform.position);
		} else {
			endsphere.transform.position = grabSphere.transform.position;
		}

		measureLine.GetComponent<LineRenderer>().SetPosition(1, endsphere.transform.position);

		distancebox.transform.position = ((startsphere.transform.position + endsphere.transform.position) / 2);
		dist = Vector3.Distance(startsphere.transform.position, endsphere.transform.position);
		dist = dist * 39.3701f;
		dist_string = dist.ToString("0.00");
		texta.text = "Dist: " + dist_string + " inches";
		textb.text = "Dist: " + dist_string + " inches";
		measureState = state.idle;

	}

	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool UnGrip(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadTouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUntouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool SteamClicked (ClickedEventArgs e, bool TimeNormal){return false;}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}

	public override void stopUsing (){

		if (measurement)
		{
			measureState = state.idle;
		}

	}
	public override void startUsing(){}


	 void Start()
	{
		base.Awake();

		grabSphere = controllerInit.gameObject.transform.Find("GrabSphere").gameObject;
		camera_eyes = GameObject.Find("Camera (eye)");
	}


	IEnumerator measureCoroutine(bool snap, bool Rearrange)
	{
		if (Rearrange) {

		
		} else {
			if (!snap) {
				startsphere.transform.position = grabSphere.transform.position;
			} else {
				startsphere.transform.position = snapToEdge (grabSphere.transform.position);
			}
		}
		LineRenderer myLine = measureLine.GetComponent<LineRenderer> ();
		while (measureState != state.idle)
		{
			if (measureState == state.start)
			{

				myLine.SetPosition(0, startsphere.transform.position);
				myLine.SetPosition(1, grabSphere.transform.position);

				distancebox.transform.position = ((startsphere.transform.position + grabSphere.transform.position) / 2);
				distancebox.transform.LookAt(camera_eyes.transform.position);
				distancebox.transform.Rotate(0, 180, 0);

				dist = Vector3.Distance(startsphere.transform.position, grabSphere.transform.position);
				dist = dist * 39.3701f;
				dist_string = dist.ToString("0.00");
				texta.text = "Dist: " + dist_string + " inches";
				textb.text = "Dist: " + dist_string + " inches";
			}
			else if (measureState == state.end)
			{
				distancebox.transform.LookAt(camera_eyes.transform.position);
				distancebox.transform.Rotate(0, 180, 0);
			}
			yield return null;
		}
		yield return null;
	}



	public Vector3 snapToEdge(Vector3 point)
	{
		Vector3 closestPoint = Vector3.zero;
		float distance = 1000000;;

		foreach (GameObject col in GameObject.FindGameObjectsWithTag("SceneObject")) {


		
			Vector3 tempPoint = col.GetComponent<Collider>().ClosestPointOnBounds (point);

			Vector3 origin = new Vector3 (col.transform.position.x -point.x , col.transform.position.y -  point.y,  col.transform.position.z - point.z );
			origin.Normalize ();

			Ray ray = new Ray (point, origin);
			RaycastHit hitInfo = new RaycastHit ();
			if (col.GetComponent<Collider> ().Raycast(ray, out hitInfo, 1000000)) {
			

				tempPoint = hitInfo.point;

			}

			float tempDist = Vector3.Distance (tempPoint, point);
			if (tempDist < distance) {
				distance = tempDist;
				closestPoint = tempPoint;
			}
		
		
		}
		if (closestPoint == Vector3.zero || distance > .1) {
		
			Debug.Log ("Returning point");
			return point;
		} else {
			Debug.Log ("New Point");
			return closestPoint;
		}



	}



	public measureInfo getClosest()
	{
		float bestDistance = 100000;
		measureInfo bestObj = null;

		foreach (measureInfo obj in allMeasurements) {
			if (obj == null) {
				continue;
			}
			float tempDist = distanceToLine (obj.startSphere.transform.position, obj.endSphere.transform.position, grabSphere.transform.position);// = Vector3.Distance (obj.startSphere.transform.position, grabSphere.transform.position);
			if (tempDist < bestDistance) {
				bestObj = obj;
				bestDistance = tempDist;

			}

		}
		return bestObj;
	}


	public float distanceToLine( Vector3 vA, Vector3 vB, Vector3 vPoint)
	{
		var vVector1 = vPoint - vA;
		var vVector2 = (vB - vA).normalized;

		var d = Vector3.Distance(vA, vB);
		var t = Vector3.Dot(vVector2, vVector1);

		if (t <= 0)
			Vector3.Distance (vA, vPoint);

		if (t >= d)
			Vector3.Distance (vB, vPoint);

		var vVector3 = vVector2 * t;

		return  Vector3.Distance ((vA + vVector3), vPoint);
	}


}
