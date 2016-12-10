using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Valve.VR;
using UnityEngine.UI;




public class ImprovedController : MonoBehaviour {

	// THE TOOL IN THE FIRST INDEX IS THE MAIN SELECTABLE TOOL THAT CAN BE SWAPPED OUT.
	public List<ITool> myTools;

	public handanimations myHand;

	public void setMainTool(ITool nextTool)
	{
		//Debug.Log ("Settign main tool to " + nextTool + "  " + this.gameObject);

		if (myTools.Count == 0) {
			myTools.Add (nextTool);
		} else {

			if (myTools [0]) {
				myTools [0].unsub ();
				myTools [0].stopUsing ();
			}

			myTools [0] = nextTool;
			myTools [0].startUsing ();
		}
		GameObject.FindObjectOfType<ToolDisplayer> ().loadDescription (myTools [0].myInfo);


	}


	RecordingSystem MyRecorder;



	public void subScribeTool(ITool theTool)
	{

		if (myTools.Count == 0) {
			myTools.Add (null);
		}
		myTools.Add (theTool);
		
	}

	public event ClickedEventHandler MenuButtonClicked;
	public event ClickedEventHandler MenuButtonUnclicked;
	public event ClickedEventHandler TriggerClicked;
	public event ClickedEventHandler TriggerUnclicked;
	public event ClickedEventHandler SteamClicked;
	public event ClickedEventHandler PadClicked;
	public event ClickedEventHandler PadUnclicked;
	public event ClickedEventHandler PadTouched;
	public event ClickedEventHandler PadUntouched;
	public event ClickedEventHandler Gripped;
	public event ClickedEventHandler Ungripped;



 public uint controllerIndex;
	public VRControllerState_t controllerState;
	public bool triggerPressed = false;
	public bool steamPressed = false;
	public bool menuPressed = false;
	public bool padPressed = false;
	public bool padTouched = false;
	public bool gripped = false;


	// Use this for initialization
	void Start()
	{
		
		if (this.GetComponent<SteamVR_TrackedObject>() == null)
		{
			gameObject.AddComponent<SteamVR_TrackedObject>();
		}

		if (controllerIndex != 0)
		{
			this.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			if (this.GetComponent<SteamVR_RenderModel>() != null)
			{
				this.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			}
		}
		else
		{
		//	controllerIndex = (uint) this.GetComponent<SteamVR_TrackedObject>().index;
		}
	}


	public void setRecorder(RecordingSystem RS)
	{
		MyRecorder = RS;
	}

	private void OnTriggerEnter(Collider collider)
	{	if (collider.gameObject.name == "GrabSphere") {
			return;
		}

		foreach (ITool IT in myTools) {
			if (IT) {
			//	Debug.Log ("Entering "+ IT);
				IT.CollisionEnter (collider);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{	foreach (ITool IT in myTools) {
			if (IT) {
				IT.CollisionExit (collider);
			}
		}
	}






	public void SetDeviceIndex(int index)
	{
		this.controllerIndex = (uint) index;
	}

	public virtual void OnTriggerClicked(ClickedEventArgs e, bool TimeNormal)
	{

		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.TriggerClick (e,TimeNormal)) {
					break;
				}
			}
		}
	

		if (TriggerClicked != null)
			TriggerClicked(this, e);

	}

