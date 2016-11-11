using UnityEngine;
using System.Collections;

public class newLaserPointer : ITool {



	bool laserOn = false;


	// Use this for initialization
	void Start () {
		AuxiliaryTool = true;
		sub ();
	}
	



	public override bool TriggerClick(ClickedEventArgs e){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e){return false;}

	public override bool MenuClick (ClickedEventArgs e) {

	
		laserOn = !laserOn;

		GetComponentInChildren<VR_LaserPointer>().holder.gameObject.SetActive(laserOn);
		return true;
	}


	public override bool MenuUnclick (ClickedEventArgs e){return false;}
	public override bool PadClick (ClickedEventArgs e){return false;}
	public override bool PadUnclick (ClickedEventArgs e){return false;}
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
