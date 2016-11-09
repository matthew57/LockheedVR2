using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

public class NewMenuSelector : ITool {



	private GameObject collidedObject;
	private GameObject prevCollidedObject;
	public GameObject menu;


	Text ToolDescript;

	[Serializable]
	public class menuButton
	{
		public GameObject myButton;
		public List<ITool> myTools;
	}


	public List<menuButton> menuButtons = new List<menuButton>();

	public enum state { on, off };
	public state menuState = state.off;
	private int currentMenuIndex = 0;



	void Start()
	{ToolDescript = GameObject.Find ("ToolText").GetComponent<Text>();
		AuxiliaryTool = true;
	

		menuButtons [0].myButton = GameObject.Find ("AxisRotationButton");
		menuButtons [1].myButton = GameObject.Find ("MovePartsButton");
		menuButtons [2].myButton = GameObject.Find ("CuttingPlaneButton");
		menuButtons [3].myButton = GameObject.Find ("MeasurementButton");
		menuButtons [4].myButton = GameObject.Find ("RecordingButton");


	
		var rightController = GameObject.Find("Controller (right)");

		sub ();


		menu = GameObject.Find("Menu");
		menu.SetActive(false);


	}

	void onOffClicked(ClickedEventArgs e)
	{
		if (menuState == state.on)
		{
			ToolDescript.enabled = false;
			menu.SetActive(false);
			menuState = state.off;
		}
		else if (menuState == state.off)
		{
			ToolDescript.enabled = true;
			menu.SetActive(true);
			menuState = state.on;
		}
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



	public override bool TriggerClick(ClickedEventArgs e){
		return false;
	}

	public override bool TriggerUnclick (ClickedEventArgs e){return false;}
	public override bool MenuClick (ClickedEventArgs e){
		onOffClicked (e);

		return true;}
	
	public override bool MenuUnclick (ClickedEventArgs e){return false;}
	public override bool PadClick (ClickedEventArgs e){
		if (menuState == state.off) {
			return false;
		}

			menuButtons[currentMenuIndex].myButton.GetComponent<Image>().color = Color.clear;
			menuButtons[currentMenuIndex].myButton.transform.GetChild(1).GetComponent<Text>().enabled = false;



		if (e.padX > 0) {
			currentMenuIndex++;

			if (currentMenuIndex == menuButtons.Count) {
				currentMenuIndex = 0;
			}
		
		} else {
			currentMenuIndex--;
			if (currentMenuIndex == -1) {
				currentMenuIndex = menuButtons.Count -1;
			}
		
		}
			menuButtons[currentMenuIndex].myButton.GetComponent<Image> ().color = new Color (0, 255, 0, .6f);
			menuButtons[currentMenuIndex].myButton.transform.GetChild (1).GetComponent<Text> ().enabled = true;
		foreach (ITool it in menuButtons[currentMenuIndex].myTools) {
			it.sub ();
		}
			//menuButtons[currentMenuIndex].myTool.sub ();
			


		return true;
	
	}
	public override bool PadUnclick (ClickedEventArgs e){return false;}
	public override bool Grip (ClickedEventArgs e){return false;}
	public override bool UnGrip(ClickedEventArgs e){return false;}
	public override bool PadTouched(ClickedEventArgs e){return false;}
	public override bool PadUntouched(ClickedEventArgs e){return false;}
	public override bool SteamClicked (ClickedEventArgs e){return false;}

	public override void CollisionEnter (Collider collider){

	}
	public override void CollisionExit (Collider collider){
	}
	public override void stopUsing (){}
	public override void startUsing(){}





}
