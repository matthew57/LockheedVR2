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

	public abstract bool TriggerClick(ClickedEventArgs e);
	public abstract bool TriggerUnclick (ClickedEventArgs e);
	public abstract bool MenuClick (ClickedEventArgs e);
	public abstract bool MenuUnclick (ClickedEventArgs e);
	public abstract bool PadClick (ClickedEventArgs e);
	public abstract bool PadUnclick (ClickedEventArgs e);
	public abstract bool Grip (ClickedEventArgs e);
	public abstract bool UnGrip(ClickedEventArgs e);
	public abstract bool PadTouched(ClickedEventArgs e);
	public abstract bool PadUntouched(ClickedEventArgs e);
	public abstract bool SteamClicked (ClickedEventArgs e);
	public abstract void CollisionEnter (Collider other);
	public abstract void CollisionExit (Collider other);


	public abstract void stopUsing ();
	public abstract void startUsing ();

	/*

//Copy and paste these into any new classes extending ITool. leave them blank if that tool doesn't use that button press

	public override bool TriggerClick(ClickedEventArgs e){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e){return false;}
	public override bool MenuClick (ClickedEventArgs e){return false;}
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
