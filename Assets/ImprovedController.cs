using UnityEngine;
using System.Collections;
using System;
using Valve.VR;




public class ImprovedController : MonoBehaviour {


	public ITool currentTool;


	public void setCurrentTool(ITool nextTool)
	{
		if (currentTool) {
			currentTool.stopUsing ();
		}

		currentTool = nextTool;
		currentTool.startUsing ();
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
			controllerIndex = (uint) this.GetComponent<SteamVR_TrackedObject>().index;
		}
	}


	private void OnTriggerEnter(Collider collider)
	{
		if (currentTool) {
			currentTool.CollisionEnter (collider);}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (currentTool) {
			currentTool.CollisionExit (collider);
		}
	}






	public void SetDeviceIndex(int index)
	{
		this.controllerIndex = (uint) index;
	}

	public virtual void OnTriggerClicked(ClickedEventArgs e)
	{
		if(currentTool)
			{currentTool.TriggerClick(e);}

		if (TriggerClicked != null)
			TriggerClicked(this, e);

	}

	public virtual void OnTriggerUnclicked(ClickedEventArgs e)
	{if(currentTool)
		{currentTool.TriggerUnclick(e);}
	if (TriggerUnclicked != null)
			TriggerUnclicked(this, e);
	}

	public virtual void OnMenuClicked(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.MenuClick(e);}
		if (MenuButtonClicked != null)
			MenuButtonClicked(this, e);
	}

	public virtual void OnMenuUnclicked(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.MenuUnclick(e);}
		if (MenuButtonUnclicked != null)
			MenuButtonUnclicked(this, e);
	}

	public virtual void OnSteamClicked(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.SteamClicked(e);}
		if (SteamClicked != null)
			SteamClicked(this, e);
	}

	public virtual void OnPadClicked(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.PadClick(e);}
		if (PadClicked != null)
			PadClicked(this, e);
	}

	public virtual void OnPadUnclicked(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.PadUnclick(e);}
		if (PadUnclicked != null)
			PadUnclicked(this, e);
	}

	public virtual void OnPadTouched(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.PadTouched(e);}
		if (PadTouched != null)
			PadTouched(this, e);
	}

	public virtual void OnPadUntouched(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.PadUntouched(e);}
		if (PadUntouched != null)
			PadUntouched(this, e);
	}

	public virtual void OnGripped(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.Grip(e);}
		if (Gripped != null)
			Gripped(this, e);
	}

	public virtual void OnUngripped(ClickedEventArgs e)
	{
		if(currentTool)
		{currentTool.UnGrip(e);}
		if (Ungripped != null)
			Ungripped(this, e);
	}

	// Update is called once per frame
	void Update()
	{

		// All of this stuff detected button press detection
		var system = OpenVR.System;
		if (system != null && system.GetControllerState(controllerIndex, ref controllerState))
		{
			ulong trigger = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));

			ClickedEventArgs e;
			e.controllerIndex = controllerIndex;
			e.flags = (uint)controllerState.ulButtonPressed;
			e.padX = controllerState.rAxis0.x;
			e.padY = controllerState.rAxis0.y;


			if (trigger > 0L && !triggerPressed)
			{
				triggerPressed = true;
				e.butClicked = Tools.button.Trigger;// ******
				OnTriggerClicked(e);

			}
			else if (trigger == 0L && triggerPressed)
			{
				triggerPressed = false;
				e.butClicked = Tools.button.Trigger;// ******
				OnTriggerUnclicked(e);
			}

			ulong grip = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_Grip));
			if (grip > 0L && !gripped)
			{
				gripped = true;
				e.butClicked = Tools.button.Grip;// ******
				OnGripped(e);

			}
			else if (grip == 0L && gripped)
			{
				gripped = false;
				e.butClicked = Tools.button.Grip;// ******
				OnUngripped(e);
			}

			ulong pad = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
			if (pad > 0L && !padPressed)
			{
				padPressed = true;
				e.butClicked = Tools.button.Pad; // ******
				OnPadClicked(e);
			}
			else if (pad == 0L && padPressed)
			{
				padPressed = false;
				e.butClicked = Tools.button.Pad;// ******
				OnPadUnclicked(e);
			}

			ulong menu = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
			if (menu > 0L && !menuPressed)
			{
				menuPressed = true;
				e.butClicked = Tools.button.Menu; // ******
				OnMenuClicked(e);
			}
			else if (menu == 0L && menuPressed)
			{
				menuPressed = false;
				e.butClicked = Tools.button.Menu;// ******
				OnMenuUnclicked(e);
			}

			pad = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
			if (pad > 0L && !padTouched)
			{
				padTouched = true;
				e.butClicked = Tools.button.Pad; // ******
				OnPadTouched(e);

			}
			else if (pad == 0L && padTouched)
			{
				padTouched = false;
				e.butClicked = Tools.button.Pad;// ******
				OnPadUntouched(e);
			}
		}
	}
}
