using UnityEngine;
using System.Collections.Generic;

using Valve.VR;
using UnityEngine.Networking;


public struct ButtonSubHandler
{
    public ClickedEventHandler clicked;
    public ClickedEventHandler unclicked;
}

public class Tools : NetworkBehaviour
{
    public enum button { Trigger, Menu, Pad, Grip };
    
    public Dictionary<button, ButtonSubHandler> buttonMap = new Dictionary<button, ButtonSubHandler>();
    //list enum

    public enum controller { Left, Right };
    public controller assignedController;

	protected ImprovedController controllerInit;
	//protected SteamVR_TrackedController controllerInit;

    public override void OnStartLocalPlayer()
    {
        findController();
    }

    private void asnTrigger(ButtonSubHandler f)
    {
        controllerInit.TriggerClicked += f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.TriggerUnclicked += f.unclicked;
        }
    }
    private void asnMenu(ButtonSubHandler f)
    {
        controllerInit.MenuButtonClicked += f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.MenuButtonUnclicked += f.unclicked;
        }
    }
    private void asnPad(ButtonSubHandler f)
    {
        controllerInit.PadClicked += f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.PadUnclicked += f.unclicked;
        }
    }
    private void asnGrip(ButtonSubHandler f)
    {
        controllerInit.Gripped += f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.Ungripped += f.unclicked;
        }
    }

    private void disableTrigger(ButtonSubHandler f)
    {
        controllerInit.TriggerClicked -= f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.TriggerUnclicked -= f.unclicked;
        }
    }
    private void disableMenu(ButtonSubHandler f)
    {
        controllerInit.MenuButtonClicked -= f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.MenuButtonUnclicked -= f.unclicked;
        }
    }
    private void disablePad(ButtonSubHandler f)
    {
        controllerInit.PadClicked -= f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.PadUnclicked -= f.unclicked;
        }
    }
    private void disableGrip(ButtonSubHandler f)
    {
        controllerInit.Gripped -= f.clicked;
        if (f.unclicked != null)
        {
            controllerInit.Ungripped -= f.unclicked;
        }
    }

    public virtual void sub()
	{//Debug.Log ("Subscribing " + this.gameObject);
        foreach (KeyValuePair<button, ButtonSubHandler> assignedButton in buttonMap)
		{
            switch (assignedButton.Key)
            {
			case (button.Trigger):

			//	Debug.Log ("Asigning trigger " + assignedButton.Value);
                    asnTrigger(assignedButton.Value);
                    break;
                case (button.Menu):
                    asnMenu(assignedButton.Value);
                    break;
                case (button.Pad):
                    asnPad(assignedButton.Value);
                    break;
                case (button.Grip):
                    asnGrip(assignedButton.Value);
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void unsub()
    {
        foreach (KeyValuePair<button, ButtonSubHandler> assignedButton in buttonMap)
        {
            switch (assignedButton.Key)
            {
                case (button.Trigger):
                    disableTrigger(assignedButton.Value);
                    break;
                case (button.Menu):
                    disableMenu(assignedButton.Value);
                    break;
                case (button.Pad):
                    disablePad(assignedButton.Value);
                    break;
                case (button.Grip):
                    disableGrip(assignedButton.Value);
                    break;
                default:
                    break;
            }
        }
    }

    void findController()
    {
        switch (assignedController)
        {
            case (controller.Left):
			controllerInit = GameObject.Find("Controller (left)").GetComponent<ImprovedController>();
              //  controllerInit = GameObject.Find("Controller (left)").GetComponent<SteamVR_TrackedController>();
                break;
            case (controller.Right):
               // controllerInit = GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedController>();
			controllerInit = GameObject.Find("Controller (right)").GetComponent<ImprovedController>();
                break;
            default:
                break;
        }
    }

    [Command]
    public void CmdServerAssignClient(NetworkIdentity controlledNetId)
    {
      //  Debug.Log(controlledNetId + "NETIDDDDDDDDDD");
        GameObject Obj = controlledNetId.gameObject;
        Obj.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }

    [Command]
    public void CmdServerRemoveClient(NetworkIdentity controlledNetId)
    {
        GameObject Obj = controlledNetId.gameObject;
        Obj.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }

}
