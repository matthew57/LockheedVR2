using UnityEngine;
using System.Collections;

public class NewAxisRotation: ITool {

	public GameObject axisPrefab;
	public Vector3 direction;
	private GameObject cadModel;


	private GameObject axis;


	public override void TriggerClick(ClickedEventArgs e){}
	public override void TriggerUnclick (ClickedEventArgs e){}
	public override void MenuClick (ClickedEventArgs e){}
	public override void MenuUnclick (ClickedEventArgs e){}

	public override void PadClick (ClickedEventArgs e){

		Debug.Log("rotate");
		if (controllerInit.triggerPressed)
		{
			Debug.Log("t Pressed");
			//Left
			if (controllerInit.controllerState.rAxis0.x <= -0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
			{
				axisState = state.rotateLeft;
				StartCoroutine("rotate");
			}

			//DPAD RIGHT BUTTON SETTINGS -TOGGLE THROUGH CUTTING PLANE SNAPPED TO ORIGIN PLANES//////////////////
			else if (controllerInit.controllerState.rAxis0.x >= 0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
			{
				axisState = state.rotateRight;
				StartCoroutine("rotate");
			}

			//Up
			else if (controllerInit.controllerState.rAxis0.y >= 0.5f && controllerInit.controllerState.rAxis0.x >= -0.5f && controllerInit.controllerState.rAxis0.x <= 0.5f)
			{
				Vector3 startLocation = new Vector3(cadModel.transform.position.x, axis.transform.position.y, cadModel.transform.position.z);
				axis.transform.position = startLocation;
			}
		}

		else
		{
			axisState = state.moving;
			Vector3 nVector = new Vector3(controllerInit.controllerState.rAxis0.x, 0, controllerInit.controllerState.rAxis0.y);
			StartCoroutine(panCoroutine(nVector));
		}
	}

	public override void PadUnclick (ClickedEventArgs e){
		if (axisState != state.off)
		{
			axisState = state.idle;
		}
	}

	public override void Grip (ClickedEventArgs e){}
	public override void UnGrip(ClickedEventArgs e){}
	public override void PadTouched(ClickedEventArgs e){}
	public override void PadUntouched(ClickedEventArgs e){}
	public override void SteamClicked (ClickedEventArgs e){}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}



	public enum state { off, idle, moving, rotateRight, rotateLeft };
	public state axisState = state.off;



	private void onOffClicked(object sender, ClickedEventArgs e)
	{
		if (axisState == state.off)
		{
			turnOnAxis();
		}
		else
		{
			turnOffAxis();
		}
	}


	IEnumerator panCoroutine(Vector3 nVector)
	{
		Debug.Log("COOOR");
		while (axisState == state.moving)
		{

			axis.transform.rotation = Quaternion.Euler(axis.transform.rotation.z, controllerInit.transform.rotation.eulerAngles.y, axis.transform.rotation.x);

			axis.transform.Translate(nVector * Time.deltaTime);

			nVector = new Vector3(controllerInit.controllerState.rAxis0.x, 0, controllerInit.controllerState.rAxis0.y);

			yield return null;
		}

		yield return null;

		print("MyCoroutine is now finished.");
	}

	IEnumerator rotate()
	{
		Debug.Log("COOOR");
		while (axisState == state.rotateRight || axisState == state.rotateLeft)
		{

			if (axisState == state.rotateRight)
			{
				rotateAxisRight();
			}
			else if(axisState == state.rotateLeft)
			{
				rotateAxisLeft();
			}

			yield return null;
		}

		yield return null;

		print("MyCoroutine is now finished.");
	}




	void turnOffAxis()
	{
		Destroy(axis);
		axisState = state.off;
	}

	void turnOnAxis()
	{
		cadModel = GameObject.Find("Model Manager").GetComponent<ModelManager>().cadModel;
		axis = Instantiate(axisPrefab);

		Vector3 startLocation = new Vector3(cadModel.transform.position.x, axis.transform.position.y, cadModel.transform.position.z);
		axis.transform.position = startLocation;
		axisState = state.idle;
	}


	void rotateAxisRight()
	{
		cadModel.transform.RotateAround(axis.transform.position, axis.transform.up, Time.deltaTime * 90f);
	}

	void rotateAxisLeft()
	{
		cadModel.transform.RotateAround(axis.transform.position, axis.transform.up, Time.deltaTime * -90f);
	}





}
