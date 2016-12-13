using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;

//This class is used to display the in world menus which will list the files available for playback. 
//Upon calling the menu, it will display, and the cursors should turn on
//Once a file is selected, the menu should be diabled, the panels should be destroyed, and the path of the file should be sent to the recordign system
//----KNOWN ISSUES----------------------------
//The call of the recording systme method needs to be fixed
//The cursors need to be found to turn on and off when the menu turns on and off
public class PlaybackManager : MonoBehaviour {

    string[] recordings;
	private int currentPage;
	private int numPages;
	public GameObject[] icons;

	public GameObject playbackPanel;
	public GameObject transitionButton;

	// Use this for initialization
	void OnEnable () {
        //First, I need to go into the file system and grab all the scenes that have been saved
        //Save those names to a list
        //Create Panels for each scene
        //Make each panel have a tag with it's name
        //Load the scene with that tag
		icons = new GameObject[6];
		//This doesn't work yet
		//ViveCursor vc = GameObject.Find ("[CameraRig]").GetComponentInChildren<ViveCursor> ();
		//vc.enabled = true;
		DestroyIcons ();
        LoadPlayBackScenes();
        DisplayPlayBackScenes();
	}

    private void LoadPlayBackScenes()
    {
        string filepath = Application.dataPath + "/PlaybackFiles/";
        recordings = Directory.GetFiles(filepath, "*.UVR");
    }

    private void DisplayPlayBackScenes()
    {
        Vector3 playerLocation = GameObject.FindObjectOfType<Camera>().transform.position;
        int numRecordings = recordings.Length;
		numPages = numRecordings / 4;
		if (numRecordings % 4 > 0)
			numPages++;
		currentPage = 0;
		CreateIcons();
    }

	private void CreateIcons() 
	{
		//The i in this for loop correlates to an index in the recordings[], which has a list of files
		//It creates a panel, sets the location, and assigns the panels text component to the name of the file
		for (int i = 0; i < 4; i++) {
			int currentIndex = i + (4 * currentPage);
			if (currentIndex < recordings.Length) 
			{
				float xPos = -2.4f;
				float yPos = 4.26f;
				float zPos = 8.72f;
				float yRotation = -20f;
				//Color panelColor = Color.gray;

				int col = i / 2;

				if (col == 1) {
					xPos = -xPos;
					yRotation = -yRotation;
				}
				if ((i + 1) % 2 == 0) {
					yPos -= 2.13f;
					//panelColor = Color.blue;
				}
				GameObject go = (GameObject) Instantiate(playbackPanel);
				go.transform.position = new Vector3 (xPos, yPos, zPos);
				go.transform.eulerAngles = new Vector3 (0, yRotation, 0);
				Canvas[] canvii = go.GetComponentsInChildren<Canvas> ();
				foreach (Canvas c in canvii) {
					Text[] texts = c.GetComponentsInChildren<Text> ();
					foreach (Text t in texts) {					
						t.text = Path.GetFileNameWithoutExtension (recordings [currentIndex]);
						go.name = t.text + ".UVR";
					}
				}
				icons [i] = go;
			}
		}
		if (numPages > (currentPage + 1)) 
		{
			createNextButton ();
		}
		if (currentPage > 0) 
		{
			createPrevButton ();
		}
	}

	public void createNextButton()
	{
		//nextPage button
		float xPos = 5.22f;
		float yPos = 3.20f;
		float zPos = 7.37f;
		float yRotation = 50f;
		GameObject go = (GameObject)Instantiate (transitionButton);
		go.transform.position = new Vector3 (xPos, yPos, zPos);
		go.transform.eulerAngles = new Vector3 (0, yRotation, 0);
		Canvas[] canvii = go.GetComponentsInChildren<Canvas> ();
		foreach (Canvas c in canvii) {
			Text[] texts = c.GetComponentsInChildren<Text> ();
			foreach (Text t in texts) {					
				t.text = ">";
			}
		}
		go.name = "Next";
		icons [4] = go;
	}

	public void createPrevButton()
	{
		//previous page button
		float xPos = -5.22f;
		float yPos = 3.20f;
		float zPos = 7.37f;
		float yRotation = -50f;
		GameObject go = (GameObject)Instantiate (transitionButton);
		go.transform.position = new Vector3 (xPos, yPos, zPos);
		go.transform.eulerAngles = new Vector3 (0, yRotation, 0);
		Canvas[] canvii = go.GetComponentsInChildren<Canvas> ();
		foreach (Canvas c in canvii) {
			Text[] texts = c.GetComponentsInChildren<Text> ();
			foreach (Text t in texts) {					
				t.text = "<";
			}
		}
		go.name = "Back";
		icons [5] = go;
	}

	public void DestroyIcons() {
		for (int i = 0; i < icons.Length; i++) {
			Destroy (icons [i]);
			icons [i] = null;
		}
	}
		
	public void goToNextPage() {
		currentPage++;
		DestroyIcons ();
		CreateIcons ();
	}

	public void goToPrevPage() {
		currentPage--;
		DestroyIcons ();
		CreateIcons ();
	}

	public void ProcessSelection(string selectionName) {
		switch (selectionName) {

		case "Back":
			goToPrevPage ();
			break;
		case "Next":
			goToNextPage ();
			break;
		default:
			if (Path.GetExtension (selectionName) == ".UVR") {
				string path = Application.dataPath + "/PlaybackFiles/" + selectionName;
				//RecordingSystem.setFilePath (path);
				setDisabled ();
			}
			break;
		}
	}

	public void setDisabled() {
		this.gameObject.SetActive (false);
		DestroyIcons ();
		//havent gotten this to work yet
		//ViveCursor vc = GameObject.Find ("[CameraRig]").GetComponentInChildren<ViveCursor> ();
		//vc.enabled = false;
	}
}
