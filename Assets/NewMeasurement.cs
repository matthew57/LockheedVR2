using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class NewMeasurement : ITool {




	public override void TriggerClick(ClickedEventArgs e){
		if (measureState == state.idle)
		{
			measureState = state.start;
			measurement = Instantiate(measurementPrefab);

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
					text = distancebox.GetComponent("TextMesh") as TextMesh;
					break;
				default:
					break;
				}
			}
			StartCoroutine("measureCoroutine");     
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
			text.text = "Distance: " + dist_string + " inches";
		}
		else if (measureState == state.end)
		{
			Destroy(measurement);
			measureState = state.idle;
		}


	}
	public override void TriggerUnclick (ClickedEventArgs e){}
	public override void MenuClick (ClickedEventArgs e){}
	public override void MenuUnclick (ClickedEventArgs e){}
	public override void PadClick (ClickedEventArgs e){}
	public override void PadUnclick (ClickedEventArgs e){}
	public override void Grip (ClickedEventArgs e){}
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
			Destroy(measurement);
		}

	}
	public override void startUsing(){}





	public GameObject measurementPrefab;
	private GameObject measurement;
	private GameObject startsphere;
	private GameObject endsphere;
	private GameObject measureLine;

	private float dist;
	private string dist_string;
	private GameObject grabSphere;
	private GameObject camera_eyes;

	//DISTANCE TEXT UPDATER//
	TextMesh text;
	private GameObject distancebox;

	public enum state { idle, start, end };
	public state measureState = state.idle;


	 void Start()
	{
		base.Start();

		grabSphere = controllerInit.gameObject.transform.Find("GrabSphere").gameObject;
		camera_eyes = GameObject.Find("Camera (eye)");
	}


	IEnumerator measureCoroutine()
	{
		startsphere.transform.position = grabSphere.transform.position;

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
				text.text = "Distance: " + dist_string + " inches";
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





}
