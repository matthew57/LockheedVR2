using UnityEngine;
using System.Collections;

public class newLaserPointer : ITool {



	bool laserOn = false;


	// Use this for initialization
	void Start () {
		AuxiliaryTool = true;
		sub ();
	}
	



	public override bool TriggerClick(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal){return false;}

	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal) {

		return false;
	}


	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadClick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal){return false;}


	public override bool Grip (ClickedEventArgs e, bool TimeNormal){	
		laserOn = !laserOn;
		GetComponentInChildren<VR_LaserPointer>().holder.gameObject.SetActive(laserOn);
		return true;}


	public override bool UnGrip(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadTouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUntouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool SteamClicked (ClickedEventArgs e, bool TimeNormal){return false;}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}



}
