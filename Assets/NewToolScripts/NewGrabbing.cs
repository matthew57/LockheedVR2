﻿using System;
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


	public GameObject collidedObject;
	public enum state { idle, colliding, pickedUp };
	public state grabbingState = state.idle;
	private Transform grabbedObjParent;

	private GameObject grabSphere;


	private float dist;
	bool ObjectUsedGrav = false; 


	Vector3 lastObjectLocation;
	Quaternion lastRotation;

	NewGrabbing otherGrabber;

	//Figure out a better way to record which objects have been touched.
	RecordingSystem myRecorder;


	List<deletedObj> deletedList = new List<deletedObj>();
	public List<GameObject> collidedList = new List<GameObject>();


	struct deletedObj{

		public GameObject obj;
		public Vector3 lastPos;
		public Quaternion lastRot;
		public bool useKinem;
		public bool useGrav;

	}



	void Start()
		{

	
		foreach (NewGrabbing ng in GetComponents<NewGrabbing>()) {
			if (ng != this) {
				otherGrabber = ng;
				break;
			}
		}
		myRecorder = GetComponent<RecordingSystem> ();
		grabSphere = controllerInit.gameObject.transform.Find("GrabSphere").gameObject;


		}


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
			if (!!otherGrabber.currentlyGrabbing (collidedObject)) {
				otherGrabber.releaseObj ();
			
			}

			//Debug.Log ("Picking up thing " + collidedObject);
				pickUp (collidedObject);
				StartCoroutine ("snapCoroutine");

		}

		return true;
	}

	public override bool TriggerUnclick (ClickedEventArgs e)
	{
		if (grabbingState == state.pickedUp)
		{
			releaseObj();

		}
		return true;
	}

	public override bool MenuClick (ClickedEventArgs e){ 

		if (assignedController == controller.Right) {
			undoDelete ();
		}


		return false;}
	
	public override bool MenuUnclick (ClickedEventArgs e){ return false;}

	// For Deleting and duplicating objects, if grabbing and object, it will leave a duplicate behind, if none are grabbed, it will create a new copy and grab it.
	public override bool PadClick (ClickedEventArgs e){ 

		if (assignedController == controller.Right) {
			if (e.padY > 0) {
				if (collidedObject && grabbingState == state.pickedUp) {
					releaseObj ();
					deleteObject (collidedObject);
					checkIfInObject ();

				} else if (collidedObject && grabbingState == state.idle) {
					ErrorPrompt.instance.showError ("Pick up an object to delete it");
				}
			} else {
				if (grabbingState == state.pickedUp) {

					Instantiate (collidedObject, collidedObject.transform.position, collidedObject.transform.rotation);
				} else if (grabbingState == state.colliding) {
		
					GameObject obj =  (GameObject)Instantiate (collidedObject, collidedObject.transform.position, collidedObject.transform.rotation);
					collidedObject = obj;
					pickUp (obj);
					StartCoroutine ("snapCoroutine");
				}
			
			}

			return true;
		} else {
			return false;
		}
	}


	//THis doesn't actually delete the object, it just moves it way far away, that way we an still bring it back if need be
	public void deleteObject(GameObject toDelete)
	{
		deletedObj currDelete = new deletedObj ();
		currDelete.obj = toDelete;
		currDelete.lastPos = toDelete.transform.position;
		currDelete.lastRot = toDelete.transform.rotation;

		Rigidbody rb = toDelete.GetComponent<Rigidbody> ();
		if (rb) {
			currDelete.useGrav = rb.useGravity;
			currDelete.useKinem = rb.isKinematic;

			rb.useGravity = false;
			rb.isKinematic = false;
		}

		deletedList.Add (currDelete);
		toDelete.transform.position = new Vector3 (-1000, -1000, -1000);

	
		if (collidedList.Contains (collidedObject)) {
			collidedList.Remove (collidedObject);
		}
	}

	public void undoDelete()
	{
		if (deletedList.Count > 0) {
			deletedList [deletedList.Count - 1].obj.transform.position = deletedList [deletedList.Count - 1].lastPos;
			deletedList [deletedList.Count - 1].obj.transform.rotation = deletedList [deletedList.Count - 1].lastRot;
			Rigidbody rb = deletedList [deletedList.Count - 1].obj.GetComponent<Rigidbody> ();
			if (rb) {
				rb.useGravity = deletedList [deletedList.Count - 1].useGrav;
				rb.isKinematic = deletedList [deletedList.Count - 1].useKinem;
				rb.velocity = Vector3.zero;

			}


			deletedList.RemoveAt (deletedList.Count - 1);

		}
	
	
	}



	public override bool PadUnclick (ClickedEventArgs e){ 
		if (assignedController == controller.Right) {
			if (grabbingState == state.pickedUp)
			{
				releaseObj();
			}
		}
			return false;}


	public override bool Grip (ClickedEventArgs e){ 


		if (collidedObject == null || collidedObject.tag == "UIObject")
		{
			return false;
		}

		if (grabbingState == state.colliding) {

			if (otherGrabber.currentlyGrabbing (collidedObject)) {
				otherGrabber.releaseObj ();
			}
				pickUp (collidedObject);
				StartCoroutine ("snapCoroutine");

				return true;

		} else if (grabbingState == state.pickedUp) {
			releaseObj();
			return true;
		
		}

	

		return false;}


	public override  bool UnGrip(ClickedEventArgs e){ return false;}
	public override bool PadTouched(ClickedEventArgs e){ return false;}
	public override bool PadUntouched(ClickedEventArgs e){ return false;}
	public override bool SteamClicked (ClickedEventArgs e){ return false;}

	//Initialize any variables in these, they get called whenever you start/stop using the given tool
	public override void stopUsing (){}
	public override void startUsing(){}


	public bool currentlyGrabbing(GameObject ob)
	{if (grabbingState == state.pickedUp && collidedObject == ob) {
			return true;
		} else {
			return false;
		}
	}

	public override void CollisionEnter (Collider other){

		if(!collidedList.Contains(other.gameObject))
			collidedList.Add (other.gameObject);
	
		if(grabbingState == state.idle || grabbingState == state.colliding)
		{
			
			collidedObject =other.gameObject;
			grabbingState = state.colliding;

			//Debug.Log ("Now inside " + collidedObject + "   " + this.gameObject);
		}
	}
	public override void CollisionExit (Collider other){

		if (collidedList.Contains (other.gameObject)) {

			collidedList.Remove (other.gameObject);
		}


		if (grabbingState != state.pickedUp)
		{
			checkIfInObject ();
		}
	
	}


	private  void resetClicked(object sender, ClickedEventArgs e)
	{
		//CmdResetModel();
	}



	IEnumerator snapCoroutine()
	{

		while ((grabbingState == state.pickedUp))
		{ lastObjectLocation = collidedObject.transform.position;
			lastRotation = collidedObject.transform.rotation;


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

	//	print("MyCoroutine is now finished.");
	}



	private void pickUp(GameObject obj)
	{

		Rigidbody rb = obj.GetComponent<Rigidbody> ();
		ObjectUsedGrav = rb.useGravity;
		rb.useGravity = false;
		rb.isKinematic = true;
		grabbedObjParent = obj.transform.parent;

		obj.transform.SetParent(controllerInit.transform);
		GrabTool gTool = obj.GetComponent<GrabTool> ();
		if (gTool) {

			obj.transform.rotation = grabSphere.transform.rotation;

			obj.transform.Rotate (new Vector3 (gTool.handleAngle, 0, 0));

			obj.transform.position = (grabSphere.transform.position - gTool.transform.rotation * gTool.getGrabPoint());


		}

		if(myRecorder){
			myRecorder.recordInteractedObj (obj);
		}

		grabbingState = state.pickedUp;
	}

	public void releaseObj()
	{
		// Debug.Log("release");
		collidedObject.transform.parent = grabbedObjParent;
		grabbingState = state.colliding;

		if (ObjectUsedGrav) {
			collidedObject.GetComponent<Rigidbody> ().useGravity = true;
			collidedObject.GetComponent<Rigidbody> ().isKinematic = false;

			StartCoroutine (throwObject (collidedObject, (collidedObject.transform.position - lastObjectLocation) / Time.deltaTime));
			//collidedObject.GetComponent<Rigidbody> ().AddForce ((collidedObject.transform.position - lastObjectLocation)/Time.deltaTime);
		
		
		}
	}

	IEnumerator throwObject(GameObject obj, Vector3 direction)
	{yield return 0;
		obj.GetComponent<Rigidbody> ().AddForce (70* (obj.transform.position - lastObjectLocation)/Time.deltaTime);


	
		Quaternion quat10;

		quat10=obj.transform.rotation*Quaternion.Inverse(lastRotation);
		quat10.x /= (Time.deltaTime *75 );
		quat10.y /= (Time.deltaTime * 75);
		quat10.z /= (Time.deltaTime *75);
	
		obj.GetComponent<Rigidbody>().AddTorque(quat10.x,quat10.y,quat10.z,ForceMode.Impulse);

	
	}


	public void controllerTrigExit(object sender, Collider collider)
	{

		if (grabbingState != state.pickedUp)
		{
			if (collider.gameObject.GetComponent<LocalPNetworkTransform>())
			{
				//CmdDisableNetTransform(collider.gameObject.GetComponent<NetworkIdentity>());
			}
			grabbingState = state.idle;
		}
	}

	void checkIfInObject()
	{	if (collidedList.Count > 0) {
			collidedObject = collidedList [collidedList.Count - 1];
	} else {
		collidedObject = null;
		grabbingState = state.idle;
	}
		
	}





}
