using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class NewGrabbing : ITool {


// ===================== DOCUMENTATION ======================= //
/* Drag this script onto either one of the controllers. This tool
 * allows you to grab any parts of the model in the scene. The 
 * assigned button is what needs to be pushed and held down once 
 * you collide with an object with the controller in order to grab
 * and hold it. Let go of the button to release. The collider on the
 * remote (called GrabSphere, a tiny ball on front end of remote) must
 * collide with the part in order for it to be grabbed. This script 
 * also creates a copy of the gameobject once it is grabbed which 
 * highlights blue, showing you where the part fits in the assembly
 * after you have taken it out.   */


	private GameObject collidedObject;
	public enum state { idle, colliding, pickedUp };
	public state grabbingState = state.idle;
	private Transform grabbedObjParent;


	private float dist;




	public override bool TriggerClick(ClickedEventArgs e)
	{
		if (collidedObject == null || collidedObject.tag == "UIObject")
		{
			return false;
		}
			
		if (grabbingState ==  state.colliding)
		{
			if (collidedObject.GetComponent<SnapBack>())
			{
				if (!collidedObject.GetComponent<SnapBack>().objectCopy)
				{
					Debug.Log("creating copy obj");

					//CmdCreateCollidedObjCopy(collidedObject.GetComponent<NetworkIdentity>());
				}
			}
			pickUp(collidedObject);
			StartCoroutine("snapCoroutine");
		}

		return true;
	}

	public override bool TriggerUnclick (ClickedEventArgs e)
	{
		if (grabbingState == state.pickedUp)
		{
			releaseObj(collidedObject);
			Debug.Log("distance is" + dist);
			if (dist <= 0.25 && collidedObject.GetComponent<SnapBack>())
			{
				Debug.Log("calling destory command");
				//CmdDestroyCollidedObjCopy(collidedObject.GetComponent<NetworkIdentity>());
			}
		}
		return true;
	}

	public override bool MenuClick (ClickedEventArgs e){ return false;}
	public override bool MenuUnclick (ClickedEventArgs e){ return false;}
	public override bool PadClick (ClickedEventArgs e){ return false;}
	public override bool PadUnclick (ClickedEventArgs e){ return false;}
	public override bool Grip (ClickedEventArgs e){ return false;}
	public override  bool UnGrip(ClickedEventArgs e){ return false;}
	public override bool PadTouched(ClickedEventArgs e){ return false;}
	public override bool PadUntouched(ClickedEventArgs e){ return false;}
	public override bool SteamClicked (ClickedEventArgs e){ return false;}

	//Initialize any variables in these, they get called whenever you start/stop using the given tool
	public override void stopUsing (){}
	public override void startUsing(){}

	public override void CollisionEnter (Collider other){

		Debug.Log ("I entered " + other.gameObject.name);
		if(grabbingState == state.idle || grabbingState == state.colliding)
		{

			collidedObject =other.gameObject;
			grabbingState = state.colliding;
		}
	}
	public override void CollisionExit (Collider other){

		if (grabbingState != state.pickedUp)
		{

			grabbingState = state.idle;
		}


	}


	private  void resetClicked(object sender, ClickedEventArgs e)
	{
		//CmdResetModel();
	}



	IEnumerator snapCoroutine()
	{
		Debug.Log("COOOR");
		while ((grabbingState == state.pickedUp))
		{
			if (collidedObject.GetComponent<SnapBack>())
			{
				if (collidedObject.GetComponent<SnapBack>().objectCopy)
				{
					dist = Vector3.Distance(collidedObject.transform.position, collidedObject.GetComponent<SnapBack>().objectCopy.transform.position);
					if (dist <= 0.25)
					{
						collidedObject.GetComponent<SnapBack>().objectCopy.GetComponent<MeshRenderer>().enabled = true;
					}
					else if (dist > 0.25)
					{
						collidedObject.GetComponent<SnapBack>().objectCopy.GetComponent<MeshRenderer>().enabled = false;
					}
				}
			}
			yield return null;
		}

		yield return null;

		print("MyCoroutine is now finished.");
	}



	private void pickUp(GameObject obj)
	{



		grabbedObjParent = obj.transform.parent;
		obj.transform.SetParent(controllerInit.transform);
		grabbingState = state.pickedUp;
	}

	private void releaseObj(GameObject Obj)
	{
		// Debug.Log("release");
		Obj.transform.parent = grabbedObjParent;
		grabbingState = state.colliding;
	}


	public void controllerTrigExit(object sender, Collider collider)
	{
		// Debug.Log("Colllllllllision enter" + collider.gameObject.name);
		if (grabbingState != state.pickedUp)
		{
			if (collider.gameObject.GetComponent<LocalPNetworkTransform>())
			{
				//CmdDisableNetTransform(collider.gameObject.GetComponent<NetworkIdentity>());
			}
			grabbingState = state.idle;
		}
	}







}
