using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

public class NewMenuSelector : ITool {



	private GameObject collidedObject;
	private GameObject prevCollidedObject;
	public GameObject menu;


	Canvas ToolDescript;

	[Serializable]
	public class menuButton
	{
		public GameObject myButton;
		public List<ITool> myTools;
	}


	Image toolDisplay;

	public List<menuButton> menuButtons = new List<menuButton>();


	bool menuOn = false;
	private int currentMenuIndex = 1;



	void Start()
	{
		if (!playBackDevice) {
			menu = GameObject.Find("Menu");
			ToolDescript = GameObject.Find ("ToolText").GetComponent<Canvas> ();
		} else {
			menu = GameObject.Find("MenuB");
			ToolDescript = GameObject.Find ("ToolTextB").GetComponent<Canvas> ();
		}
		AuxiliaryTool = true;
	
		toolDisplay = GameObject.Find ("CurrentToolDisplay").GetComponent<Image> ();

		menuButtons [0].myButton = menu.transform.Find ("AxisRotationButton").gameObject;
		menuButtons [1].myButton = menu.transform.Find ("MovePartsButton").gameObject;
		menuButtons [2].myButton = menu.transform.Find  ("CuttingPlaneButton").gameObject;
		menuButtons [3].myButton = menu.transform.Find ("MeasurementButton").gameObject;
		menuButtons [4].myButton = menu.transform.Find  ("RecordingButton").gameObject;


	
		//var rightController = GameObject.Find("Controller (right)");

		sub ();

		menu.SetActive(false);

		//SUbScribe the default Tools
		menuButtons[currentMenuIndex].myButton.GetComponent<Image> ().color = new Color (0, 255, 0, .6f);
		menuButtons[currentMenuIndex].myButton.transform.GetComponentInChildren<Text>().enabled = true;
		foreach (ITool it in menuButtons[currentMenuIndex].myTools) {
			it.sub ();
		}



	}

	void onOffClicked(ClickedEventArgs e)
	{
		
		menuOn = !menuOn;
		ToolDescript.enabled = menuOn;
		menu.SetActive(menuOn);

	}

	public void setMenuOnOff(bool OnOff)
	{//Debug.Log ("Turning " + OnOff);
		menuOn = OnOff;
		ToolDescript.enabled = OnOff;
		menu.SetActive(OnOff);
	}


	public bool isMenuOn()
	{
		return menuOn;
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



	public override bool TriggerClick(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool TriggerUnclick (ClickedEventArgs e, bool TimeNormal){return false;}


	public override bool MenuClick (ClickedEventArgs e, bool TimeNormal){

	//	Debug.Log ("I am clicked " + this.gameObject);
		onOffClicked (e);

		return true;}
	
	public override bool MenuUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadClick (ClickedEventArgs e, bool TimeNormal){

		//Debug.Log ("Entering");
		if (!menuOn) {
			return false;
		}
	
		menuButtons[currentMenuIndex].myButton.GetComponent<Image>().color = Color.clear;
		menuButtons[currentMenuIndex].myButton.transform.GetComponentInChildren<Text>().enabled = false;


		if (e.padX > 0 ||!TimeNormal) {
				currentMenuIndex++;

				if (currentMenuIndex == menuButtons.Count) {
					currentMenuIndex = 0;
				}
		
			} else {
				currentMenuIndex--;
				if (currentMenuIndex == -1) {
					currentMenuIndex = menuButtons.Count - 1;
				}
		
			}

		if (!playBackDevice) {
			toolDisplay.sprite = menuButtons [currentMenuIndex].myButton.transform.FindChild ("Image").GetComponent<Image> ().sprite;
		}
		setMenuIndex (currentMenuIndex);


		return true;
	
	}
	public override bool PadUnclick (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool Grip (ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool UnGrip(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadTouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool PadUntouched(ClickedEventArgs e, bool TimeNormal){return false;}
	public override bool SteamClicked (ClickedEventArgs e, bool TimeNormal){return false;}

	public override void CollisionEnter (Collider collider){

	}
	public override void CollisionExit (Collider collider){
	}
	public override void stopUsing (){}
	public override void startUsing(){}


	public int getMenuIndex ()
	{
		return currentMenuIndex;
	}


	public void setMenuIndex(int ind){
		
		menuButtons[currentMenuIndex].myButton.GetComponent<Image>().color = Color.clear;
		menuButtons[currentMenuIndex].myButton.transform.GetComponentInChildren<Text>().enabled = false;

		currentMenuIndex = ind;
	//	Debug.Log ("Setting menu index " + ind + "   " + this.gameObject );
		menuButtons [currentMenuIndex].myButton.GetComponent<Image> ().color = new Color (0, 255, 0, .6f);
		menuButtons [currentMenuIndex].myButton.transform.GetComponentInChildren<Text> ().enabled = true;
		foreach (ITool it in menuButtons[currentMenuIndex].myTools) {
			it.sub ();
		
		}
	}

}
