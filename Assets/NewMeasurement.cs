using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class NewMeasurement : ITool {





	public GameObject measurementPrefab;
	private GameObject measurement;
	private List<GameObject> allMeasurements = new List<GameObject> ();
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

	public enum state { idle, start, end };
	public state measureState = state.idle;






	public override void TriggerClick(ClickedEventArgs e){
		Debug.Log ("Measurement pressed");

		if (measureState == state.idle)
		{
			measureState = state.start;
			measurement = Instantiate(measurementPrefab);
			allMeasurements.Add (measurement);
			foreach (var child in measurement.GetComponentsInChildren<Transform>())
			{
				switch (child.gameObject.name)
				{
				case ("StartSphere"):
					startsphere = child.gameObject;                       
					break;
				case ("EndSphere"):
					endsphere = child.gameObject;
					child.GetComponent<MeshRenderer>().enabled = false;
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
			StartCoroutine("measureCoroutine" , false);     
		}
		else if (measureState == state.start)
		{
			measureState = state.end;

			endsphere.GetComponent<MeshRenderer>().enabled = true;
			endsphere.transform.position = grabSphere.transform.position;

			measureLine.GetComponent<LineRenderer>().SetPosition(1, endsphere.transform.position);

			distancebox.transform.position = ((startsphere.transform.position + endsphere.transform.position) / 2);
			dist = Vector3.Distance(startsphere.transform.position, endsphere.transform.position);
			dist = dist * 39.3701f;
			dist_string = dist.ToString("0.00");
			texta.text = "Distance: " + dist_string + " inches";
			textb.text = "Distance: " + dist_string + " inches";
			measureState = state.idle;
		
		}
		else if (measureState == state.end)
		{
			//Destroy(measurement);
			measureState = state.idle;
		}


	}
	public override void TriggerUnclick (ClickedEventArgs e){}
	public override void MenuClick (ClickedEventArgs e){}
	public override void MenuUnclick (ClickedEventArgs e){}
	public override void PadClick (ClickedEventArgs e){


		if (allMeasurements.Count > 0) {
			GameObject temp = allMeasurements [0];
			allMeasurements.Remove (temp);
			Destroy (temp);
		}

	}
	public override void PadUnclick (ClickedEventArgs e){}




	//USED FOR SNAPPING
	public override void Grip (ClickedEventArgs e){
		if (measureState == state.idle)
		{
			measureState = state.start;
			measurement = Instantiate(measurementPrefab);
			allMeasurements.Add (measurement);
			foreach (var child in measurement.GetComponentsInChildren<Transform>())
			{
				switch (child.gameObject.name)
				{
				case ("StartSphere"):
					startsphere = child.gameObject;                       
					break;
				case ("EndSphere"):
					endsphere = child.gameObject;
					child.GetComponent<MeshRenderer>().enabled = false;
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
			StartCoroutine("measureCoroutine",true);     
		}
		else if (measureState == state.start)
		{
			measureState = state.end;

			endsphere.GetComponent<MeshRenderer>().enabled = true;

			endsphere.transform.position =snapToEdge (grabSphere.transform.position);

			measureLine.GetComponent<LineRenderer>().SetPosition(1, endsphere.transform.position);

			distancebox.transform.position = ((startsphere.transform.position + endsphere.transform.position) / 2);
			dist = Vector3.Distance(startsphere.transform.position, endsphere.transform.position);
			dist = dist * 39.3701f;
			dist_string = dist.ToString("0.00");
			texta.text = "Distance: " + dist_string + " inches";
			textb.text = "Distance: " + dist_string + " inches";
			measureState = state.idle;

		}


	}
	public override void UnGrip(ClickedEventArgs e){}
	public override void PadTouched(ClickedEventArgs e){}
	public override void PadUntouched(ClickedEventArgs e){}
	public override void SteamClicked (ClickedEventArgs e){}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}

	public override void stopUsing (){

		if (measurement)
		{
			measureState = state.idle;
			foreach (GameObject obj in allMeasurements) {
				Destroy (obj);
			}
		}

	}
	public override void startUsing(){}


	 void Start()
	{
		base.Start();

		grabSphere = controllerInit.gameObject.transform.Find("GrabSphere").gameObject;
		camera_eyes = GameObject.Find("Camera (eye)");
	}


	IEnumerator measureCoroutine(bool snap)
	{if (!snap) {
			startsphere.transform.position = grabSphere.transform.position;
		} else {
			startsphere.transform.position = snapToEdge (grabSphere.transform.position);
		}
		while (measureState != state.idle)
		{
			if (measureState == state.start)
			{

				measureLine.GetComponent<LineRenderer>().SetPosition(0, startsphere.transform.position);
				measureLine.GetComponent<LineRenderer>().SetPosition(1, grabSphere.transform.position);

				distancebox.transform.position = ((startsphere.transform.position + grabSphere.transform.position) / 2);
				distancebox.transform.LookAt(camera_eyes.transform.position);
				distancebox.transform.Rotate(0, 180, 0);

				dist = Vector3.Distance(startsphere.transform.position, grabSphere.transform.position);
				dist = dist * 39.3701f;
				dist_string = dist.ToString("0.00");
				texta.text = "Distance: " + dist_string + " inches";
				textb.text = "Distance: " + dist_string + " inches";
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
			float tempDist = Vector3.Distance (tempPoint, point);
			if (tempDist < distance) {
				distance = tempDist;
				closestPoint = tempPoint;
			}
		
		
		}
		if (closestPoint == Vector3.zero || distance > .1) {
		
			return point;
		} else {
			return closestPoint;
		}



	}



}
