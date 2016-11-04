using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class MenuSelector : Tools
{
    
    private GameObject collidedObject;
    private GameObject prevCollidedObject;
    private GameObject menu;

    public Dictionary<string,Tools> menueItems = new Dictionary<string, Tools>();

	public Dictionary<string,ITool> IToolItems = new Dictionary<string, ITool>();

    public enum state { on, off };
    public state menuState = state.off;

    public button onOffButton;
    public button selectButton;

    private ButtonSubHandler onOff;
    private ButtonSubHandler select;




    public override void OnStartLocalPlayer()
    {
		//Debug.Log ("initialize menu selector " + this.gameObject);
        base.OnStartLocalPlayer();

       // menueItems.Add("MovePartsButton", gameObject.GetComponent<Grabbing>());
      //  menueItems.Add("CuttingPlaneButton", gameObject.GetComponent<CuttingPlane>());
        //menueItems.Add("AxisRotationButton", gameObject.GetComponent<AxisRotation>());
        //menueItems.Add("MeasurementButton", gameObject.GetComponent<Measurement>());
		//menueItems.Add ("RecordingButton", GameObject.FindObjectOfType<RecordingSystem> ());

		//New Thin
			      

	
		IToolItems.Add("CuttingPlaneButton", gameObject.GetComponent<NewCuttingPlane>());
	
		IToolItems.Add("MeasurementButton", gameObject.GetComponent<NewMeasurement>());
		//IToolItems.Add ("RecordingButton", GameObject.FindObjectOfType<RecordingSystem> ());
		IToolItems.Add ("MovePartsButton", gameObject.GetComponent<NewGrabbing> ());
		IToolItems.Add ("AxisRotationButton", gameObject.GetComponent<NewAxisRotation> ());




        var rightController = GameObject.Find("Controller (right)");


	
        rightController.GetComponent<CollisionCallback>().triggerEnter += controllerTrigEnter;
        rightController.GetComponent<CollisionCallback>().triggerExit += controllerTrigExit;

       // rightController.GetComponent<SteamVR_TrackedController>().TriggerClicked += selectClicked;

		rightController.GetComponent<ImprovedController>().TriggerClicked += selectClicked;
        onOff.clicked = onOffClicked;
        onOff.unclicked = null;
        buttonMap.Add(onOffButton, onOff);
     
        sub();
       
        menu = GameObject.Find("Menu");
        menu.SetActive(false);

      
    }

    void onOffClicked(object sender, ClickedEventArgs e)
    {
        if (menuState == state.on)
        {
            menu.SetActive(false);
            menuState = state.off;
        }
        else if (menuState == state.off)
        {
            menu.SetActive(true);
            menuState = state.on;
        }
    }


    protected void OldScript(object sender, ClickedEventArgs e)
    {
        if (collidedObject == null)
        { return; }
        
        if (menueItems.ContainsKey(collidedObject.name) && collidedObject.tag == "UIObject")
        {

            collidedObject.GetComponent<Image>().color = new Color(0, 255, 0, .6f);
            collidedObject.transform.GetChild(1).GetComponent<Text>().enabled = true;

            if (prevCollidedObject != null)
            {
                prevCollidedObject.GetComponent<Image>().color = Color.clear;
                prevCollidedObject.transform.GetChild(1).GetComponent<Text>().enabled = false;
                menueItems[prevCollidedObject.name].unsub();
            }

			Debug.Log ("Subscribing " + menueItems[collidedObject.name]);
            menueItems[collidedObject.name].sub();
            prevCollidedObject = collidedObject;
        }

        return;

    }

	// TEST CODE FOR NEW ITOOL
	protected void selectClicked(object sender,ClickedEventArgs e)
	{
		if (collidedObject == null)
		{ return; }
		Debug.Log ("Getting clicked " );
		if (IToolItems.ContainsKey(collidedObject.name) && collidedObject.tag == "UIObject")
		{

			collidedObject.GetComponent<Image>().color = new Color(0, 255, 0, .6f);
			collidedObject.transform.GetChild(1).GetComponent<Text>().enabled = true;

			if (prevCollidedObject != null)
			{
				prevCollidedObject.GetComponent<Image>().color = Color.clear;
				prevCollidedObject.transform.GetChild(1).GetComponent<Text>().enabled = false;

				//menueItems[prevCollidedObject.name].unsub();
			}

			Debug.Log ("Tryingto find " + collidedObject.name);
			IToolItems [collidedObject.name].sub();
		
			prevCollidedObject = collidedObject;
		}

		return;

	}




    private void controllerTrigEnter(object sender, Collider collider)
    {
		//Debug.Log ("Entering controller");

        if (prevCollidedObject != null && collider.gameObject == prevCollidedObject)
        {
            return;
        }

		Debug.Log ("Entering " + collider.gameObject);
        if (collider.gameObject.tag == "UIObject" && collider.gameObject.GetComponent<Image>().color != new Color(0, 255, 0, .6f))
		{Debug.Log ("saving "+ collider.gameObject);
            collidedObject = collider.gameObject;
            collidedObject.GetComponent<Image>().color = new Color(0, 255, 255, .6f);
            collidedObject.transform.GetChild(1).GetComponent<Text>().enabled = true;

        }
    }

    private void controllerTrigExit(object sender, Collider collider)
    {

		//Debug.Log ("Exiting controller");
        if (collidedObject != null && collidedObject.tag == "UIObject" && collidedObject.GetComponent<Image>().color != new Color(0, 255, 0, .6f))
        {
            collidedObject.transform.GetChild(1).GetComponent<Text>().enabled = false;
            collidedObject.GetComponent<Image>().color = Color.clear;
        }
        collidedObject = null;
    }

    IEnumerator move(float distance)
    {
        var timer = 0.0;
        for (timer = 0; timer <= 20; timer += .5)
        {

            transform.Rotate(distance, 0, 0);
            timer += Time.deltaTime;
            //new WaitForSeconds(0.1F);
            yield return null;
        }
        yield return null;
    }
}
