// ===================== DOCUMENTATION ======================= //
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

public class NewCuttingPlane : NewGrabbing{


	public GameObject cuttingPlane;
	public int cuttingSpeedReduction = 200;



	public enum state {off , idle, pan};
	public state cPlaneState = state.idle;

	public bool cPlaneOnOff = true;

	private int set = 0;


	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal){
	


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
			cuttingPlane = GameObject.FindObjectOfType<ClippingPlane> ().gameObject;//.Find("CuttingPlane");
		}
			
		//cuttingPlane.GetComponent<BoxCollider>().enabled = onOff;
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




	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal){return false;}

	public override bool PadClick (ClickedEventArgs e, bool TimeNormal){

	

		//DPAD UP or down BUTTON SETTINGS - MOVE CUTTING PLANE UP ON NORMAL/////

		if(e.padY > Mathf.Abs(e.padX) ||e.padY < -1*Mathf.Abs(e.padX) )
		{
			
			cPlaneState = state.pan;
			StartCoroutine("panCoroutine");
		}

	
		//DPAD LEFT BUTTON SETTINGS - FLIP NORMAL OF CUTTING PLANE/////////////
		else if(e.padX <  -1 * Mathf.Abs(e.padY)) 
		{
			cuttingPlane.gameObject.transform.up = cuttingPlane.gameObject.transform.up * -1;
			cPlaneState = state.idle;
		}

		//DPAD RIGHT BUTTON SETTINGS -TOGGLE THROUGH CUTTING PLANE SNAPPED TO ORIGIN PLANES//////////////////
		else if(e.padX > Mathf.Abs(e.padY))
		{
			TogglePlaneOriginDir();
		}
	
			return true;

	}


	IEnumerator panCoroutine()
	{
		//	Debug.Log("COOOR");
		while (cPlaneState != state.idle && cPlaneState != state.off)
		{
			cuttingPlane.transform.position = cuttingPlane.transform.position + (cuttingPlane.gameObject.transform.up * controllerInit.controllerState.rAxis0.y) / 130;

			yield return null;
		}

		yield return null;

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

	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal){
		if (cPlaneState == state.pan)
		{
			cPlaneState = state.idle;
		}
	
		return true;
	}

	public override void startUsing()
	{
		onOffSwitch (false);

	}


}
