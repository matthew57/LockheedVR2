using UnityEngine;
using System.Collections;

public class NewAxisRotation: ITool {

	public GameObject axisPrefab;
	public Vector3 direction;
	public GameObject cadModel;


	private GameObject axis;

	public state axisState = state.off;

	GameObject lastCollided;

	public override bool TriggerClick(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal){
	
		if (axisState == state.off)
		{
			turnOnAxis();
		}
		else
		{
			turnOffAxis();
		}
		return true;
	
	}
	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal){return false;}

	public override bool PadClick (ClickedEventArgs e, bool TimeNormal){

		if (TimeNormal && !playBackDevice) {
			if (!controllerInit.triggerPressed) {
				//Left or right
				if (e.padX < -1 * Mathf.Abs (e.padY) || e.padX > Mathf.Abs (e.padY)) {
					axisState = state.rotate;
					StartCoroutine ("rotate");
				}
			//Up
			else if (e.padY > Mathf.Abs (e.padX)) {
					Vector3 startLocation = new Vector3 (cadModel.transform.position.x, axis.transform.position.y, cadModel.transform.position.z);
					axis.transform.position = startLocation;
				}
			} else {
				axisState = state.moving;
				Vector3 nVector = new Vector3 (controllerInit.controllerState.rAxis0.x, 0, controllerInit.controllerState.rAxis0.y);
				StartCoroutine (panCoroutine (nVector));
			}
		} else {
			PadUnclick (e, true);
		}


		return true;
	}

	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal){
		if (TimeNormal&& !playBackDevice) {
			if (axisState != state.off) {
				axisState = state.idle;
			}
		} else {
		}

		return true;
	}

	public override bool Grip (ClickedEventArgs e, bool TimeNormal){
		if (lastCollided) {
			cadModel = lastCollided;
			GameObject.FindObjectOfType<ModelManager> ().cadModel = lastCollided;

			turnOffAxis ();
			turnOnAxis ();


			return true;
		}

		return false;
	
	}
	public override bool UnGrip(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadTouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUntouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool SteamClicked (ClickedEventArgs e, bool TimeNormal){return false;}

	public override void CollisionEnter (Collider other){
		lastCollided = other.gameObject;

	}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}



	public enum state { off, idle, moving, rotate };



	IEnumerator panCoroutine(Vector3 nVector)
	{
		GameObject.FindObjectOfType<RecordingSystem> ().trackObject (axis);
		while (axisState == state.moving)
		{
			axis.transform.rotation = Quaternion.Euler(axis.transform.rotation.z, controllerInit.transform.rotation.eulerAngles.y, axis.transform.rotation.x);
			axis.transform.Translate(nVector * Time.deltaTime);
			nVector = new Vector3(controllerInit.controllerState.rAxis0.x, 0, controllerInit.controllerState.rAxis0.y);

			yield return null;
		}
		GameObject.FindObjectOfType<RecordingSystem> ().stopTrackObject (axis);
		yield return null;
	}

	IEnumerator rotate()
	{

		GameObject.FindObjectOfType<RecordingSystem> ().trackObject (cadModel);
		while (axisState == state.rotate)
		{
			cadModel.transform.RotateAround(axis.transform.position, axis.transform.up, Time.deltaTime * 90f * controllerInit.controllerState.rAxis0.x);

			yield return null;
		}
		GameObject.FindObjectOfType<RecordingSystem> ().stopTrackObject (cadModel);
		yield return null;

	}




	void turnOffAxis()
	{
		Destroy(axis);
		axisState = state.off;
	}

	void turnOnAxis()
	{
		cadModel = GameObject.FindObjectOfType<ModelManager>().cadModel;
		axis = Instantiate(axisPrefab);

		Vector3 startLocation = new Vector3(cadModel.transform.position.x, axis.transform.position.y, cadModel.transform.position.z);
		axis.transform.position = startLocation;
		axisState = state.idle;
	}







}
