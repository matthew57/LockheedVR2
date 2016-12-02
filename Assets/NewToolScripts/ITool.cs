using UnityEngine;
using System.Collections;

public abstract class  ITool : MonoBehaviour {


	public ToolDisplayer.ToolDisplayInfo myInfo;
	public bool playBackDevice;

	public enum controller { Left, Right, Both };
	[Tooltip("Select This if this tool can be used at the same time as other tools (menu, laser pointer, arc teleporter...)")]
	public bool AuxiliaryTool;
	public controller assignedController;
	protected bool subscribed = false;

	protected ImprovedController controllerInit;

	public void Awake()
	{
		findController();
	}


	// RETURN TRUE IF YOU HAVE CODE THAT IS USED IN THESE METHODS. FALSE WILL ALLOW THE COMMAND TO GO TO OTHER SECONDARY TOOLS

	public abstract bool TriggerClick(ClickedEventArgs e, bool TimeNormal);
	public abstract bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal);
	public abstract bool MenuClick (ClickedEventArgs e, bool TimeNormal);
	public abstract bool MenuUnclick (ClickedEventArgs e, bool TimeNormal);
	public abstract bool PadClick (ClickedEventArgs e, bool TimeNormal);
	public abstract bool PadUnclick (ClickedEventArgs e, bool TimeNormal);
	public abstract bool Grip (ClickedEventArgs e, bool TimeNormal);
	public abstract bool UnGrip(ClickedEventArgs e, bool TimeNormal);
	public abstract bool PadTouched(ClickedEventArgs e, bool TimeNormal);
	public abstract bool PadUntouched(ClickedEventArgs e, bool TimeNormal);
	public abstract bool SteamClicked (ClickedEventArgs e, bool TimeNormal);
	public abstract void CollisionEnter (Collider other);
	public abstract void CollisionExit (Collider other);


	public abstract void stopUsing ();
	public abstract void startUsing ();

	/*

//Copy and paste these into any new classes extending ITool. leave them blank if that tool doesn't use that button press

	public override bool TriggerClick(ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool PadClick (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool Grip (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool UnGrip(ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool PadTouched(ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool PadUntouched(ClickedEventArgs e, bool TimeNormal)){return false;}
	public override bool SteamClicked (ClickedEventArgs e, bool TimeNormal)){return false;}
	public override void CollisionEnter (Collider other){}
	public override void CollisionExit (Collider other){}
	public override void stopUsing (){}
	public override void startUsing(){}

	*/

	public void sub()
	{


		if (!AuxiliaryTool) {
			controllerInit.setMainTool (this);

		} else {
			controllerInit.subScribeTool (this);
		}
		subscribed = true;
	}

	public void unsub()
	{

		subscribed = false;
	}

	void findController()
	{
		switch (assignedController)
		{
		case (controller.Left):
			if (!playBackDevice) {
				controllerInit = GameObject.Find ("Controller (left)").GetComponent<ImprovedController> ();
			} else {
				controllerInit = GameObject.Find ("Controller (left)B").GetComponent<PlayBackInputController> ();
			}
			break;
		case (controller.Right):
			if (!playBackDevice) {
				controllerInit = GameObject.Find ("Controller (right)").GetComponent<ImprovedController> ();
			}
			else {
				controllerInit = GameObject.Find ("Controller (right)B").GetComponent<PlayBackInputController> ();
			}
			break;
		case (controller.Both):

			break;
		default:
			break;
		}
	}










}
