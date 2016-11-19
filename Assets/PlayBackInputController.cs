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

	public void callButtonAction(ClickedEventArgs e)
	{
		if (e.controllerIndex != controllerIndex) {

		
			return;
		}

	//	Debug.Log ("same controller " + e.controllerIndex + "   " + controllerIndex);

		this.transform.position = e.location;
		this.transform.rotation = e.rotation;

		if (e.butClicked == Tools.button.Trigger && e.offOn)
			{
				triggerPressed = true;
				OnTriggerClicked(e);

			}
		else if (e.butClicked == Tools.button.Trigger && !e.offOn)
			{
				triggerPressed = false;
				OnTriggerUnclicked(e);
			}

			
		if (e.butClicked == Tools.button.Grip && e.offOn)
			{
				gripped = true;
				OnGripped(e);

			}
		else if (e.butClicked == Tools.button.Grip && !e.offOn)
			{
				gripped = false;
				OnUngripped(e);
			}


		if (e.butClicked == Tools.button.Pad && e.offOn)
			{
				padPressed = true;
				OnPadClicked(e);
			}
		else if (e.butClicked == Tools.button.Pad && !e.offOn)
			{
				padPressed = false;
				OnPadUnclicked(e);
			}


			if (e.butClicked == Tools.button.Menu && e.offOn)
			{
				menuPressed = true;
				OnMenuClicked(e);
			}
			else if (e.butClicked == Tools.button.Menu && !e.offOn)
			{
				menuPressed = false;
				OnMenuUnclicked(e);
			}

			
		if (e.butClicked == Tools.button.PadTouch && e.offOn)
			{
				padTouched = true;
				OnPadTouched(e);

			}
		else if (e.butClicked == Tools.button.PadTouch && !e.offOn)
			{
				padTouched = false;
				OnPadUntouched(e);
			}



	}
}
