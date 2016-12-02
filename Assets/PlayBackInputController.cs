using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Valve.VR;
using UnityEngine.UI;


// this class mimics the input of the improved controller.
//instead of getting input from the controllers, it gets input from the playback system.

public class PlayBackInputController : ImprovedController {

	void Start()
	{
	}

	public override void Update()
	{
		
	}

	public void callButtonAction(ClickedEventArgs e, bool TimeNormal)
	{
		if (e.controllerIndex != controllerIndex) {
			return;
		}

	//	Debug.Log ("same controller " + e.controllerIndex + "   " + controllerIndex);

		this.transform.position = e.location;
		this.transform.rotation = e.rotation;

		if (e.butClicked == Tools.button.Trigger) {
			if (e.offOn) {
				triggerPressed = true && TimeNormal;
				OnTriggerClicked (e, TimeNormal);
			} else {
				triggerPressed = false || !TimeNormal;
				OnTriggerUnclicked (e, TimeNormal);
			}
		}

			
		if (e.butClicked == Tools.button.Grip) {
			if (e.offOn) {
				gripped = true && TimeNormal;
				OnGripped (e, TimeNormal);
			} else {
				gripped = false || !TimeNormal;
				OnUngripped (e, TimeNormal);
			}
		}

		if (e.butClicked == Tools.button.Pad) {
			if (e.offOn) {
				padPressed = true && TimeNormal;
				OnPadClicked (e, TimeNormal);
			} else {
				padPressed = false || !TimeNormal;
				OnPadUnclicked (e, TimeNormal);
			}
		}


		if (e.butClicked == Tools.button.Menu){
			if (e.offOn) {
				menuPressed = true && TimeNormal;
				OnMenuClicked (e, TimeNormal);
			} else {
				menuPressed = false|| !TimeNormal;
				OnMenuUnclicked(e, TimeNormal);
			}
		}
			
			
		if (e.butClicked == Tools.button.PadTouch ) {
			if (e.offOn) {
				padTouched = true && TimeNormal;
				OnPadTouched (e, TimeNormal);
			} else {
				padTouched = false || !TimeNormal;
				OnPadUntouched (e, TimeNormal);
			}
		}
	
	}





}
