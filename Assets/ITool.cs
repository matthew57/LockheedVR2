using UnityEngine;
using System.Collections;

public abstract class  ITool : MonoBehaviour {




	public enum controller { Left, Right, Both };
	public controller assignedController;

	protected ImprovedController controllerInit;

	public void Start()
	{
		findController();
	}



	public abstract void TriggerClick(ClickedEventArgs e);
	public abstract void TriggerUnclick (ClickedEventArgs e);
	public abstract void MenuClick (ClickedEventArgs e);
	public abstract void MenuUnclick (ClickedEventArgs e);
	public abstract void PadClick (ClickedEventArgs e);
	public abstract void PadUnclick (ClickedEventArgs e);
	public abstract void Grip (ClickedEventArgs e);
	public abstract void UnGrip(ClickedEventArgs e);
	public abstract void PadTouched(ClickedEventArgs e);
	public abstract void PadUntouched(ClickedEventArgs e);
	public abstract void SteamClicked (ClickedEventArgs e);
	public abstract void CollisionEnter (Collider other);
	public abstract void CollisionExit (Collider other);


	public abstract void stopUsing ();
	public abstract void startUsing ();

	/*

//Copy and paste these into any new classes extending ITool. leave them blank if that tool doesn't use that button press

	public override void TriggerClick(ClickedEventArgs e){}
	public override void TriggerUnclick (ClickedEventArgs e){}
	public override void MenuClick (ClickedEventArgs e){}
	public override void MenuUnclick (ClickedEventArgs e){}
	public override void PadClick (ClickedEventArgs e){}
	public override void PadUnclick (ClickedEventArgs e){}
	public override void Grip (ClickedEventArgs e){}
	public override void UnGrip(ClickedEventArgs e){}
	public override void PadTouched(ClickedEventArgs e){}
	public override void PadUntouched(ClickedEventArgs e){}
	public override void SteamClicked (ClickedEventArgs e){}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}

	*/

	public void sub()
	{
		controllerInit.setCurrentTool (this);
	}

	void findController()
	{
		switch (assignedController)
		{
		case (controller.Left):
			controllerInit = GameObject.Find("Controller (left)").GetComponent<ImprovedController>();
			break;
		case (controller.Right):
			controllerInit = GameObject.Find("Controller (right)").GetComponent<ImprovedController>();
			break;
		default:
			break;
		}
	}










}