	public virtual void OnTriggerUnclicked(ClickedEventArgs e, bool TimeNormal)
	{
	foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.TriggerUnclick(e,TimeNormal)) {
					break;
				}
			}
		}
	if (TriggerUnclicked != null)
			TriggerUnclicked(this, e);
	}

	public virtual void OnMenuClicked(ClickedEventArgs e, bool TimeNormal)
	{

		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.MenuClick(e,TimeNormal)) {
					break;
				}
			}
		}
		if (MenuButtonClicked != null)
			MenuButtonClicked(this, e);
	}

	public virtual void OnMenuUnclicked(ClickedEventArgs e, bool TimeNormal)
	{
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.MenuUnclick(e,TimeNormal)) {
					break;
				}
			}
		}
		if (MenuButtonUnclicked != null)
			MenuButtonUnclicked(this, e);
	}

	public virtual void OnSteamClicked(ClickedEventArgs e, bool TimeNormal)
	{
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.SteamClicked(e,TimeNormal)) {
					break;
				}
			}
		}
		if (SteamClicked != null)
			SteamClicked(this, e);
	}

	public virtual void OnPadClicked(ClickedEventArgs e, bool TimeNormal)
	{//Debug.Log ("checking " + this.gameObject);
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.PadClick(e,TimeNormal)) {
					return;
				}
			}
		}
		if (PadClicked != null)
			PadClicked(this, e);
	}

	public virtual void OnPadUnclicked(ClickedEventArgs e, bool TimeNormal)
	{
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.PadUnclick(e,TimeNormal)) {
					return;
				}
			}
		}
		if (PadUnclicked != null)
			PadUnclicked(this, e);
	}

	public virtual void OnPadTouched(ClickedEventArgs e, bool TimeNormal)
	{
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.PadTouched(e,TimeNormal)) {
					break;
				}
			}
		}
		if (PadTouched != null)
			PadTouched(this, e);
	}

	public virtual void OnPadUntouched(ClickedEventArgs e, bool TimeNormal)
	{
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.PadUntouched(e,TimeNormal)) {
					break;
				}
			}
		}
		if (PadUntouched != null)
			PadUntouched(this, e);
	}

	public virtual void OnGripped(ClickedEventArgs e, bool TimeNormal)
	{
		foreach (ITool IT in myTools) {
			if (IT) {
				if (IT.Grip(e,TimeNormal)) {
					break;
				}
			}
		}
		if (Gripped != null)
			Gripped(this, e);
	}

	public virtual void OnUngripped(ClickedEventArgs e, bool TimeNormal)
	{foreach (ITool IT in myTools) {
		if (IT) {
				if (IT.UnGrip(e,TimeNormal)) {
				break;
			}
		}
	}
		if (Ungripped != null)
			Ungripped(this, e);
	}

	// Update is called once per frame
	public virtual void Update()
	{

		// All of this stuff detected button press detection
		var system = OpenVR.System;
		if (system != null && system.GetControllerState(controllerIndex, ref controllerState))
		{
			ulong trigger = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));

			bool buttonPressed = false;
			ClickedEventArgs e = new ClickedEventArgs ();
			e.controllerIndex = controllerIndex;
			e.flags = (uint)controllerState.ulButtonPressed;
			e.padX = controllerState.rAxis0.x;
			e.padY = controllerState.rAxis0.y;
			e.location = this.transform.position;
			e.rotation = this.transform.rotation;


			if (trigger > 0L && !triggerPressed)
			{
				triggerPressed = true;
				e.butClicked = Tools.button.Trigger;// ******
				OnTriggerClicked(e, true);
				e.offOn = true;
				buttonPressed = true;
				if (myHand) {
					myHand.SetHandAnim (handanimations.HandPosition.GrabSmall);
				}
			}
			else if (trigger == 0L && triggerPressed)
			{
				triggerPressed = false;
				e.butClicked = Tools.button.Trigger;// ******
				OnTriggerUnclicked(e, true);
				e.offOn = false;
				buttonPressed = true;
				if (myHand) {
					myHand.SetHandAnim (handanimations.HandPosition.Idle);
				}
			}

			ulong grip = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_Grip));
			if (grip > 0L && !gripped)
			{
				gripped = true;
				e.butClicked = Tools.button.Grip;// ******
				OnGripped(e, true);
				e.offOn = true;
				buttonPressed = true;

			}
			else if (grip == 0L && gripped)
			{
				gripped = false;
				e.butClicked = Tools.button.Grip;// ******
				OnUngripped(e, true);
				e.offOn = false;
				buttonPressed = true;
			}

			ulong  pad = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
			if (pad > 0L && !padTouched)
			{
				padTouched = true;
				e.butClicked = Tools.button.PadTouch; // ******
				OnPadTouched(e, true);
				e.offOn = true;
				buttonPressed = true;

			}
			else if (pad == 0L && padTouched)
			{
				padTouched = false;
				e.butClicked = Tools.button.PadTouch;// ******
				OnPadUntouched(e, true);
				e.offOn = false;
				buttonPressed = true;
			}




			pad = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
			if (pad > 0L && !padPressed)
			{
				padPressed = true;
				e.butClicked = Tools.button.Pad; // ******
				OnPadClicked(e, true);
				e.offOn = true;
				buttonPressed = true;
			}
			else if (pad == 0L && padPressed)
			{
				padPressed = false;
				e.butClicked = Tools.button.Pad;// ******
				OnPadUnclicked(e, true);
				e.offOn = false;
				buttonPressed = true;
			}

			ulong menu = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
			if (menu > 0L && !menuPressed)
			{
				menuPressed = true;
				e.butClicked = Tools.button.Menu; // ******
				OnMenuClicked(e, true);
				e.offOn = true;
				buttonPressed = true;
			}
			else if (menu == 0L && menuPressed)
			{
				menuPressed = false;
				e.butClicked = Tools.button.Menu;// ******
				OnMenuUnclicked(e, true);
				e.offOn = false;
				buttonPressed = true;
			}

		
			if (MyRecorder && buttonPressed) {
				MyRecorder.recordClick (e, true);
			}

		}
	}
}
