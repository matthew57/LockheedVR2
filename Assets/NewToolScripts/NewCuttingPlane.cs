﻿// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers.
 * Drag the cutting plane game object onto the Cutting Plane slot on the 
 * script. The higher the cutting speed reduction, the slower the 
 * plane will pan when you click up or down on the touchpad of the controller. 
 * The assigned button will turn on/off the cutting plane, while the touchpad 
 * is already assigned to do the rest of the functionality. When you click up/down
 * on the touchpad, it will pan the plane forward or backward on its
 * normal. Clicking left on the touchpad will flip the normal of the plane.
 * Clicking right on the touchpad will toggle snap the plane through the 
 * three main coordinate axis planes (i.e. xz, yz, and xy orientation) */


using UnityEngine;
using System.Collections;

public class NewCuttingPlane : ITool {


	public GameObject cuttingPlane;
	public int cuttingSpeedReduction = 200;



	public enum state {off , idle, panForward, panBackward};
	public state cPlaneState = state.off;

	public bool cPlaneOnOff = true;

	private int set = 0;



	IEnumerator panCoroutine()
	{
	//	Debug.Log("COOOR");
		while (cPlaneState != state.idle && cPlaneState != state.off)
		{
			if (cPlaneState == state.panForward)
			{
				//Debug.Log("MOVING");
				cuttingPlane.transform.position = cuttingPlane.transform.position + (cuttingPlane.gameObject.transform.up) / 150;
			}
			else if (cPlaneState == state.panBackward)
			{
				//Debug.Log("MOVING");
				cuttingPlane.transform.position = cuttingPlane.transform.position - (cuttingPlane.gameObject.transform.up) / 150;
			}
			yield return null;
		}

		yield return null;

		//print("MyCoroutine is now finished.");
	}
		


	public override bool TriggerClick(ClickedEventArgs e){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e){return false;}
		

	public override bool MenuClick (ClickedEventArgs e){
	


		if (cPlaneState == state.off)
		{
			cPlaneState = state.idle;
			onOffSwitch (true);
		}
		else
		{
			onOffSwitch (false);
			cPlaneState = state.off;
		}
		return false;
	
	}



	void onOffSwitch(bool onOff)
	{
		if (!cuttingPlane)
		{
			cuttingPlane = GameObject.Find("CuttingPlane");
		}

		cuttingPlane.GetComponent<BoxCollider>().enabled = onOff;
		cuttingPlane.GetComponent<MeshRenderer>().enabled = onOff;

		if(onOff)
		{
			Shader.SetGlobalFloat("_clip", 1.0f);
		}
		else
		{
			Shader.SetGlobalFloat("_clip", 0.0f);
		}

	}






	public override bool MenuUnclick (ClickedEventArgs e){return false;}

	public override bool PadClick (ClickedEventArgs e){

		//DPAD UP BUTTON SETTINGS - MOVE CUTTING PLANE UP ON NORMAL/////
		if (controllerInit.controllerState.rAxis0.y >= 0.5f && controllerInit.controllerState.rAxis0.x >= -0.5f && controllerInit.controllerState.rAxis0.x <= 0.5f)
		{
			Debug.Log("FFFFFFFFFF");
			cPlaneState = state.panForward;
			StartCoroutine("panCoroutine");
		}

		//DPAD DOWN BUTTON SETTINGS - MOVE CUTTING PLANE DOWN ON NORMAL/////
		if (controllerInit.controllerState.rAxis0.y <= -0.5f && controllerInit.controllerState.rAxis0.x >= -0.5f && controllerInit.controllerState.rAxis0.x <= 0.5f)
		{
			Debug.Log("BBBBBBBBB");
			cPlaneState = state.panBackward;
			StartCoroutine("panCoroutine");
		}

		//DPAD LEFT BUTTON SETTINGS - FLIP NORMAL OF CUTTING PLANE/////////////
		else if (controllerInit.controllerState.rAxis0.x <= -0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
		{
			cuttingPlane.gameObject.transform.up = cuttingPlane.gameObject.transform.up * -1;
			cPlaneState = state.idle;
		}

		//DPAD RIGHT BUTTON SETTINGS -TOGGLE THROUGH CUTTING PLANE SNAPPED TO ORIGIN PLANES//////////////////
		else if (controllerInit.controllerState.rAxis0.x >= 0.5f && controllerInit.controllerState.rAxis0.y >= -0.5f && controllerInit.controllerState.rAxis0.y <= 0.5f)
		{
			TogglePlaneOriginDir();
		}
	
			return true;


	}


	void TogglePlaneOriginDir()
	{
		if (set == 0)
		{
			cuttingPlane.gameObject.transform.up = Vector3.up;
			set++;
			return;
		}
		if (set == 1)
		{
			cuttingPlane.gameObject.transform.up = Vector3.right;
			set++;
			return;
		}
		if (set == 2)
		{
			cuttingPlane.gameObject.transform.up = Vector3.forward;
			set = 0;
			return;
		}
		cPlaneState = state.idle;
	}

	public override bool PadUnclick (ClickedEventArgs e){
		if (cPlaneState == state.panForward)
		{
			Debug.Log("RRRRRRRRRR");
			cPlaneState = state.idle;
		}
		if (cPlaneState == state.panBackward)
		{
			Debug.Log("RRRRRRRRRR");
			cPlaneState = state.idle;
		}
		return true;
	}

	public override bool Grip (ClickedEventArgs e){return false;}
	public override bool UnGrip(ClickedEventArgs e){return false;}
	public override bool PadTouched(ClickedEventArgs e){return false;}
	public override bool PadUntouched(ClickedEventArgs e){return false;}
	public override bool SteamClicked (ClickedEventArgs e){return false;}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}



}